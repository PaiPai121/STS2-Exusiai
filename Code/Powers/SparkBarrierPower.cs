using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class SparkBarrierPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/SparkBarrierPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/SparkBarrierPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Spark Barrier"),
        ("description", "Whenever you play a Gunspark, gain [blue]{Amount}[/blue] Block."),
        ("smartDescription", "Whenever you play a Gunspark, gain [blue]{Amount}[/blue] Block.")
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Gunspark)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        await CreatureCmd.GainBlock(Owner, new BlockVar((int)Amount, ValueProp.Unpowered), cardPlay);
    }
}
