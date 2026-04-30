using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class AngelsBlessing : MyFirstModCardModel
{
    private int _cardsPlayedThisTurn;
    private bool _blessingActive;

    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;


    public override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    public override List<(string, string)> Localization => [("title", "天使祝福"), ("description", "每回合中，你每打出5张牌，抽[blue]{Cards}[/blue]张牌。打出时先抽[blue]{Cards}[/blue]张牌。")];

    public AngelsBlessing() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _cardsPlayedThisTurn = 0;
        _blessingActive = true;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (Owner == player)
        {
            _cardsPlayedThisTurn = 0;
            _blessingActive = false;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_blessingActive)
            return;

        if (cardPlay.Card == null)
            return;

        if (Owner == null || cardPlay.Card.Owner != Owner)
            return;

        if (cardPlay.Card == this)
            return;

        _cardsPlayedThisTurn++;
        if (_cardsPlayedThisTurn % 5 != 0)
            return;

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
