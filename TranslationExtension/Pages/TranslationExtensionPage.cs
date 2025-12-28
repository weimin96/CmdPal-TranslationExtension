// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TranslationExtension;

namespace TranslationExtension;

internal sealed partial class TranslationExtensionPage : DynamicListPage
{
    private string _currentSearch = string.Empty;
    private string _selectedPair = string.Empty;
    private string _translationResult = string.Empty;
    private bool _isLoading = false;
    private System.Threading.CancellationTokenSource? _cts;

    public TranslationExtensionPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Name = "快速翻译";
        this.ShowDetails = true; // 开启详情面板以展示长翻译结果
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        var trimmed = newSearch.Trim();
        if (_currentSearch != trimmed)
        {
            _currentSearch = trimmed;
            _selectedPair = string.Empty;
            _translationResult = string.Empty;
            _isLoading = false;
            _cts?.Cancel();
            RaiseItemsChanged();
        }
    }

    public override IListItem[] GetItems()
    {
        if (string.IsNullOrWhiteSpace(_currentSearch))
        {
            return [
                new ListItem(new SettingsPage())
                {
                    Title = "翻译服务配置",
                    Subtitle = "点击进入设置页面配置 API 密钥",
                    Icon = new IconInfo("\uE713")
                }
            ];
        }

        // 如果已经选择了方向并正在加载或已完成
        if (!string.IsNullOrEmpty(_selectedPair))
        {
            var statusItem = new ListItem(new NoOpCommand())
            {
                Title = _isLoading ? "正在努力翻译中..." : "翻译完成",
                Subtitle = _isLoading ? $"源文本: {_currentSearch}" : $"已从 {_selectedPair} 获取结果",
                Icon = new IconInfo(_isLoading ? "\uE895" : "\uE930"),
                Details = new Details()
                {
                    Title = $"{_selectedPair} - 翻译结果",
                    Body = _isLoading ? "请稍候..." : _translationResult
                }
            };
            return [statusItem];
        }

        // 默认显示两个翻译方向供用户选择
        return [
            CreateDirectionItem("🇨🇳 中文 -> 🇺🇸 英文", TranslationSettings.DefaultZhEnPrompt),
            CreateDirectionItem("🇺🇸 英文 -> 🇨🇳 中文", TranslationSettings.DefaultEnZhPrompt)
        ];
    }

    private ListItem CreateDirectionItem(string direction, string prompt)
    {
        return new ListItem(new AnonymousCommand(() => StartTranslation(direction, prompt)))
        {
            Title = direction,
            Subtitle = $"将 \"{_currentSearch}\" {direction.Split(' ')[0]}",
            Icon = new IconInfo("\uF2B7")
        };
    }

    private void StartTranslation(string direction, string prompt)
    {
        _selectedPair = direction;
        _isLoading = true;
        _translationResult = string.Empty;
        RaiseItemsChanged();

        _cts?.Cancel();
        _cts = new System.Threading.CancellationTokenSource();
        var token = _cts.Token;

        System.Threading.Tasks.Task.Run(async () =>
        {
            try
            {
                var result = await TranslationService.TranslateAsync(_currentSearch, prompt);
                if (!token.IsCancellationRequested)
                {
                    _isLoading = false;
                    _translationResult = result;
                    RaiseItemsChanged();
                }
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    _isLoading = false;
                    _translationResult = $"翻译出错: {ex.Message}";
                    RaiseItemsChanged();
                }
            }
        }, token);
    }
}
