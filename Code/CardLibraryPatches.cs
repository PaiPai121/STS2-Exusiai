using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Characters;

namespace MyFirstMod.Code;

[HarmonyPatch(typeof(NCardLibrary))]
public static class CardLibraryPatches
{
    [HarmonyPostfix]
    [HarmonyAfter(["BaseLib"])]
    [HarmonyPatch("_Ready")]
    private static void IncludeVisualExusiaiCardsInExusiaiFilter(
        Dictionary<NCardPoolFilter, Func<CardModel, bool>> ____poolFilters,
        Dictionary<CharacterModel, NCardPoolFilter> ____cardPoolFilters)
    {
        if (!____cardPoolFilters.TryGetValue(ModelDb.Character<Exusiai>(), out NCardPoolFilter? exusiaiFilter))
            return;

        if (!____poolFilters.TryGetValue(exusiaiFilter, out Func<CardModel, bool>? originalFilter))
            return;

        CardPoolModel exusiaiPool = ModelDb.CardPool<ExusiaiCardPool>();
        ____poolFilters[exusiaiFilter] = card => originalFilter(card) || card.VisualCardPool == exusiaiPool;
    }

    [HarmonyPrefix]
    [HarmonyPatch("UpdateCardPoolFilter")]
    private static void KeepFocusOnSelectedPoolFilter(NCardPoolFilter filter, ref Godot.Control? ____lastHoveredControl)
    {
        ____lastHoveredControl = filter;
    }
}
