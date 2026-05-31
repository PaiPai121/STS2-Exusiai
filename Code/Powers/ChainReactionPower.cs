using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstMod.Code;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class ChainReactionPower : CustomPowerModel
{
    private bool _ignoreSourceCard = true;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/cards/ChainReaction.jpg";
    public override string CustomBigIconPath => "res://myfirstmod/images/cards/ChainReaction.jpg";
    public override List<(string, string)> Localization => [
        ("title", "连锁反应"),
        ("description", "本回合中，每当你打出攻击牌，对其目标追加[blue]{Amount}[/blue]点伤害。"),
        ("smartDescription", "本回合中，每当你打出攻击牌，对其目标追加[blue]{Amount}[/blue]点伤害。")
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        if (_ignoreSourceCard && cardPlay.Card is ChainReaction)
        {
            _ignoreSourceCard = false;
            return;
        }

        _ignoreSourceCard = false;

        if (cardPlay.Target == null || !cardPlay.Target.IsAlive)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await DamageCmd.Attack(Amount).FromCard(cardPlay.Card).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
            await PowerCmd.Remove(this);
    }
}
