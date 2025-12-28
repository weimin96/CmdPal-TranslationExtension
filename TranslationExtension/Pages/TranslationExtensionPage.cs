// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TranslationExtension;

namespace TranslationExtension;

internal sealed partial class TranslationExtensionPage : DynamicListPage, IDisposable
{
    private List<ListItem> _allItems = new();
    private string _currentSearch = string.Empty;
    private System.Threading.CancellationTokenSource? _cts;
    private Task<string>? _translationTask;

    public TranslationExtensionPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "TranslateCmdPal";
        Name = "Open";
        this.ShowDetails = true;
        
        // Initialize with default items (Settings)
        UpdateListWithSettings();
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    public override async void UpdateSearchText(string oldSearch, string newSearch)
    {
        var trimmed = newSearch.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            _currentSearch = string.Empty;
            _cts?.Cancel();
            UpdateListWithSettings();
            RaiseItemsChanged(_allItems.Count);
            return;
        }

        if (_currentSearch == trimmed)
        {
            return;
        }

        _currentSearch = trimmed;
        
        // Cancel previous task
        _cts?.Cancel();
        _cts = new System.Threading.CancellationTokenSource();
        var token = _cts.Token;

        // Determine direction based on content (Simple heuristic: contains Chinese -> to English, else -> to Chinese)
        bool isChinese = ContainsChinese(trimmed);
        string prompt = isChinese ? TranslationSettings.DefaultZhEnPrompt : TranslationSettings.DefaultEnZhPrompt;
        string directionLabel = isChinese ? "中文 -> 英文" : "英文 -> 中文";

        // Show loading state
        _allItems.Clear();
        _allItems.Add(new ListItem(new NoOpCommand())
        {
            Title = "正在翻译...",
            Subtitle = $"{directionLabel}: {trimmed}",
            Icon = new IconInfo("\uE895"), // Clock/Loading icon
            Details = new Details { Title = "翻译中", Body = "请稍候..." }
        });
        RaiseItemsChanged(_allItems.Count);

        try
        {
            // Debounce slightly
            await Task.Delay(300, token);

            // Double check cancellation before precise work
            if (token.IsCancellationRequested) return;

            var result = await TranslationService.TranslateAsync(trimmed, prompt);
            
            if (!token.IsCancellationRequested && !string.IsNullOrEmpty(result))
            {
                // Update UI on completion
                _allItems.Clear();
                _allItems.Add(new ListItem(new AnonymousCommand(() => 
                {
                   var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                   dataPackage.SetText(result);
                   Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
                }))
                {
                    Title = result,
                    Subtitle = $"{directionLabel}: {trimmed}",
                    Icon = new IconInfo("\uE930"), // Checkmark/Completed
                    Details = new Details 
                    { 
                        Title = "翻译结果", 
                        Body = result 
                    }
                });
                RaiseItemsChanged(_allItems.Count);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
            {
                 _allItems.Clear();
                _allItems.Add(new ListItem(new NoOpCommand())
                {
                    Title = "Error",
                    Subtitle = ex.Message,
                    Icon = new IconInfo("\uE711")
                });
                RaiseItemsChanged(_allItems.Count);
            }
        }
    }
    
    // --- Helper Methods ---

    private void UpdateListWithSettings()
    {
        _allItems.Clear();
        _allItems.Add(new ListItem(new SettingsPage())
        {
            Title = "翻译服务配置",
            Subtitle = "点击进入设置页面配置 API 密钥",
            Icon = new IconInfo("\uE713")
        });
    }

    private static bool ContainsChinese(string text)
    {
        // Simple range check for CJK Unified Ideographs
        foreach (char c in text)
        {
            if (c >= 0x4E00 && c <= 0x9FFF) return true;
        }
        return false;
    }

    public override IListItem[] GetItems()
    {
        return _allItems.ToArray();
    }
}
