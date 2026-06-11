using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MyFirstMod.Code.Characters;

namespace MyFirstMod.Code;

[HarmonyPatch(typeof(TouchOfOrobas))]
internal static class ExusiaiTouchOfOrobasPatch
{
    [HarmonyPatch(nameof(TouchOfOrobas.SetupForPlayer))]
    [HarmonyPrefix]
    private static bool SetupForPlayerPrefix(Player player, ref bool __result)
    {
        if (player.Character is not Exusiai)
            return true;

        __result = false;
        return false;
    }

    [HarmonyPatch(nameof(TouchOfOrobas.AfterObtained))]
    [HarmonyPrefix]
    private static bool AfterObtainedPrefix(TouchOfOrobas __instance, ref Task __result)
    {
        if (__instance.Owner?.Character is not Exusiai)
            return true;

        __result = Task.CompletedTask;
        return false;
    }
}
