using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MyFirstMod.Code.Powers;

public class OverclockPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/cards/Overclock.jpg";
    public override string CustomBigIconPath => "res://myfirstmod/images/cards/Overclock.jpg";
    public override List<(string, string)> Localization => [
        ("title", "过载模式"),
        ("description", "你接下来打出的[blue]{Amount}[/blue]张攻击牌费用变为[blue]0[/blue]。回合结束时移除。"),
        ("smartDescription", "你接下来打出的[blue]{Amount}[/blue]张攻击牌费用变为[blue]0[/blue]。回合结束时移除。")
    ];

    public override bool TryModifyStarCost(CardModel card, decimal currentCost, out decimal modifiedCost)
    {
        if (IsAffectedAttack(card))
        {
            modifiedCost = 0;
            return true;
        }

        modifiedCost = currentCost;
        return false;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal currentCost, out decimal modifiedCost)
    {
        if (IsAffectedAttack(card))
        {
            modifiedCost = 0;
            return true;
        }

        modifiedCost = currentCost;
        return false;
    }

    public override Task AfterApplied(Creature source, CardModel card)
    {
        RefreshHandAttackCosts();
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (IsAffectedAttack(cardPlay.Card))
        {
            cardPlay.Card.SetToFreeThisTurn();
            cardPlay.Card.SetStarCostUntilPlayed(0);
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!IsAffectedAttack(cardPlay.Card))
            return;

        await PowerCmd.Decrement(this);
        RefreshHandAttackCosts();
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
            await PowerCmd.Remove(this);
    }

    public override Task AfterRemoved(Creature source)
    {
        RefreshHandAttackCosts();
        return Task.CompletedTask;
    }

    private bool IsAffectedAttack(CardModel? card)
    {
        return Amount > 0
            && card != null
            && card.Owner == Owner.Player
            && card.Type == CardType.Attack;
    }

    private void RefreshHandAttackCosts()
    {
        var player = Owner.Player;
        if (player == null)
            return;

        foreach (var pile in player.Piles)
        {
            if (pile.Type != PileType.Hand)
                continue;

            foreach (var card in pile.Cards)
            {
                if (card.Type == CardType.Attack)
                    card.InvokeEnergyCostChanged();
            }
        }
    }
}
