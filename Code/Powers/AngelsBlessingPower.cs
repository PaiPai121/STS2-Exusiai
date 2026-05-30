using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MyFirstMod.Code.Powers;

public class AngelsBlessingPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => "res://myfirstmod/images/exusiai/character_icon_exusiai.png";
    public override string CustomBigIconPath => "res://myfirstmod/images/exusiai/character_icon_exusiai.png";
    public override List<(string, string)> Localization => [("title", "天使祝福"), ("description", "每打出5张牌，抽[blue]{Amount}[/blue]张牌。"), ("smartDescription", "每打出5张牌，抽[blue]{Amount}[/blue]张牌。")];

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

        _cardsPlayedThisTurn++;
        if (_cardsPlayedThisTurn % 5 != 0) return;

        if (Owner.Player == null)
            return;

        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}
