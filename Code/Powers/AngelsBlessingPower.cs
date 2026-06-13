using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;
using MyFirstMod.Code;

namespace MyFirstMod.Code.Powers;

public class AngelsBlessingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override bool IsInstanced => true;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/AngelsBlessingPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/AngelsBlessingPower.png";
    public override List<(string, string)> Localization => [("title", "Angel's Blessing"), ("description", "Whenever you play [blue]{Amount}[/blue] cards, draw 1 card."), ("smartDescription", "Whenever you play [blue]{Amount}[/blue] cards, draw 1 card.")];

    private int _cardsPlayedThisTurn;

    [SavedProperty]
    public int CardsPlayedThisTurn
    {
        get => _cardsPlayedThisTurn;
        set
        {
            AssertMutable();
            _cardsPlayedThisTurn = value;
        }
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.Player == player)
            CardsPlayedThisTurn = 0;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null) return;
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (Amount <= 0) return;

        CardsPlayedThisTurn++;
        if (CardsPlayedThisTurn % Amount != 0) return;

        if (Owner.Player == null)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}
