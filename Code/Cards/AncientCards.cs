using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiSpecialCardPool))]
public class SanctifiedCrossfire : RapidFireCardModel
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<ExusiaiCardPool>();

    public override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(16, ValueProp.Move),
        new PowerVar<VulnerablePower>(2),
        new PowerVar<WeakPower>(1)
    ];

    public override List<(string, string)> Localization => [
        ("title", "New Covenant Crossfire"),
        ("description", "Deal {Damage:diff()} damage. Apply {VulnerablePower:diff()} Vulnerable and {WeakPower:diff()} Weak. Add 1 Gunspark.")
    ];

    public SanctifiedCrossfire() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);

            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["WeakPower"].IntValue, Owner.Creature, this);
        }

        await GeneratedTokenHelper.AddGunsparkToHand(this);
        await TryGenerateRapidFireCopy(choiceContext, cardPlay);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiSpecialCardPool))]
public class PenguinLogisticsParcel : MyFirstModCardModel
{
    private const int BasePreviewCount = 3;
    private const int UpgradedPreviewCount = 5;
    private const int PickCount = 3;

    public override CardPoolModel VisualCardPool => ModelDb.CardPool<ExusiaiCardPool>();

    public override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Preview", BasePreviewCount),
        new CardsVar(PickCount)
    ];

    public override List<(string, string)> Localization => [
        ("title", "Penguin Logistics Parcel"),
        ("description", "Choose {Cards} of {Preview:diff()} random Rare cards from any character. Add them to your hand; they cost 0 this turn."),
        ("select", "Choose [blue]{MaxCount}[/blue] cards from the parcel.")
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public PenguinLogisticsParcel() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner?.Creature?.CombatState;
        if (Owner == null || combatState == null)
            return;

        List<CardModel> candidates = GetRareParcelCandidates()
            .StableShuffle(Owner.RunState.Rng.CombatCardSelection)
            .Take(DynamicVars["Preview"].IntValue)
            .ToList();

        if (candidates.Count == 0)
            return;

        List<CardModel> selected = candidates;
        if (candidates.Count > DynamicVars.Cards.IntValue)
        {
            selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                candidates,
                Owner,
                new CardSelectorPrefs(new LocString("cards", "MYFIRSTMOD-PENGUIN_LOGISTICS_PARCEL.select"), DynamicVars.Cards.IntValue, DynamicVars.Cards.IntValue))).ToList();
        }

        foreach (CardModel candidate in selected)
        {
            CardModel parcelCard = combatState.CreateCard(candidate, Owner);
            parcelCard.SetToFreeThisTurn();
            parcelCard.SetStarCostUntilPlayed(0);
            await CardPileCmd.AddGeneratedCardToCombat(parcelCard, PileType.Hand, addedByPlayer: true);
            parcelCard.InvokeEnergyCostChanged();
        }
    }

    public override void OnUpgrade()
    {
        DynamicVars["Preview"].UpgradeValueBy(UpgradedPreviewCount - BasePreviewCount);
    }

    private static List<CardModel> GetRareParcelCandidates()
    {
        return GetParcelPools()
            .SelectMany(pool => pool.GenerateAllCards())
            .Where(card => card.Rarity == CardRarity.Rare)
            .Where(card => card.ShouldShowInCardLibrary)
            .GroupBy(card => card.Id)
            .Select(group => group.First())
            .ToList();
    }

    private static IEnumerable<CardPoolModel> GetParcelPools()
    {
        yield return ModelDb.CardPool<IroncladCardPool>();
        yield return ModelDb.CardPool<SilentCardPool>();
        yield return ModelDb.CardPool<DefectCardPool>();
        yield return ModelDb.CardPool<ExusiaiCardPool>();
    }
}
