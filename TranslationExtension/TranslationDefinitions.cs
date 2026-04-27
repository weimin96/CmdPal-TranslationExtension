using System.Collections.Generic;

namespace TranslationExtension;

public static class TranslationDefinitions
{
    public record Choice(string Title, string Value);

    public static readonly List<Choice> Providers = new()
    {
        new("Baidu 翻译", "Baidu"),
        new("DeepSeek", "DeepSeek"),
        new("智谱AI", "Glm"),
        new("MiniMax", "Minimax")
    };

    public static readonly List<Choice> DeepSeekModels = new()
    {
        new("deepseek-v4-flash", "deepseek-v4-flash"),
        new("deepseek-v4-pro", "deepseek-v4-pro")
    };

    public static readonly List<Choice> GlmModels = new()
    {
        new("glm-5.1", "glm-5.1"),
        new("glm-5-turbo", "glm-5-turbo"),
        new("glm-5", "glm-5"),
        new("glm-4.7", "glm-4.7"),
        new("glm-4.7-flash", "glm-4.5-flash"),
        new("glm-4.7-flashx", "glm-4.7-flashx"),
        new("glm-4.6", "glm-4.6"),
        new("glm-4.5-air", "glm-4.5-air"),
        new("glm-4.5-airx", "glm-4.5-airx"),
        new("glm-4.5-flash", "glm-4.5-flash"),
        new("glm-4-flash-250414", "glm-4-flash-250414")
    };

    public static readonly List<Choice> MinimaxModels = new()
    {
        new("MiniMax-M2.7", "MiniMax-M2.7"),
        new("MiniMax-M2.7-highspeed", "MiniMax-M2.7-highspeed"),
        new("MiniMax-M2.5", "MiniMax-M2.5"),
        new("MiniMax-M2.5-highspeed", "MiniMax-M2.5-highspeed"),
        new("MiniMax-M2.1", "MiniMax-M2.1"),
        new("MiniMax-M2.1-highspeed", "MiniMax-M2.1-highspeed")
    };

    public static string GetChoicesJson(List<Choice> choices)
    {
        var items = new List<string>();
        foreach (var choice in choices)
        {
            items.Add($$"""
            {
                "title": "{{choice.Title}}",
                "value": "{{choice.Value}}"
            }
            """);
        }
        return $"[{string.Join(",", items)}]";
    }
}
