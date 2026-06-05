using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class SparkCircuitPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/SparkCircuitPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/SparkCircuitPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Spark Circuit"),
        ("description", "Whenever enough Gunsparks are played, draw [blue]1[/blue] card for each Spark Circuit. Upgraded Spark Circuits also add [blue]1[/blue] Gunspark to your hand."),
        ("smartDescription", "Whenever enough Gunsparks are played, draw [blue]1[/blue] card for each Spark Circuit. Upgraded Spark Circuits also add [blue]1[/blue] Gunspark to your hand.")
    ];

    private int _requiredGunsparks = 3;
    private int _circuitCount;
    private int _generatedSparksOnTrigger;
    private int _gunsparksPlayed;

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this || amount <= 0 || cardSource is not SparkCircuit)
            return Task.CompletedTask;

        _requiredGunsparks = Math.Min(_requiredGunsparks, (int)amount);
        _circuitCount++;
        if (amount <= 2)
            _generatedSparksOnTrigger++;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Gunspark gunspark)
            return;

        if (cardPlay.Card.Owner != Owner.Player)
            return;

        _gunsparksPlayed++;
        if (_gunsparksPlayed < _requiredGunsparks)
            return;

        _gunsparksPlayed = 0;
        if (Owner.Player == null)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        int triggerCount = Math.Max(1, _circuitCount);
        await CardPileCmd.Draw(choiceContext, triggerCount, Owner.Player);
        await GeneratedTokenHelper.AddGunsparksToHand(gunspark, _generatedSparksOnTrigger);
    }
}
