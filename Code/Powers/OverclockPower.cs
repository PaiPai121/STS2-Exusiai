using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class OverclockPower : CustomPowerModel
{
    private const int RechargeInterval = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/OverclockPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/OverclockPower.png";
    public override List<(string, string)> Localization => [
        ("title", "过载模式"),
        ("description", "本回合中，你接下来打出的[blue]{Amount}[/blue]张攻击牌费用变为[blue]0[/blue]。之后每隔1个回合，回合开始时再次获得此效果。"),
        ("smartDescription", "本回合中，你接下来打出的[blue]{Amount}[/blue]张攻击牌费用变为[blue]0[/blue]。之后每隔1个回合，回合开始时再次获得此效果。")
    ];

    private int _freeAttacksPerOverload;
    private int _turnsUntilRecharge = RechargeInterval;

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

    public override Task AfterApplied(Creature? source, CardModel? card)
    {
        if (card is Overclock)
        {
            _freeAttacksPerOverload = card.DynamicVars.Cards.IntValue;
            Amount = _freeAttacksPerOverload;
            _turnsUntilRecharge = RechargeInterval;
        }

        RefreshHandAttackCosts();
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.Player != player)
            return Task.CompletedTask;

        if (_freeAttacksPerOverload <= 0)
            return Task.CompletedTask;

        _turnsUntilRecharge--;
        if (_turnsUntilRecharge > 0)
            return Task.CompletedTask;

        Amount = _freeAttacksPerOverload;
        _turnsUntilRecharge = RechargeInterval;
        Flash();
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

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!IsAffectedAttack(cardPlay.Card))
            return Task.CompletedTask;

        Amount = Math.Max(0, Amount - 1);
        RefreshHandAttackCosts();
        return Task.CompletedTask;
    }

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
        {
            Amount = 0;
            RefreshHandAttackCosts();
        }

        return Task.CompletedTask;
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
