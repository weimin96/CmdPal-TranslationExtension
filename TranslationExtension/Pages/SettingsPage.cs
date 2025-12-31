using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TranslationExtension;

internal sealed partial class SettingsPage : ContentPage
{
    private readonly Settings _settingsForm = new();
    private readonly TranslationSettings _appSettings;

    public SettingsPage()
    {
        Title = "翻译插件设置";
        Name = "Settings";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        _appSettings = SettingsManager.Instance;

        // Provider Choice
        var providers = new List<ChoiceSetSetting.Choice>
        {
            new("Baidu Translate", "Baidu"),
            new("Google Translate", "Google")
        };
        _settingsForm.Add(new ChoiceSetSetting("Provider", providers)
        {
            Label = "翻译服务商",
            Description = "选择首选翻译引擎",
            Value = _appSettings.Provider.ToString()
        });

        // Baidu Settings
        _settingsForm.Add(new TextSetting("BaiduAppId", _appSettings.BaiduAppId)
        {
            Label = "Baidu App ID",
            Description = "输入百度翻译 API App ID"
        });
        _settingsForm.Add(new TextSetting("BaiduSecretKey", _appSettings.BaiduSecretKey)
        {
            Label = "Baidu Secret Key",
            Description = "输入百度翻译 API 密钥"
        });
        // 百度翻译 API 快速跳转链接
        //_settingsForm.Add(new LinkSetting("BaiduApiLink", "https://fanyi-api.baidu.com/")
        //{
        //    Label = "百度翻译开发者平台",
        //    Description = "点击获取 App ID 和密钥"
        //});

        // Google Settings
        _settingsForm.Add(new TextSetting("GoogleApiKey", _appSettings.GoogleApiKey)
        {
            Label = "Google API Key",
            Description = "输入 Google Translate API Key"
        });

        _settingsForm.SettingsChanged += SettingsChanged;
    }

    public override IContent[] GetContent()
    {
        return _settingsForm.ToContent();
    }

    private void SettingsChanged(object sender, Settings args)
    {
        // Update AppSettings from the form settings
        if (Enum.TryParse<TranslationProvider>(args.GetSetting<string>("Provider"), out var p))
        {
            _appSettings.Provider = p;
        }

        _appSettings.BaiduAppId = args.GetSetting<string>("BaiduAppId") ?? string.Empty;
        _appSettings.BaiduSecretKey = args.GetSetting<string>("BaiduSecretKey") ?? string.Empty;
        _appSettings.GoogleApiKey = args.GetSetting<string>("GoogleApiKey") ?? string.Empty;

        SettingsManager.Save(_appSettings);
        
        // Optional: Show a subtle notification or log? 
        // ExtensionHost.LogMessage(new LogMessage { Message = "Settings Saved" });
    }
}
