using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.Keywords;

namespace MyFirstMod.Code.Cards;

/// <summary>
/// Base class for Rapid Fire attacks. A Rapid Fire card creates one copy when played.
/// The generated copy loses Rapid Fire and gains Ethereal and Exhaust to prevent loops.
/// Subclasses should call TryGenerateRapidFireCopy at the end of OnPlay.
/// </summary>
public abstract class RapidFireCardModel : MyFirstModCardModel
{
    private bool _hasRapidFire = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => _hasRapidFire ? [MyKeywords.RapidFire] : [];

    protected RapidFireCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary, bool autoAdd = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary, autoAdd)
    {
    }

    /// <summary>
    /// Generate one Rapid Fire follow-up copy, unless this card is already a generated copy
    /// or combat is ending.
    /// </summary>
    protected async Task TryGenerateRapidFireCopy(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Only original Rapid Fire cards generate a copy.
        if (!_hasRapidFire)
            return;

        ExusiaiCombatHistory.RecordRapidFirePlayed(this);

        // Do not generate follow-up cards when combat is ending.
        var combatState = CombatState;
        if (combatState != null)
        {
            bool allEnemiesDead = true;
            foreach (var enemy in combatState.Enemies)
            {
                if (enemy.IsAlive)
                {
                    allEnemiesDead = false;
                    break;
                }
            }
            if (allEnemiesDead)
                return;
        }

        // Clone the current card, matching relic-style card copies.
        CardModel copy = CreateClone();

        // The copy cannot trigger Rapid Fire again.
        if (copy is RapidFireCardModel rapidFireCopy)
        {
            rapidFireCopy._hasRapidFire = false;
        }
        if (copy.Keywords.Contains(MyKeywords.RapidFire))
        {
            copy.RemoveKeyword(MyKeywords.RapidFire);
        }

        // The copy leaves hand at end of turn or after being played.
        copy.AddKeyword(CardKeyword.Ethereal);
        copy.AddKeyword(CardKeyword.Exhaust);

        // Add the generated copy to hand.
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
    }
}
