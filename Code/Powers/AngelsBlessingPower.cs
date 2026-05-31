using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MyFirstMod.Code;

namespace MyFirstMod.Code.Powers;

public class AngelsBlessingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/powers/AngelsBlessingPower.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/powers/AngelsBlessingPower.png";
    public override List<(string, string)> Localization => [("title", "天使祝福"), ("description", "每打出[blue]{Amount}[/blue]张牌，抽1张牌。"), ("smartDescription", "每打出[blue]{Amount}[/blue]张牌，抽1张牌。")];

    private int _cardsPlayedThisTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner.Player == player)
            _cardsPlayedThisTurn = 0;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == null) return;
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (Amount <= 0) return;

        _cardsPlayedThisTurn++;
        if (_cardsPlayedThisTurn % Amount != 0) return;

        if (Owner.Player == null)
            return;

        if (!CombatGuards.HasLivingEnemy(Owner.CombatState))
            return;

        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}
