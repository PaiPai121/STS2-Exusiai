using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstMod.Code;

namespace MyFirstMod.Code.Powers;

public class SweepModePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/SweepModePower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/SweepModePower.png";
    public override List<(string, string)> Localization => [
        ("title", "Sweep Mode"),
        ("description", "Whenever you play an Attack, deal [red]{Amount}[/red] damage to ALL enemies."),
        ("smartDescription", "Whenever you play an Attack, deal [red]{Amount}[/red] damage to ALL enemies.")
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        if (Owner.CombatState == null)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await DamageCmd.Attack(Amount).FromCard(cardPlay.Card).TargetingAllOpponents(Owner.CombatState).Execute(choiceContext);
    }
}
