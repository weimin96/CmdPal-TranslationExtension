using System;
using System.IO;
using System.Text.Json;

namespace TranslationExtension;

public static class SettingsManager
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TranslationExtension",
        "settings.json"
    );

    private static readonly object _lock = new();
    private static TranslationSettings? _instance;
    private static readonly TranslationSettingsContext _jsonContext = TranslationSettingsContext.Default;

    public static TranslationSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= Load();
                }
            }
            return _instance;
        }
    }

    public static TranslationSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize(json, _jsonContext.TranslationSettings) ?? new TranslationSettings();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载设置失败：{ex.Message}");
        }
        return new TranslationSettings();
    }

    public static void Save(TranslationSettings settings)
    {
        lock (_lock)
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, _jsonContext.TranslationSettings);
            File.WriteAllText(SettingsPath, json);
            _instance = settings;
        }
    }
}
