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
    private string _lastTranslation = string.Empty;
    private string _currentSearch = string.Empty;
    private System.Threading.CancellationTokenSource? _cts;

    public TranslationExtensionPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Name = "快速翻译1";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearch = newSearch.Trim();
        
        if (string.IsNullOrEmpty(_currentSearch))
        {
            _lastTranslation = string.Empty;
            RaiseItemsChanged();
            return;
        }

        // 防抖处理并触发异步翻译
        _cts?.Cancel();
        _cts = new System.Threading.CancellationTokenSource();
        var token = _cts.Token;

        _lastTranslation = "正在努力翻译中...";
        RaiseItemsChanged();

        System.Threading.Tasks.Task.Run(async () =>
        {
            try
            {
                await System.Threading.Tasks.Task.Delay(500, token); // 500ms 防抖
                var result = await TranslationService.TranslateAsync(_currentSearch);
                
                if (!token.IsCancellationRequested)
                {
                    _lastTranslation = result;
                    RaiseItemsChanged();
                }
            }
            catch (System.OperationCanceledException)
            {
                // 已取消，忽略
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    _lastTranslation = $"错误: {ex.Message}";
                    RaiseItemsChanged();
                }
            }
        }, token);
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

        return [
            new ListItem(new AnonymousCommand(() => {
                // 点击可以执行操作，不返回值
            }))
            {
                Title = _lastTranslation,
                Subtitle = _lastTranslation == "正在努力翻译中..." ? $"正在处理: {_currentSearch}" : "翻译结果 (点击关闭)",
                Icon = new IconInfo("\uE8C9") // Message icon
            }
        ];
    }
}
