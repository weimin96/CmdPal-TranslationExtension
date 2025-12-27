using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TranslationExtension;

internal sealed partial class ProviderSettingsForm : ContentPage
{
    private readonly Settings _toolkitSettings = new();
    private readonly TranslationSettings _translationSettings;
    private readonly TranslationProvider _provider;

    public ProviderSettingsForm(TranslationProvider provider)
    {
        _provider = provider;
        _translationSettings = SettingsManager.Instance;
        Title = $"{provider} 参数配置";
        BuildSettings();
        _toolkitSettings.SettingsChanged += OnSettingsChanged;
    }

    private void BuildSettings()
    {
        if (_provider == TranslationProvider.GLM)
        {
            _toolkitSettings.Add(new TextSetting("GlmApiKey", _translationSettings.GlmApiKey)
            {
                Label = "API KEY",
                Placeholder = "智谱 AI 密钥",
            });

            _toolkitSettings.Add(new TextSetting("GlmSystemPrompt", _translationSettings.GlmSystemPrompt)
            {
                Label = "系统提示(System)",
                Placeholder = "助手角色定义"
            });

            _toolkitSettings.Add(new TextSetting("GlmPrompt", _translationSettings.GlmPrompt)
            {
                Label = "用户指令(User)",
                Placeholder = "翻译具体指令"
            });

            _toolkitSettings.Add(new TextSetting("GlmMaxTokens", _translationSettings.GlmMaxTokens.ToString())
            {
                Label = "最大长度(Tokens)",
            });

            _toolkitSettings.Add(new TextSetting("GlmTemperature", _translationSettings.GlmTemperature.ToString("F2"))
            {
                Label = "发散度(Temp)",
            });
        }
        else
        {
            _toolkitSettings.Add(new TextSetting("GoogleApiKey", _translationSettings.GoogleApiKey)
            {
                Label = "API KEY",
                Placeholder = "Google Cloud 密钥",
            });
        }
    }

    public override IContent[] GetContent() => _toolkitSettings.ToContent();

    private void OnSettingsChanged(object sender, Settings args)
    {
        if (_provider == TranslationProvider.GLM)
        {
            _translationSettings.GlmApiKey = _toolkitSettings.GetSetting<string>("GlmApiKey")?.Trim() ?? string.Empty;
            _translationSettings.GlmSystemPrompt = _toolkitSettings.GetSetting<string>("GlmSystemPrompt") ?? TranslationSettings.DefaultSystemPrompt;
            _translationSettings.GlmPrompt = _toolkitSettings.GetSetting<string>("GlmPrompt") ?? TranslationSettings.DefaultPrompt;
            
            if (int.TryParse(_toolkitSettings.GetSetting<string>("GlmMaxTokens"), out var tokens))
                _translationSettings.GlmMaxTokens = tokens;
                
            if (double.TryParse(_toolkitSettings.GetSetting<string>("GlmTemperature"), out var temp))
                _translationSettings.GlmTemperature = temp;
        }
        else
        {
            _translationSettings.GoogleApiKey = _toolkitSettings.GetSetting<string>("GoogleApiKey")?.Trim() ?? string.Empty;
        }

        SettingsManager.Save(_translationSettings);
    }
}
