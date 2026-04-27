using System;

namespace TranslationExtension;

public enum TranslationProvider
{
    Baidu,
    Google,
    DeepSeek,
    Glm,
    Minimax
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

    // DeepSeek Settings
    public string DeepSeekApiKey { get; set; } = string.Empty;
    public string DeepSeekModel { get; set; } = "deepseek-v4-flash";

    // GLM Settings
    public string GlmApiKey { get; set; } = string.Empty;
    public string GlmModel { get; set; } = "glm-5.1";

    // MiniMax Settings
    public string MinimaxApiKey { get; set; } = string.Empty;
    public string MinimaxModel { get; set; } = "MiniMax-M2.7-highspeed";
}
