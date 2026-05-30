using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MyFirstMod.Code.Cards;
using MyFirstMod.Code.RelicPools;

namespace MyFirstMod.Code.Relics;

[Pool(typeof(ExusiaiRelicPool))]
public class SparkCapacitor : MyFirstModRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private bool _usedThisCombat;

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (_usedThisCombat)
            return;

        if (Owner != player)
            return;

        var combatState = player.Creature?.CombatState;
        if (combatState == null)
            return;

        CardModel spark = combatState.CreateCard<Gunspark>(player);
        await CardPileCmd.AddGeneratedCardToCombat(spark, PileType.Hand, addedByPlayer: true);
        _usedThisCombat = true;
        Flash();
        Status = RelicStatus.Normal;
    }
}
