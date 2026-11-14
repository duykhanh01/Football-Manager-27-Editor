using System.Collections;
using System.Globalization;
using System.Resources;
using Fm27Editor.App;

namespace Fm27Editor.App.Tests;

[CollectionDefinition("Language settings", DisableParallelization = true)]
public sealed class LanguageSettingsCollection { }

[Collection("Language settings")]
public sealed class AppLanguageTests
{
    [Fact]
    public void Resources_translate_by_key_and_fall_back_to_english()
    {
        Assert.Equal("Name", AppLanguage.Get("Label_Name", "en"));
        Assert.Equal("Tên", AppLanguage.Get("Label_Name", "vi"));
        Assert.Equal("Name", AppLanguage.Get("Label_Name", "fr"));
        Assert.Equal("missing_key", AppLanguage.Get("missing_key", "vi"));
    }

    [Fact]
    public void Vietnamese_resource_has_all_english_keys()
    {
        var resources = new ResourceManager("Fm27Editor.App.Resources.Strings", typeof(AppLanguage).Assembly);
        static HashSet<string> Keys(ResourceSet set) => set.Cast<DictionaryEntry>().Select(e => (string)e.Key).ToHashSet();
        using var en = resources.GetResourceSet(CultureInfo.InvariantCulture, true, false)!;
        using var vi = resources.GetResourceSet(CultureInfo.GetCultureInfo("vi"), true, false)!;
        Assert.Equal(Keys(en).OrderBy(x => x), Keys(vi).OrderBy(x => x));
    }

    [Fact]
    public void Saved_language_is_applied_on_next_load()
    {
        var file = Path.Combine(Path.GetTempPath(), "fm27-language-" + Guid.NewGuid().ToString("N"), "language.txt");
        AppLanguage.SettingPathOverride = file;
        try
        {
            AppLanguage.Save("vi");
            AppLanguage.Load();
            Assert.Equal("vi", AppLanguage.Current);
            Assert.Equal("Tên", AppLanguage.T("Name"));
            AppLanguage.Save("en");
            AppLanguage.Load();
            Assert.Equal("Name", AppLanguage.T("Name"));
            Assert.Throws<ArgumentOutOfRangeException>(() => AppLanguage.Save("fr"));
        }
        finally
        {
            AppLanguage.SettingPathOverride = null;
            AppLanguage.Load();
            Directory.Delete(Path.GetDirectoryName(file)!, true);
        }
    }
}
