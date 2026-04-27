using System;
using System.Text.Json.Serialization;

namespace TranslationExtension.Providers;

public class DeepSeekRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "deepseek-v4-flash";

    [JsonPropertyName("messages")]
    public DeepSeekMessage[] Messages { get; set; } = Array.Empty<DeepSeekMessage>();

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
}

public class DeepSeekMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class DeepSeekResponse
{
    [JsonPropertyName("choices")]
    public DeepSeekChoice[]? Choices { get; set; }

    [JsonPropertyName("error")]
    public DeepSeekError? Error { get; set; }
}

public class DeepSeekChoice
{
    [JsonPropertyName("message")]
    public DeepSeekMessage? Message { get; set; }
}

public class DeepSeekError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
