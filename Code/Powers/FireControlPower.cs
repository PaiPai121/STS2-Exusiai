using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code;
using MyFirstMod.Code.Cards;

namespace MyFirstMod.Code.Powers;

public class FireControlPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/FireControlPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/FireControlPower.png";
    public override List<(string, string)> Localization => [
        ("title", "Fire Control"),
        ("description", "At the start of each turn, add [blue]{Amount}[/blue] Gunspark to your hand."),
        ("smartDescription", "At the start of each turn, add [blue]{Amount}[/blue] Gunspark to your hand.")
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.Player != player)
            return;

        var combatState = player.Creature?.CombatState;
        if (combatState == null)
            return;

        if (!CombatGuards.HasLivingEnemy(combatState))
            return;

        for (int i = 0; i < Amount; i++)
        {
            CardModel spark = combatState.CreateCard<Gunspark>(player);
            await CardPileCmd.AddGeneratedCardToCombat(spark, PileType.Hand, player);
        }
    }
}
