using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MyFirstMod.Code.Cards;
using MyFirstMod.Code.RelicPools;

namespace MyFirstMod.Code.Relics;

[Pool(typeof(ExusiaiRelicPool))]
public class SniperChipset : MyFirstModRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private bool _usedThisCombat;

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_usedThisCombat)
            return;

        if (cardPlay.Card is not RapidFireCardModel)
            return;

        if (cardPlay.Card.Owner != Owner)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.Creature?.CombatState))
            return;

        var combatState = Owner.Creature?.CombatState;
        if (combatState == null)
            return;

        CardModel spark = combatState.CreateCard<Gunspark>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(spark, PileType.Hand, addedByPlayer: true);

        _usedThisCombat = true;
        Flash();
        Status = RelicStatus.Normal;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _usedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}
