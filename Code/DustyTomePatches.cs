using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MyFirstMod.Code.Cards;
using MyFirstMod.Code.Characters;

namespace MyFirstMod.Code;

[HarmonyPatch(typeof(DustyTome))]
internal static class ExusiaiDustyTomePatch
{
    [HarmonyPatch(nameof(DustyTome.SetupForPlayer))]
    [HarmonyPrefix]
    private static bool SetupForPlayerPrefix(DustyTome __instance, Player player)
    {
        if (player.Character is not Exusiai)
            return true;

        __instance.AncientCard = ModelDb.Card<PenguinLogisticsParcel>().Id;
        return false;
    }
}
