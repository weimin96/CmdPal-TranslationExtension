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

        // 原文
        string q = text;
        // 源语言，百度支持自动识别
        string from = "auto";
        // 目标语言，默认中文
        string to = "zh";
        
        // 自动判定翻译方向：如果包含中文则翻译成英文，否则翻译成中文
        bool isChinese = false;
        foreach (char c in text) { if (c >= 0x4E00 && c <= 0x9FFF) { isChinese = true; break; } }
        to = isChinese ? "en" : "zh";

        // 改成您的APP ID
        string appId = settings.BaiduAppId;
        // 改成您的密钥
        string secretKey = settings.BaiduSecretKey;
        
        // 随机数
        Random rd = new Random();
        string salt = rd.Next(10000, 99999).ToString();
        
        // 计算签名：sign = md5(appid + q + salt + secretKey)
        string sign = MD5Encrypt(appId + q + salt + secretKey);
        
        // 百度翻译 API 地址
        // 使用 HTTPS 协议：https://api.fanyi.baidu.com/api/trans/vip/translate
        // q 参数需要进行 UrlEncode
        string url = "https://api.fanyi.baidu.com/api/trans/vip/translate?";
        url += "q=" + System.Net.WebUtility.UrlEncode(q);
        url += "&from=" + from;
        url += "&to=" + to;
        url += "&appid=" + appId;
        url += "&salt=" + salt;
        url += "&sign=" + sign;

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var resultJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(resultJson);
        var root = doc.RootElement;

        // 检查错误码
        if (root.TryGetProperty("error_code", out var errorCode))
        {
           string code = errorCode.ToString();
           if(code != "52000" && code != "0") 
           {
                // 如果存在错误码且不是成功状态
                if (root.TryGetProperty("error_msg", out var errorMsg))
                    return $"Baidu Error: {errorMsg.GetString()} ({code})";
                return $"Baidu Error Code: {code}";
           }
        }

        // 解析翻译结果
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

    // 计算MD5值
    private static string MD5Encrypt(string str)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        // 将字符串转换成字节数组
        var byteOld = System.Text.Encoding.UTF8.GetBytes(str);
        // 调用加密方法
        var byteNew = md5.ComputeHash(byteOld);
        // 将加密结果转换为字符串
        var sb = new StringBuilder();
        foreach (var b in byteNew)
        {
            // 将字节转换成16进制表示的字符串
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }    

    private static async Task<string> TranslateWithGoogle(string text, TranslationSettings settings)
    {
        // 简化的 Google 翻译占位实现
        return $"[Google] 翻译结果: {text} (请配置有效的 Google API 处理逻辑)";
    }
}
