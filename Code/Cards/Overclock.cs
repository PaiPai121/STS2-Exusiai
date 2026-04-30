using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class Overclock : MyFirstModCardModel
{
    private int _remainingEmpoweredAttacks;
    private bool _overclockActive;

    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;


    public override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    public override List<(string, string)> Localization => [("title", "过载模式"), ("description", "虚无。抽[blue]{Cards}[/blue]张牌。本回合中，你接下来打出的[blue]{Cards}[/blue]张攻击牌打出前费用变为0。")];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    public Overclock() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _remainingEmpoweredAttacks = DynamicVars.Cards.IntValue;
        _overclockActive = true;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (Owner == player)
        {
            _remainingEmpoweredAttacks = 0;
            _overclockActive = false;
        }

        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!_overclockActive)
            return Task.CompletedTask;

        if (_remainingEmpoweredAttacks <= 0)
            return Task.CompletedTask;

        if (cardPlay.Card == null)
            return Task.CompletedTask;

        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card == this)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        cardPlay.Card.SetStarCostUntilPlayed(0);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this)
            return Task.CompletedTask;

        if (!_overclockActive)
            return Task.CompletedTask;

        if (_remainingEmpoweredAttacks <= 0)
            return Task.CompletedTask;

        if (cardPlay.Card?.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        _remainingEmpoweredAttacks--;
        if (_remainingEmpoweredAttacks <= 0)
            _overclockActive = false;

        return Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
