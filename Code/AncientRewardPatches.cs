using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Saves.Runs;
using MyFirstMod.Code.Cards;
using MyFirstMod.Code.Characters;

namespace MyFirstMod.Code;

[HarmonyPatch(typeof(ArchaicTooth))]
internal static class ExusiaiArchaicToothPatch
{
    [HarmonyPatch(nameof(ArchaicTooth.SetupForPlayer))]
    [HarmonyPostfix]
    private static void SetupForPlayerPostfix(ArchaicTooth __instance, Player player, ref bool __result)
    {
        if (__result || player.Character is not Exusiai)
            return;

        CardModel? starter = FindTranscendenceStarter(player);
        if (starter == null)
            return;

        __instance.SetupForTests(starter.ToSerializable(), CreateAncientReplacement(player, starter).ToSerializable());
        __result = true;
    }

    [HarmonyPatch(nameof(ArchaicTooth.AfterObtained))]
    [HarmonyPrefix]
    private static bool AfterObtainedPrefix(ArchaicTooth __instance, ref Task __result)
    {
        if (__instance.Owner?.Character is not Exusiai)
            return true;

        __result = TransformExusiaiStarter(__instance.Owner);
        return false;
    }

    private static async Task TransformExusiaiStarter(Player player)
    {
        CardModel? starter = FindTranscendenceStarter(player);
        if (starter == null)
            return;

        await CardCmd.Transform(starter, CreateAncientReplacement(player, starter));
    }

    private static CardModel? FindTranscendenceStarter(Player player)
    {
        return player.Deck.Cards.FirstOrDefault(card => card is CardTemplate);
    }

    private static CardModel CreateAncientReplacement(Player player, CardModel starter)
    {
        CardModel replacement = player.RunState.CreateCard<SanctifiedCrossfire>(player);

        if (starter.IsUpgraded)
            CardCmd.Upgrade(replacement);

        if (starter.Enchantment != null)
        {
            EnchantmentModel enchantment = (EnchantmentModel)starter.Enchantment.MutableClone();
            CardCmd.Enchant(enchantment, replacement, enchantment.Amount);
        }

        return replacement;
    }
}
