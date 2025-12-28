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
            if (settings.Provider == TranslationProvider.Baidu)
            {
                return await TranslateWithBaidu(text, settings);
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

    private static async Task<string> TranslateWithBaidu(string text, TranslationSettings settings)
    {
        if (string.IsNullOrEmpty(settings.BaiduAppId) || string.IsNullOrEmpty(settings.BaiduSecretKey))
            return "请先在设置中配置 Baidu API Credentials";

        string q = text;
        string from = "auto";
        string to = "zh"; // Default to zh
        
        // Simple direction check logic can be passed in or inferred, 
        // but Baidu auto-detects 'from'. 
        // We need 'to' language. logic in Page handles display direction, 
        // but ideally we pass target lang. 
        // For now, let's infer 'to' based on if text contains Chinese.
        bool isChinese = false;
        foreach (char c in text) { if (c >= 0x4E00 && c <= 0x9FFF) { isChinese = true; break; } }
        to = isChinese ? "en" : "zh";

        string appId = settings.BaiduAppId;
        string secretKey = settings.BaiduSecretKey;
        string salt = new Random().Next(10000, 99999).ToString();
        string sign = MD5Encrypt(appId + q + salt + secretKey);
        
        string url = $"http://api.fanyi.baidu.com/api/trans/vip/translate?q={Uri.EscapeDataString(q)}&from={from}&to={to}&appid={appId}&salt={salt}&sign={sign}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var resultJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(resultJson);
        var root = doc.RootElement;

        if (root.TryGetProperty("error_code", out var errorCode))
        {
           string code = errorCode.ToString();
           if(code != "52000" && code != "0") // 52000 is success generally, but usually error_code only appears on error? Verify API.
           {
               // If error_code exists and is not success code
                if (root.TryGetProperty("error_msg", out var errorMsg))
                    return $"Baidu Error: {errorMsg.GetString()} ({code})";
                return $"Baidu Error Code: {code}";
           }
        }

        if (root.TryGetProperty("trans_result", out var transResult))
        {
             var sb = new StringBuilder();
             foreach (var item in transResult.EnumerateArray())
             {
                 if (sb.Length > 0) sb.AppendLine();
                 sb.Append(item.GetProperty("dst").GetString());
             }
             return sb.ToString();
        }

        return "未获取到翻译结果";
    }

    private static string MD5Encrypt(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("x2"));
        }
        return sb.ToString();
    }    

    private static async Task<string> TranslateWithGoogle(string text, TranslationSettings settings)
    {
        // 简化的 Google 翻译占位实现
        return $"[Google] 翻译结果: {text} (请配置有效的 Google API 处理逻辑)";
    }
}
