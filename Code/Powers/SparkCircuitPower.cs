using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class SparkCircuitPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/SparkCircuitPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/SparkCircuitPower.png";
    public override List<(string, string)> Localization => [
        ("title", "火花回路"),
        ("description", "每打出[blue]3[/blue]张枪火火花，抽[blue]1[/blue]张牌。"),
        ("smartDescription", "每打出[blue]3[/blue]张枪火火花，抽[blue]1[/blue]张牌。")
    ];

    private const int RequiredGunsparks = 3;
    private int _gunsparksPlayed;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Gunspark)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        _gunsparksPlayed++;
        if (_gunsparksPlayed < RequiredGunsparks)
            return;

        _gunsparksPlayed = 0;
        if (Owner.Player != null)
            await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}
