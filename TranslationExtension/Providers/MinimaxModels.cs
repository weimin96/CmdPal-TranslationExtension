using System;
using System.Text.Json.Serialization;

namespace TranslationExtension.Providers;

/// <summary>
/// MiniMax API 请求模型 (兼容 Anthropic Messages API)
/// </summary>
public class MinimaxRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "MiniMax-M2.7-highspeed";

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 1000;

    [JsonPropertyName("system")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; set; }

    [JsonPropertyName("messages")]
    public MinimaxMessage[] Messages { get; set; } = Array.Empty<MinimaxMessage>();

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
}

/// <summary>
/// MiniMax 消息模型
/// </summary>
public class MinimaxMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public MinimaxContent[] Content { get; set; } = Array.Empty<MinimaxContent>();
}

/// <summary>
/// MiniMax 内容块模型
/// </summary>
public class MinimaxContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    [JsonPropertyName("thinking")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Thinking { get; set; }
}

/// <summary>
/// MiniMax API 响应模型
/// </summary>
public class MinimaxResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public MinimaxContent[]? Content { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; set; }

    [JsonPropertyName("base_resp")]
    public MinimaxBaseResp? BaseResp { get; set; }
}

/// <summary>
/// MiniMax 基础响应模型
/// </summary>
public class MinimaxBaseResp
{
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("status_msg")]
    public string? StatusMsg { get; set; }
}
