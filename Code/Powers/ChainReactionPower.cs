using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class ChainReactionPower : CustomPowerModel
{
    private bool _ignoreSourceCard = true;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/ChainReactionPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/ChainReactionPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Chain Reaction"),
        ("description", "This turn, your Attacks deal [blue]{Amount}[/blue] extra damage to their target."),
        ("smartDescription", "This turn, your Attacks deal [blue]{Amount}[/blue] extra damage to their target.")
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

        await CreatureCmd.Damage(choiceContext, cardPlay.Target, Amount, ValueProp.Move | ValueProp.Unpowered, Owner, cardPlay.Card);
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
            await PowerCmd.Remove(this);
    }
}
