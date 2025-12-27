using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TranslationExtension;

internal sealed partial class SettingsPage : ListPage
{
    public SettingsPage()
    {
        Title = "翻译插件设置";
        Name = "Settings";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
    }

    public override IListItem[] GetItems()
    {
        return [
            new ListItem(new BasicSettingsForm())
            {
                Title = "基本设置",
                Subtitle = "选择默认翻译服务商",
                Icon = new IconInfo("\uE713") // Settings icon
            },
            new ListItem(new TranslationProviderPage())
            {
                Title = "翻译服务",
                Subtitle = "配置各服务商详细参数",
                Icon = new IconInfo("\uF2B7") // Server/Globe icon
            }
        ];
    }
}
