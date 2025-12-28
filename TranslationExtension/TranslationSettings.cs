using System;

namespace TranslationExtension;

public enum TranslationProvider
{
    Baidu,
    Google
}

public class TranslationSettings
{
    public const string DefaultZhEnPrompt = "请将以下中文翻译成英文：";
    public const string DefaultEnZhPrompt = "请将以下英文翻译成中文：";
    
    public TranslationProvider Provider { get; set; } = TranslationProvider.Baidu;
    
    // Baidu Settings
    public string BaiduAppId { get; set; } = string.Empty;
    public string BaiduSecretKey { get; set; } = string.Empty;

    // Google Settings
    public string GoogleApiKey { get; set; } = string.Empty;
}
