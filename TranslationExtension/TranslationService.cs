using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TranslationExtension;

public class TranslationService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<string> TranslateAsync(string text, string? promptOverride = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var settings = SettingsManager.Instance;
        try
        {
            if (settings.Provider == TranslationProvider.GLM)
            {
                return await TranslateWithGlm(text, settings, promptOverride);
            }
            else
            {
                return await TranslateWithGoogle(text, settings);
            }
        }
        catch (Exception ex)
        {
            return $"翻译出错: {ex.Message}";
        }
    }

    private static async Task<string> TranslateWithGlm(string text, TranslationSettings settings, string? promptOverride)
    {
        if (string.IsNullOrEmpty(settings.GlmApiKey)) return "请先在设置中配置 GLM API Key";

        var url = "https://open.bigmodel.cn/api/paas/v4/chat/completions";
        var prompt = promptOverride ?? settings.GlmPrompt;
        var requestBody = new
        {
            model = settings.GlmModel,
            messages = new[]
            {
                new { role = "system", content = settings.GlmSystemPrompt },
                new { role = "user", content = $"{prompt}\n{text}" }
            },
            temperature = settings.GlmTemperature,
            top_p = settings.GlmTopP,
            max_tokens = settings.GlmMaxTokens
        };

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {settings.GlmApiKey}");
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "翻译结果为空";
    }

    private static async Task<string> TranslateWithGoogle(string text, TranslationSettings settings)
    {
        // 简化的 Google 翻译占位实现
        return $"[Google] 翻译结果: {text} (请配置有效的 Google API 处理逻辑)";
    }
}
