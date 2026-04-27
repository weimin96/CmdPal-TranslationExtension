using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TranslationExtension;

/// <summary>
/// 设置表单保持单一服务商配置可见，避免未选服务商字段干扰填写。
/// </summary>
internal sealed partial class SettingsFormContent : FormContent
{
    private const string ProviderPropertyName = "Provider";

    private TranslationProvider _selectedProvider = GetConfigurableProvider(SettingsManager.Instance.Provider);

    public SettingsFormContent()
    {
        RenderForm();
    }

    /// <summary>
    /// 页面重新进入时以已保存配置为准，避免上一次未保存的选择污染当前表单。
    /// </summary>
    public void Refresh()
    {
        _selectedProvider = GetConfigurableProvider(SettingsManager.Instance.Provider);
        RenderForm();
    }

    public override ICommandResult SubmitForm(string payload)
    {
        return SubmitForm(payload, "{}");
    }

    public override ICommandResult SubmitForm(string payload, string state)
    {
        try
        {
            var formData = ParseJsonObject(payload);
            var actionData = ParseJsonObject(state);

            if (!TryReadProvider(formData, actionData, out var provider))
            {
                ShowStatus("请选择可用的翻译服务商。");
                return CommandResult.KeepOpen();
            }

            SaveSubmittedSettings(formData, provider);
            ShowStatus("翻译设置已保存。");
            return CommandResult.GoHome();
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"设置表单数据格式无效：{ex.Message}");
            ShowStatus("设置表单数据格式无效，未保存设置。");
            return CommandResult.KeepOpen();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存设置失败：{ex.Message}");
            ShowStatus("保存设置失败，请稍后重试。");
            return CommandResult.KeepOpen();
        }
    }

    private void RenderForm()
    {
        TemplateJson = GetTemplate();
        DataJson = GetDataJson();
    }

    private string GetDataJson()
    {
        var settings = SettingsManager.Instance;
        var data = new JsonObject
        {
            [ProviderPropertyName] = _selectedProvider.ToString(),
            ["BaiduAppId"] = settings.BaiduAppId,
            ["BaiduSecretKey"] = settings.BaiduSecretKey,
            ["DeepSeekApiKey"] = settings.DeepSeekApiKey,
            ["DeepSeekModel"] = settings.DeepSeekModel,
            ["GlmApiKey"] = settings.GlmApiKey,
            ["GlmModel"] = settings.GlmModel,
            ["MinimaxApiKey"] = settings.MinimaxApiKey,
            ["MinimaxModel"] = settings.MinimaxModel,
        };

        return data.ToJsonString();
    }

    private static JsonObject ParseJsonObject(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonNode.Parse(json)?.AsObject() ?? [];
    }

    private static bool TryReadProvider(JsonObject formData, JsonObject actionData, out TranslationProvider provider)
    {
        provider = TranslationProvider.Baidu;

        var providerName = ReadString(actionData, ProviderPropertyName, ReadString(formData, ProviderPropertyName));
        if (!Enum.TryParse(providerName, out provider))
        {
            return false;
        }

        return IsConfigurableProvider(provider);
    }

    private static TranslationProvider GetConfigurableProvider(TranslationProvider provider)
    {
        return IsConfigurableProvider(provider) ? provider : TranslationProvider.Baidu;
    }

    private static bool IsConfigurableProvider(TranslationProvider provider)
    {
        return provider is TranslationProvider.Baidu
            or TranslationProvider.DeepSeek
            or TranslationProvider.Glm
            or TranslationProvider.Minimax;
    }

    private static void SaveSubmittedSettings(JsonObject formData, TranslationProvider provider)
    {
        var settings = CreateSettingsCopy(SettingsManager.Instance);
        settings.Provider = provider;

        switch (provider)
        {
            case TranslationProvider.Baidu:
                SaveBaiduSettings(formData, settings);
                break;
            case TranslationProvider.DeepSeek:
                SaveDeepSeekSettings(formData, settings);
                break;
            case TranslationProvider.Glm:
                SaveGlmSettings(formData, settings);
                break;
            case TranslationProvider.Minimax:
                SaveMinimaxSettings(formData, settings);
                break;
            default:
                throw new InvalidOperationException("当前服务商不支持在设置页配置。");
        }

        SettingsManager.Save(settings);
    }

    private static TranslationSettings CreateSettingsCopy(TranslationSettings source)
    {
        return new TranslationSettings
        {
            Provider = source.Provider,
            BaiduAppId = source.BaiduAppId,
            BaiduSecretKey = source.BaiduSecretKey,
            GoogleApiKey = source.GoogleApiKey,
            DeepSeekApiKey = source.DeepSeekApiKey,
            DeepSeekModel = source.DeepSeekModel,
            GlmApiKey = source.GlmApiKey,
            GlmModel = source.GlmModel,
            MinimaxApiKey = source.MinimaxApiKey,
            MinimaxModel = source.MinimaxModel,
        };
    }

    private static void SaveBaiduSettings(JsonObject formData, TranslationSettings settings)
    {
        settings.BaiduAppId = ReadString(formData, "BaiduAppId");
        settings.BaiduSecretKey = ReadString(formData, "BaiduSecretKey");
    }

    private static void SaveDeepSeekSettings(JsonObject formData, TranslationSettings settings)
    {
        settings.DeepSeekApiKey = ReadString(formData, "DeepSeekApiKey");
        settings.DeepSeekModel = ReadString(formData, "DeepSeekModel", settings.DeepSeekModel);
    }

    private static void SaveGlmSettings(JsonObject formData, TranslationSettings settings)
    {
        settings.GlmApiKey = ReadString(formData, "GlmApiKey");
        settings.GlmModel = ReadString(formData, "GlmModel", settings.GlmModel);
    }

    private static void SaveMinimaxSettings(JsonObject formData, TranslationSettings settings)
    {
        settings.MinimaxApiKey = ReadString(formData, "MinimaxApiKey");
        settings.MinimaxModel = ReadString(formData, "MinimaxModel", settings.MinimaxModel);
    }

    private static string ReadString(JsonObject data, string propertyName, string fallbackValue = "")
    {
        if (!data.TryGetPropertyValue(propertyName, out var value))
        {
            return fallbackValue;
        }

        return value?.GetValue<string>() ?? fallbackValue;
    }

    private static void ShowStatus(string message)
    {
        new ToastStatusMessage(message).Show();
    }

    private string GetTemplate()
    {
        var providerActions = GetProviderSelectionActionsTemplate();

        return $$"""
        {
            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
            "type": "AdaptiveCard",
            "version": "1.6",
            "body": [
                {
                    "type": "TextBlock",
                    "text": "翻译插件设置",
                    "size": "Large",
                    "weight": "Bolder",
                    "style": "heading",
                    "wrap": true
                },
                {
                    "type": "TextBlock",
                    "text": "选择服务商并填写对应凭据",
                    "size": "Small",
                    "isSubtle": true,
                    "wrap": true,
                    "spacing": "None"
                },
                {
                    "type": "Container",
                    "style": "emphasis",
                    "spacing": "Large",
                    "items": [
                        {
                            "type": "TextBlock",
                            "text": "翻译服务商",
                            "weight": "Bolder",
                            "wrap": true
                        },
                        {
                            "type": "ActionSet",
                            "spacing": "Small",
                            "actions": [
                                {{providerActions}}
                            ]
                        }
                    ]
                },
                {{GetBaiduSettingsTemplate()}},
                {{GetDeepSeekSettingsTemplate()}},
                {{GetGlmSettingsTemplate()}},
                {{GetMinimaxSettingsTemplate()}}
            ]
        }
        """;
    }

    private static string GetProviderSelectionActionsTemplate()
    {
        var actions = new[]
        {
            GetProviderVisibilityActionTemplate("百度翻译", "baiduSettings"),
            GetProviderVisibilityActionTemplate("DeepSeek", "deepSeekSettings"),
            GetProviderVisibilityActionTemplate("智谱 AI", "glmSettings"),
            GetProviderVisibilityActionTemplate("MiniMax", "minimaxSettings"),
        };

        return string.Join(",", actions);
    }

    private static string GetProviderVisibilityActionTemplate(string title, string visibleElementId)
    {
        return $$"""
        {
            "type": "Action.ToggleVisibility",
            "title": "{{title}}",
            "targetElements": [
                {
                    "elementId": "baiduSettings",
                    "isVisible": {{ToJsonBoolean(visibleElementId == "baiduSettings")}}
                },
                {
                    "elementId": "deepSeekSettings",
                    "isVisible": {{ToJsonBoolean(visibleElementId == "deepSeekSettings")}}
                },
                {
                    "elementId": "glmSettings",
                    "isVisible": {{ToJsonBoolean(visibleElementId == "glmSettings")}}
                },
                {
                    "elementId": "minimaxSettings",
                    "isVisible": {{ToJsonBoolean(visibleElementId == "minimaxSettings")}}
                }
            ]
        }
        """;
    }

    private static string ToJsonBoolean(bool value)
    {
        return value ? "true" : "false";
    }

    private string GetBaiduSettingsTemplate()
    {
        return $$"""
        {
            "type": "Container",
            "id": "baiduSettings",
            "isVisible": {{ToJsonBoolean(_selectedProvider == TranslationProvider.Baidu)}},
            "spacing": "Medium",
            "separator": true,
            "items": [
                {
                    "type": "TextBlock",
                    "text": "百度翻译凭据",
                    "size": "Medium",
                    "weight": "Bolder",
                    "wrap": true
                },
                {
                    "type": "Input.Text",
                    "id": "BaiduAppId",
                    "label": "App ID",
                    "placeholder": "输入百度翻译 App ID",
                    "value": "${BaiduAppId}"
                },
                {
                    "type": "Input.Text",
                    "id": "BaiduSecretKey",
                    "label": "Secret Key",
                    "placeholder": "输入百度翻译密钥",
                    "value": "${BaiduSecretKey}",
                    "style": "Password"
                },
                {
                    "type": "ActionSet",
                    "actions": [
                        {
                            "type": "Action.Submit",
                            "title": "保存百度翻译设置",
                            "style": "positive",
                            "data": {
                                "Provider": "Baidu"
                            }
                        }
                    ]
                }
            ]
        }
        """;
    }

    private string GetDeepSeekSettingsTemplate()
    {
        return $$"""
        {
            "type": "Container",
            "id": "deepSeekSettings",
            "isVisible": {{ToJsonBoolean(_selectedProvider == TranslationProvider.DeepSeek)}},
            "spacing": "Medium",
            "separator": true,
            "items": [
                {
                    "type": "TextBlock",
                    "text": "DeepSeek 凭据",
                    "size": "Medium",
                    "weight": "Bolder",
                    "wrap": true
                },
                {
                    "type": "Input.Text",
                    "id": "DeepSeekApiKey",
                    "label": "API Key",
                    "placeholder": "输入 DeepSeek API Key",
                    "value": "${DeepSeekApiKey}",
                    "style": "Password"
                },
                {
                    "type": "Input.ChoiceSet",
                    "id": "DeepSeekModel",
                    "label": "模型",
                    "value": "${DeepSeekModel}",
                    "choices": {{TranslationDefinitions.GetChoicesJson(TranslationDefinitions.DeepSeekModels)}}
                },
                {
                    "type": "ActionSet",
                    "actions": [
                        {
                            "type": "Action.Submit",
                            "title": "保存 DeepSeek 设置",
                            "style": "positive",
                            "data": {
                                "Provider": "DeepSeek"
                            }
                        }
                    ]
                }
            ]
        }
        """;
    }

    private string GetGlmSettingsTemplate()
    {
        return $$"""
        {
            "type": "Container",
            "id": "glmSettings",
            "isVisible": {{ToJsonBoolean(_selectedProvider == TranslationProvider.Glm)}},
            "spacing": "Medium",
            "separator": true,
            "items": [
                {
                    "type": "TextBlock",
                    "text": "智谱 AI 凭据",
                    "size": "Medium",
                    "weight": "Bolder",
                    "wrap": true
                },
                {
                    "type": "Input.Text",
                    "id": "GlmApiKey",
                    "label": "API Key",
                    "placeholder": "输入智谱 AI API Key",
                    "value": "${GlmApiKey}",
                    "style": "Password"
                },
                {
                    "type": "Input.ChoiceSet",
                    "id": "GlmModel",
                    "label": "模型",
                    "value": "${GlmModel}",
                    "choices": {{TranslationDefinitions.GetChoicesJson(TranslationDefinitions.GlmModels)}}
                },
                {
                    "type": "ActionSet",
                    "actions": [
                        {
                            "type": "Action.Submit",
                            "title": "保存智谱 AI 设置",
                            "style": "positive",
                            "data": {
                                "Provider": "Glm"
                            }
                        }
                    ]
                }
            ]
        }
        """;
    }

    private string GetMinimaxSettingsTemplate()
    {
        return $$"""
        {
            "type": "Container",
            "id": "minimaxSettings",
            "isVisible": {{ToJsonBoolean(_selectedProvider == TranslationProvider.Minimax)}},
            "spacing": "Medium",
            "separator": true,
            "items": [
                {
                    "type": "TextBlock",
                    "text": "MiniMax 凭据",
                    "size": "Medium",
                    "weight": "Bolder",
                    "wrap": true
                },
                {
                    "type": "Input.Text",
                    "id": "MinimaxApiKey",
                    "label": "API Key",
                    "placeholder": "输入 MiniMax API Key",
                    "value": "${MinimaxApiKey}",
                    "style": "Password"
                },
                {
                    "type": "Input.ChoiceSet",
                    "id": "MinimaxModel",
                    "label": "模型",
                    "value": "${MinimaxModel}",
                    "choices": {{TranslationDefinitions.GetChoicesJson(TranslationDefinitions.MinimaxModels)}}
                },
                {
                    "type": "ActionSet",
                    "actions": [
                        {
                            "type": "Action.Submit",
                            "title": "保存 MiniMax 设置",
                            "style": "positive",
                            "data": {
                                "Provider": "Minimax"
                            }
                        }
                    ]
                }
            ]
        }
        """;
    }
}
