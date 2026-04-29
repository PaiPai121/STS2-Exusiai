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

    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    public Overclock() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _remainingEmpoweredAttacks = DynamicVars.Cards.IntValue;
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
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
        if (cardPlay.Card != this)
        {
            if (_remainingEmpoweredAttacks > 0 && cardPlay.Card?.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
            {
                _remainingEmpoweredAttacks--;
            }
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
