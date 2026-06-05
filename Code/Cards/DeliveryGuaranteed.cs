using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class DeliveryGuaranteed : MyFirstModCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    public override List<(string, string)> Localization => [("title", "Guaranteed Delivery!"), ("description", "Choose up to {Cards:diff()} cards from your discard pile and copy them into your hand. The copies cost 0 this turn and gain Ethereal and Exhaust.")];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public DeliveryGuaranteed() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> selected = (await CommonActions.SelectCards(
            this,
            new LocString("cards", "MYFIRSTMOD-DELIVERY_GUARANTEED.select"),
            choiceContext,
            PileType.Discard,
            0,
            DynamicVars.Cards.IntValue)).ToList();

        List<CardModel> copies = [];
        foreach (CardModel card in selected)
        {
            CardModel copy = card.CreateClone();
            copy.SetToFreeThisTurn();
            copy.SetStarCostUntilPlayed(0);
            copy.AddKeyword(CardKeyword.Ethereal);
            copy.AddKeyword(CardKeyword.Exhaust);
            copies.Add(copy);
        }

        foreach (CardModel copy in copies)
        {
            await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
            copy.InvokeEnergyCostChanged();
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
