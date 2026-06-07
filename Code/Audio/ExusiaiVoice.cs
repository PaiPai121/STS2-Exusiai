using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Rooms;

namespace MyFirstMod.Code.Audio;

internal static class ExusiaiVoice
{
    private static readonly string[] PowerVoicePaths =
    [
        "res://myfirstmod/audio/voice/skill1.wav",
        "res://myfirstmod/audio/voice/skill2.wav"
    ];

    private const string BattleFailedVoicePath = "res://myfirstmod/audio/voice/battlefailed.wav";
    private const string NormalBattleFinishedVoicePath = "res://myfirstmod/audio/voice/normalbattlefinished.wav";
    private const string PerfectBattleFinishedVoicePath = "res://myfirstmod/audio/voice/perfectbattlefinished.wav";

    private static readonly StringName SfxBus = new("SFX");
    private static readonly Dictionary<string, AudioStream?> StreamCache = [];
    private static readonly Random Rng = new();
    private static bool _resultVoicePlayed;

    public static void ResetCombatVoiceState()
    {
        _resultVoicePlayed = false;
    }

    public static void TryPlayPowerVoice(CardPlay cardPlay)
    {
        if (cardPlay.PlayIndex != 0)
            return;

        if (cardPlay.Card is not MyFirstModCardModel || cardPlay.Card.Type != CardType.Power)
            return;

        string path = PowerVoicePaths[Rng.Next(PowerVoicePaths.Length)];
        PlayVoice(path, "ExusiaiPowerVoice");
    }

    public static void TryPlayVictoryVoice(CombatRoom room)
    {
        if (_resultVoicePlayed)
            return;

        var player = LocalContext.GetMe(room.CombatState);
        var entry = player?.RunState.CurrentMapPointHistoryEntry?.GetEntry(player.NetId);
        bool perfect = entry == null || entry.DamageTaken <= 0;

        _resultVoicePlayed = true;
        PlayVoice(perfect ? PerfectBattleFinishedVoicePath : NormalBattleFinishedVoicePath, "ExusiaiBattleResultVoice");
    }

    public static void TryPlayFailureVoice()
    {
        if (_resultVoicePlayed)
            return;

        _resultVoicePlayed = true;
        PlayVoice(BattleFailedVoicePath, "ExusiaiBattleResultVoice");
    }

    private static void PlayVoice(string path, string playerName)
    {
        AudioStream? stream = GetStream(path);
        if (stream == null || NGame.Instance == null)
            return;

        AudioStreamPlayer player = new()
        {
            Name = playerName,
            Stream = stream,
            Bus = SfxBus,
            VolumeLinear = 1f
        };

        player.Finished += player.QueueFree;
        NGame.Instance.AddChild(player);
        player.Play();
    }

    private static AudioStream? GetStream(string path)
    {
        if (StreamCache.TryGetValue(path, out AudioStream? cached))
            return cached;

        if (!ResourceLoader.Exists(path))
        {
            GD.Print($"[exusiai] voice asset missing: {path}");
            StreamCache[path] = null;
            return null;
        }

        AudioStream? stream = ResourceLoader.Load<AudioStream>(path);
        StreamCache[path] = stream;
        return stream;
    }
}

[HarmonyPatch(typeof(CombatHistory), nameof(CombatHistory.CardPlayFinished))]
internal static class ExusiaiPowerVoicePatch
{
    private static void Postfix(CardPlay cardPlay)
    {
        ExusiaiVoice.TryPlayPowerVoice(cardPlay);
    }
}

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
internal static class ExusiaiCombatVoicePatch
{
    private static void Postfix()
    {
        ExusiaiVoice.ResetCombatVoiceState();
    }
}

[HarmonyPatch(typeof(CombatRoom), nameof(CombatRoom.OnCombatEnded))]
internal static class ExusiaiVictoryVoicePatch
{
    private static void Postfix(CombatRoom __instance)
    {
        ExusiaiVoice.TryPlayVictoryVoice(__instance);
    }
}

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.LoseCombat))]
internal static class ExusiaiFailureVoicePatch
{
    private static void Postfix()
    {
        ExusiaiVoice.TryPlayFailureVoice();
    }
}
