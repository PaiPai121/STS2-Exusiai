using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstMod.Code;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class IgnitionProtocolPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/IgnitionProtocolPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/IgnitionProtocolPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Ignition Protocol"),
        ("description", "Gunsparks deal [red]{Amount}[/red] additional damage. Whenever you play a Gunspark, increase this by [blue]1[/blue]."),
        ("smartDescription", "Gunsparks deal [red]{Amount}[/red] additional damage. Whenever you play a Gunspark, increase this by [blue]1[/blue].")
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Gunspark)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await PowerCmd.Apply<IgnitionProtocolPower>(Owner, 1, Owner, cardPlay.Card);
    }
}
