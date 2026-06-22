using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MyFirstMod.Code;

namespace MyFirstMod.Code.Powers;

public class AngelsBlessingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/AngelsBlessingPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/AngelsBlessingPower.png";
    public override List<(string, string)> Localization => [("title", "Angel's Blessing"), ("description", "Play [blue]{Amount}[/blue] more cards to draw 1 card."), ("smartDescription", "Play [blue]{Amount}[/blue] more cards to draw 1 card.")];

    private int _triggerThreshold;

    [SavedProperty]
    public int TriggerThreshold
    {
        get => _triggerThreshold;
        set
        {
            AssertMutable();
            _triggerThreshold = value;
        }
    }

    public override Task AfterApplied(Creature? source, CardModel? card)
    {
        if (TriggerThreshold <= 0)
            TriggerThreshold = Math.Max(1, Amount);

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null) return;
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (Amount <= 0) return;

        Amount--;
        if (Amount > 0) return;

        Flash();
        ResetCountdown();

        if (Owner.Player == null)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }

    private void ResetCountdown()
    {
        if (TriggerThreshold <= 0)
            TriggerThreshold = Math.Max(1, Amount);

        Amount = TriggerThreshold;
    }
}
