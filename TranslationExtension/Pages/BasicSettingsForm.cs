using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;

namespace TranslationExtension;

internal sealed partial class BasicSettingsForm : ContentPage
{
    private readonly Settings _toolkitSettings = new();
    private readonly TranslationSettings _translationSettings;

    public BasicSettingsForm()
    {
        _translationSettings = SettingsManager.Instance;
        Title = "基本设置";
        BuildSettings();
        _toolkitSettings.SettingsChanged += OnSettingsChanged;
    }

    private void BuildSettings()
    {
        var providerChoices = new System.Collections.Generic.List<ChoiceSetSetting.Choice>
        {
            new ChoiceSetSetting.Choice("GLM", "GLM (智谱 AI)"),
            new ChoiceSetSetting.Choice("Google", "Google Translate"),
        };

        _toolkitSettings.Add(new ChoiceSetSetting("Provider", providerChoices)
        {
            Label = "当前翻译服务商",
            Description = "选择用于翻译的默认引擎",
            Value = _translationSettings.Provider.ToString()
        });
    }

    public override IContent[] GetContent() => _toolkitSettings.ToContent();

    private void OnSettingsChanged(object sender, Settings args)
    {
        var providerValue = _toolkitSettings.GetSetting<string>("Provider");
        if (Enum.TryParse<TranslationProvider>(providerValue, out var provider))
        {
            _translationSettings.Provider = provider;
            SettingsManager.Save(_translationSettings);
        }
    }
}
