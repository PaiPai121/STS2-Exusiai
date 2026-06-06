using System.Reflection;
using System.Text.Json;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace MyFirstMod.Code.Localization;

[HarmonyPatch]
static class ExusiaiLocalizationPatch
{
    private static readonly string[] Tables = [
        "cards",
        "relics",
        "characters",
        "ancients",
        "card_keywords"
    ];

    private static readonly FieldInfo LocDictionaryField = AccessTools.Field(typeof(LocTable), "_translations");
    private static bool _localeCallbackRegistered;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
    private static void AfterModelDbInit()
    {
        RegisterLocaleCallback();
        ReloadCurrentLanguage();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocManager), nameof(LocManager.SetLanguage))]
    private static void AfterSetLanguage()
    {
        ReloadCurrentLanguage();
    }

    private static void RegisterLocaleCallback()
    {
        if (_localeCallbackRegistered || LocManager.Instance == null)
            return;

        LocManager.Instance.SubscribeToLocaleChange(ReloadCurrentLanguage);
        _localeCallbackRegistered = true;
    }

    private static void ReloadCurrentLanguage()
    {
        var locManager = LocManager.Instance;
        if (locManager == null)
            return;

        foreach (string tableName in Tables)
            MergeTable(locManager, ResolveLanguageDirectory(locManager.Language), tableName);
    }

    private static string ResolveLanguageDirectory(string language)
    {
        string normalized = language.Replace('-', '_').ToLowerInvariant();

        if (normalized is "zhs" or "zh" or "zh_cn" or "zh_hans" or "zh_chs" or "chinese" or "simplified_chinese")
            return "zhs";

        if (normalized.StartsWith("zh_"))
            return "zhs";

        return "eng";
    }

    private static void MergeTable(LocManager locManager, string language, string tableName)
    {
        MergeTableFromPath(locManager, $"res://exusiai/localization/{language}/{tableName}.json", tableName);
        MergeTableFromPath(locManager, $"res://myfirstmod/localization/{language}/{tableName}.json", tableName);
    }

    private static void MergeTableFromPath(LocManager locManager, string path, string tableName)
    {
        if (!Godot.FileAccess.FileExists(path))
            return;

        using Godot.FileAccess file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
            return;

        var values = JsonSerializer.Deserialize<Dictionary<string, string>>(file.GetAsText());
        if (values == null)
            return;

        LocTable table = locManager.GetTable(tableName);
        if (LocDictionaryField.GetValue(table) is not Dictionary<string, string> translations)
            return;

        foreach (var pair in values)
            translations[pair.Key] = pair.Value;
    }
}
