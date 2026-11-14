namespace Fm27Editor.App;

/// <summary>Language is loaded before the UI is built. Untranslated strings fall back to English.</summary>
internal static class AppLanguage
{
    internal static string? SettingPathOverride { get; set; }
    static string SettingPath => SettingPathOverride ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fm27Editor", "language.txt");
    public static string Current { get; private set; } = "en";
    public static void Load()
    {
        Current = "en";
        try { if (File.Exists(SettingPath) && File.ReadAllText(SettingPath).Trim() == "vi") Current = "vi"; }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
    }
    public static void Save(string language)
    {
        if (language is not ("en" or "vi")) throw new ArgumentOutOfRangeException(nameof(language));
        Directory.CreateDirectory(Path.GetDirectoryName(SettingPath)!);
        File.WriteAllText(SettingPath, language);
    }
    public static string T(string text) => Current == "vi" && VietnameseTranslations.Entries.TryGetValue(text, out var translated) ? translated : text;
}
