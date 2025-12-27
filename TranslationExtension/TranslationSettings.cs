using System;

namespace TranslationExtension;

public enum TranslationProvider
{
    GLM,
    Google
}

public class TranslationSettings
{
    public const string DefaultSystemPrompt = "你是一个专业的翻译助手。";
    public const string DefaultPrompt = "翻译以下文字：";
    public const string DefaultMultiplePrompt = "翻译以下多段文字：";

    public TranslationProvider Provider { get; set; } = TranslationProvider.GLM;
    
    // GLM Settings
    public string GlmApiKey { get; set; } = string.Empty;
    public string GlmModel { get; set; } = "glm-4";
    public string GlmSystemPrompt { get; set; } = DefaultSystemPrompt;
    public string GlmPrompt { get; set; } = DefaultPrompt;
    public string GlmMultiplePrompt { get; set; } = DefaultMultiplePrompt;
    public double GlmTemperature { get; set; } = 0.7;
    public double GlmTopP { get; set; } = 0.7;
    public int GlmMaxTokens { get; set; } = 2000;

    // Google Settings
    public string GoogleApiKey { get; set; } = string.Empty;

    // Common Settings
    public string SourceLanguage { get; set; } = "Auto";
    public string TargetLanguage { get; set; } = "zh-CN";
}
