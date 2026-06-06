using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MyFirstMod.Code.Powers;

public class WarfarinsPlasmaPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/cards/WarfarinsPlasma.jpg";
    public override string CustomBigIconPath => "res://myfirstmod/images/cards/WarfarinsPlasma.jpg";
    public override List<(string, string)> Localization => [
        ("title", "Warfarin's Special"),
        ("description", "Gain [blue]{Amount}[/blue] Strength this turn."),
        ("smartDescription", "Gain [blue]{Amount}[/blue] Strength this turn.")
    ];

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await PowerCmd.Apply<StrengthPower>(target, amount, applier, cardSource, silent: true);
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount != Amount)
            await PowerCmd.Apply<StrengthPower>(Owner, amount, applier, cardSource, silent: true);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side)
            return;

        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<StrengthPower>(Owner, -Amount, Owner, null);
    }
}
