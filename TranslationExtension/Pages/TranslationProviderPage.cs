using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TranslationExtension;

internal sealed partial class TranslationProviderPage : ListPage
{
    public TranslationProviderPage()
    {
        Title = "翻译服务";
        Name = "Providers";
        Icon = new IconInfo("\uE9CE"); // World icon
    }

    public override IListItem[] GetItems()
    {
        return [
            new ListItem(new ProviderSettingsForm(TranslationProvider.GLM))
            {
                Title = "GLM (智谱 AI)",
                Subtitle = "配置 GLM 模型参数",
                Icon = new IconInfo("\uE99A") // Bot icon
            },
            new ListItem(new ProviderSettingsForm(TranslationProvider.Google))
            {
                Title = "Google Translate",
                Subtitle = "配置 Google API 密钥",
                Icon = new IconInfo("\uF2B7") // Google icon style
            }
        ];
    }
}
