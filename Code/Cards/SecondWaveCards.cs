using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Keywords;
using MyFirstMod.Code.Powers;

namespace MyFirstMod.Code.Cards;

static class ExusiaiCombatHistory
{
    private static readonly Dictionary<Player, (ICombatState CombatState, int RoundNumber, int Count)> RapidFirePlayedByPlayer = [];
    private static readonly Dictionary<Player, (ICombatState CombatState, int RoundNumber, int Count)> GunsparksPlayedByPlayer = [];

    public static int CardsPlayedThisTurn(MyFirstModCardModel card)
    {
        if (!TryGetRuntimeContext(card, out Player owner, out ICombatState state))
            return 0;

        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(state) &&
            e.CardPlay.Card.Owner == owner);
    }

    public static int GunsparksPlayedThisTurn(MyFirstModCardModel card)
    {
        int trackedCount = TrackedGunsparksPlayedThisTurn(card);
        if (trackedCount > 0)
            return trackedCount;

        if (!TryGetRuntimeContext(card, out Player owner, out ICombatState state))
            return 0;

        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(state) &&
            e.CardPlay.Card.Owner == owner &&
            e.CardPlay.Card is Gunspark);
    }

    public static int RapidFireCardsPlayedThisTurn(MyFirstModCardModel card)
    {
        int trackedCount = TrackedRapidFireCardsPlayedThisTurn(card);
        if (trackedCount > 0)
            return trackedCount;

        if (!TryGetRuntimeContext(card, out Player owner, out ICombatState state))
            return 0;

        // Fallback for older cards already in combat before this tracker existed.
        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(state) &&
            e.CardPlay.Card.Owner == owner &&
            e.CardPlay.Card.Keywords.Contains(MyKeywords.RapidFire));
    }

    public static void RecordRapidFirePlayed(MyFirstModCardModel card)
    {
        RecordThisTurn(card, RapidFirePlayedByPlayer);
    }

    public static void RecordGunsparkPlayed(MyFirstModCardModel card)
    {
        RecordThisTurn(card, GunsparksPlayedByPlayer);
    }

    private static int TrackedRapidFireCardsPlayedThisTurn(MyFirstModCardModel card)
    {
        return TrackedThisTurn(card, RapidFirePlayedByPlayer);
    }

    private static int TrackedGunsparksPlayedThisTurn(MyFirstModCardModel card)
    {
        return TrackedThisTurn(card, GunsparksPlayedByPlayer);
    }

    private static void RecordThisTurn(
        MyFirstModCardModel card,
        Dictionary<Player, (ICombatState CombatState, int RoundNumber, int Count)> tracker)
    {
        if (!TryGetRuntimeContext(card, out Player owner, out ICombatState state))
            return;

        if (tracker.TryGetValue(owner, out var entry) &&
            ReferenceEquals(entry.CombatState, state) &&
            entry.RoundNumber == state.RoundNumber)
        {
            tracker[owner] = (state, state.RoundNumber, entry.Count + 1);
            return;
        }

        tracker[owner] = (state, state.RoundNumber, 1);
    }

    private static int TrackedThisTurn(
        MyFirstModCardModel card,
        Dictionary<Player, (ICombatState CombatState, int RoundNumber, int Count)> tracker)
    {
        if (!TryGetRuntimeContext(card, out Player owner, out ICombatState state))
            return 0;

        if (!tracker.TryGetValue(owner, out var entry))
            return 0;

        if (!ReferenceEquals(entry.CombatState, state) || entry.RoundNumber != state.RoundNumber)
            return 0;

        return entry.Count;
    }

    public static bool HasGunsparkInHand(MyFirstModCardModel card)
    {
        return TryGetRuntimeContext(card, out Player owner, out _) &&
            PileType.Hand.GetPile(owner).Cards.Any(handCard => handCard is Gunspark);
    }

    public static bool AnyLivingEnemyIsVulnerable(MyFirstModCardModel card)
    {
        return TryGetRuntimeContext(card, out _, out ICombatState state) &&
            state.Enemies.Any(enemy =>
            enemy.IsAlive &&
            (enemy.GetPower<VulnerablePower>()?.Amount ?? 0) > 0);
    }

    private static bool TryGetRuntimeContext(MyFirstModCardModel card, out Player owner, out ICombatState state)
    {
        owner = null!;
        state = null!;

        if (!card.IsMutable)
            return false;

        Player? cardOwner = card.Owner;
        ICombatState? cardState = card.CombatState ?? cardOwner?.Creature?.CombatState;
        if (cardOwner == null || cardState == null)
            return false;

        owner = cardOwner;
        state = cardState;
        return true;
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class TracerRounds : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Tracer Rounds"), ("description", "Deal {Damage:diff()} damage. Apply {Cards:diff()} Vulnerable.")];
    public TracerRounds() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        await PowerCmd.Apply<VulnerablePower>(c, p.Target, DynamicVars.Cards.IntValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class FieldStrip : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Field Strip"), ("description", "Draw {Cards:diff()} cards. Add 1 Gunspark to your hand.")];
    public FieldStrip() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }

    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class CrossfirePattern : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Crossfire Pattern"), ("description", "Deal {Damage:diff()} damage {Cards:diff()} times.")];
    public CrossfirePattern() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class HaloFeint : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Halo Feint"), ("description", "Gain {Block:diff()} Block. If you have a Gunspark in hand, draw {Cards:diff()} card.")];
    public HaloFeint() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.HasGunsparkInHand(this);

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (PileType.Hand.GetPile(Owner).Cards.Any(card => card is Gunspark))
            await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkPrimer : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Spark Primer"), ("description", "Gain {Block:diff()} Block. Add {Cards:diff()} Gunsparks to the top of your draw pile.")];
    public SparkPrimer() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (CombatState == null)
            return;

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            CardModel spark = CombatState.CreateCard<Gunspark>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(spark, PileType.Draw, Owner, CardPilePosition.Top);
        }
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class SuppressionSignal : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Suppression Signal"), ("description", "Gain {Block:diff()} Block. Apply {Cards:diff()} Vulnerable.")];
    public SuppressionSignal() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (p.Target != null)
            await PowerCmd.Apply<VulnerablePower>(c, p.Target, DynamicVars.Cards.IntValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class MarkedAdvance : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new BlockVar(5, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Marked Advance"), ("description", "Deal {Damage:diff()} damage. If the target has Vulnerable, gain {Block:diff()} Block.")];
    public MarkedAdvance() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.AnyLivingEnemyIsVulnerable(this);

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        bool targetIsVulnerable = (p.Target.GetPower<VulnerablePower>()?.Amount ?? 0) > 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        if (targetIsVulnerable)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(3);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class FlashpointMark : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Flashpoint Mark"), ("description", "Deal {Damage:diff()} damage. Apply {Cards:diff()} Vulnerable.")];
    public FlashpointMark() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
            await PowerCmd.Apply<VulnerablePower>(c, p.Target, DynamicVars.Cards.IntValue, Owner.Creature, this);
        }

        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class LockOnOrder : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Lock-On Order"), ("description", "Apply {Cards:diff()} Vulnerable. Add 1 Gunspark to your hand.")];
    public LockOnOrder() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        await PowerCmd.Apply<VulnerablePower>(c, p.Target, DynamicVars.Cards.IntValue, Owner.Creature, this);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }

    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkRecycle : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Spark Recycle"), ("description", "Remove up to {Cards:diff()} cards in your hand from combat. Draw that many cards.")];
    public SparkRecycle() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        List<CardModel> selected = (await CommonActions.SelectCards(
            this,
            new LocString("cards", "MYFIRSTMOD-SPARK_RECYCLE.select"),
            c,
            PileType.Hand,
            0,
            DynamicVars.Cards.IntValue)).ToList();

        await CardPileCmd.RemoveFromCombat(selected);

        await CardPileCmd.Draw(c, selected.Count, Owner);
    }

    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkAegis : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Spark Aegis"), ("description", "Gain {Block:diff()} Block. If you played a Gunspark this turn, gain {Block:diff()} Block again.")];
    public SparkAegis() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.GunsparksPlayedThisTurn(this) > 0;

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (ExusiaiCombatHistory.GunsparksPlayedThisTurn(this) > 0)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class RelayFootwork : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Relay Footwork"), ("description", "Gain {Block:diff()} Block. If you played Rapid Fire this turn, gain [blue]4[/blue] Block.")];
    public RelayFootwork() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.RapidFireCardsPlayedThisTurn(this) > 0;

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (ExusiaiCombatHistory.RapidFireCardsPlayedThisTurn(this) > 0)
            await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(4, ValueProp.Move), p);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class SpottersCover : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(9, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Spotter's Cover"), ("description", "Gain {Block:diff()} Block. If the target has Vulnerable, draw {Cards:diff()} card.")];
    public SpottersCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.AnyLivingEnemyIsVulnerable(this);

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (p.Target != null && (p.Target.GetPower<VulnerablePower>()?.Amount ?? 0) > 0)
            await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class RelayVolley : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Relay Volley"), ("description", "Deal {Damage:diff()} damage to all enemies. Add 1 Gunspark to your discard pile.")];
    public RelayVolley() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (Owner.Creature?.CombatState != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState).Execute(c);

        await GeneratedTokenHelper.AddGunsparksToPile(this, 1, PileType.Discard);
        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkCrossfire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Spark Crossfire"), ("description", "Deal {Damage:diff()} damage. If you played a Gunspark this turn, deal {Damage:diff()} damage again.")];
    public SparkCrossfire() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.GunsparksPlayedThisTurn(this) > 0;

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
            if (ExusiaiCombatHistory.GunsparksPlayedThisTurn(this) > 0)
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }

        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class TempoBurst : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Tempo Burst"), ("description", "Deal {Damage:diff()} damage plus {Cards:diff()} damage for each card you played earlier this turn.")];
    public TempoBurst() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        int bonus = ExusiaiCombatHistory.CardsPlayedThisTurn(this) * DynamicVars.Cards.IntValue;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(p.Target).Execute(c);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class QuickdrawDrill : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Quickdraw Drill"), ("description", "Move up to {Cards:diff()} Rapid Fire cards from your draw pile to your hand. They cost 0.")];
    public QuickdrawDrill() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        List<CardModel> rapidFireCards = PileType.Draw.GetPile(Owner).Cards
            .Where(card => card.Keywords.Contains(MyKeywords.RapidFire))
            .ToList();

        if (rapidFireCards.Count == 0)
            return;

        List<CardModel> selected = (await CardSelectCmd.FromSimpleGrid(
            c,
            rapidFireCards,
            Owner,
            new CardSelectorPrefs(new LocString("cards", "MYFIRSTMOD-QUICKDRAW_DRILL.select"), 0, DynamicVars.Cards.IntValue))).ToList();

        foreach (CardModel card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Top, this);
            card.SetToFreeThisTurn();
            card.SetStarCostUntilPlayed(0);
            card.InvokeEnergyCostChanged();
        }
    }

    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class HaloRelay : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Halo Relay"), ("description", "Gain {Block:diff()} Block. If you played Rapid Fire this turn, draw {Cards:diff()} card.")];
    public HaloRelay() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.RapidFireCardsPlayedThisTurn(this) > 0;

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (ExusiaiCombatHistory.RapidFireCardsPlayedThisTurn(this) > 0)
            await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class VectorReboot : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [
        ("title", "Vector Reboot"),
        ("description", "Gain {Block:diff()} Block. Return up to {Cards:diff()} non-Attacks from your discard pile to your hand. They cost 0. Add 1 Gunspark for each."),
        ("select", "Choose up to [blue]{MaxCount}[/blue] non-Attack cards to reboot.")
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public VectorReboot() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);

        List<CardModel> candidates = PileType.Discard.GetPile(Owner).Cards
            .Where(card => card.Type != CardType.Attack)
            .ToList();

        if (candidates.Count == 0)
            return;

        List<CardModel> selected = (await CardSelectCmd.FromSimpleGrid(
            c,
            candidates,
            Owner,
            new CardSelectorPrefs(new LocString("cards", "MYFIRSTMOD-VECTOR_REBOOT.select"), 0, DynamicVars.Cards.IntValue))).ToList();

        foreach (CardModel card in selected)
        {
            await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Top, this);
            card.SetToFreeThisTurn();
            card.SetStarCostUntilPlayed(0);
            card.InvokeEnergyCostChanged();
        }

        if (selected.Count > 0)
            await GeneratedTokenHelper.AddGunsparksToHand(this, selected.Count);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkBarrier : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2, ValueProp.Unpowered)];
    public override List<(string, string)> Localization => [("title", "Spark Barrier"), ("description", "Whenever you play a Gunspark, gain {Block:diff()} Block.")];
    public SparkBarrier() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<SparkBarrierPower>(c, Owner.Creature, (int)DynamicVars.Block.BaseValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class OpenFireDiscipline : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Unpowered)];
    public override List<(string, string)> Localization => [("title", "Open Fire Discipline"), ("description", "Whenever you play Rapid Fire, deal {Damage:diff()} damage to its target.")];
    public OpenFireDiscipline() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<RapidFireSupportPower>(c, Owner.Creature, (int)DynamicVars.Damage.BaseValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class RhythmTrigger : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Rhythm Trigger"), ("description", "Deal {Damage:diff()} damage. If you played Rapid Fire this turn, draw {Cards:diff()} cards.")];
    public RhythmTrigger() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.RapidFireCardsPlayedThisTurn(this) > 0;

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        if (ExusiaiCombatHistory.RapidFireCardsPlayedThisTurn(this) > 0)
            await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class FinalSalvo : MyFirstModCardModel
{
    private sealed class HitsVar(decimal value) : DynamicVar("Hits", value);

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new CardsVar(3)];
    public override List<(string, string)> Localization => [("title", "Final Salvo"), ("description", "Deal {Damage:diff()} damage [blue]{Hits}[/blue] times. +1 hit for each Gunspark played this turn, up to +3.")];
    public FinalSalvo() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override bool ShouldShowActivationHighlight() => ExusiaiCombatHistory.GunsparksPlayedThisTurn(this) > 0;

    public override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.AddObj("Hits", new HitsVar(CurrentHits()));
    }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        int hits = CurrentHits();
        for (int i = 0; i < hits; i++)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);

    private int CurrentHits()
    {
        return DynamicVars.Cards.IntValue + Math.Min(3, ExusiaiCombatHistory.GunsparksPlayedThisTurn(this));
    }
}
