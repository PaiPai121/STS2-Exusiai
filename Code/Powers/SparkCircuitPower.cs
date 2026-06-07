using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstMod.Code;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class SparkCircuitPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/SparkCircuitPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/SparkCircuitPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Spark Circuit"),
        ("description", "After every [blue]3[/blue] Gunsparks you play, draw [blue]{Amount}[/blue] card(s)."),
        ("smartDescription", "After every [blue]3[/blue] Gunsparks you play, draw [blue]{Amount}[/blue] card(s).")
    ];

    private int _gunsparksPlayed;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Gunspark)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        _gunsparksPlayed++;
        if (_gunsparksPlayed < 3)
            return;

        _gunsparksPlayed -= 3;
        if (Owner.Player == null)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}
