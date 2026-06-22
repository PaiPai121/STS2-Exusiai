using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Powers;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class PointBlankShot : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Point-Blank Shot"), ("description", "Deal {Damage:diff()} damage. Add {Cards:diff()} Gunspark to your hand.")];
    public PointBlankShot() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await GeneratedTokenHelper.AddGunsparksToHand(this, DynamicVars.Cards.IntValue);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class CoverReload : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Cover Reload"), ("description", "Gain {Block:diff()} Block. Add {Cards:diff()} Gunspark to your discard pile.")];
    public CoverReload() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparksToPile(this, DynamicVars.Cards.IntValue, PileType.Discard);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class InterleavedFire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Interleaved Fire"), ("description", "Deal {Damage:diff()} damage.")];
    public InterleavedFire() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkCircuit : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new CardsVar(3)];
    public override List<(string, string)> Localization => [("title", "Spark Circuit"), ("description", "Gain {Block:diff()} Block. This combat, every {Cards:diff()} Gunsparks you play draws 1 card.")];
    public SparkCircuit() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<SparkCircuitPower>(c, Owner.Creature, 1, Owner.Creature, this);
    }

    public override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[Pool(typeof(ExusiaiCardPool))]
public class IgnitionProtocol : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Unpowered), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Ignition Protocol"), ("description", "Gunsparks deal {Damage:diff()} additional damage this combat. Every {Cards:diff()} Gunsparks, increase it by 1.")];
    public IgnitionProtocol() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<IgnitionProtocolPower>(c, Owner.Creature, (int)DynamicVars.Damage.BaseValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class BreakthroughVector : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Breakthrough Vector"), ("description", "Deal {Damage:diff()} damage. Add {Cards:diff()} Gunspark to your hand.")];
    public BreakthroughVector() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await GeneratedTokenHelper.AddGunsparksToHand(this, DynamicVars.Cards.IntValue);
        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class HaloCover : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Halo Cover"), ("description", "Gain {Block:diff()} Block. Add {Cards:diff()} Gunspark to your hand.")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public HaloCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparksToHand(this, DynamicVars.Cards.IntValue);
    }

    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class FireControl : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Fire Control"), ("description", "Gain {Block:diff()} Block. At the start of each turn, add 1 Gunspark to your hand.")];
    public FireControl() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<FireControlPower>(c, Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

[Pool(typeof(ExusiaiCardPool))]
public class AngelicReload : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [
        ("title", "Angelic Reload"),
        ("description", "Gain {Block:diff()} Block. Return up to {Cards:diff()} Attacks from your discard pile to your hand. They cost 0 this turn."),
        ("select", "Choose up to [blue]{MaxCount}[/blue] Attacks to reload.")
    ];
    public AngelicReload() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);

        List<CardModel> candidates = PileType.Discard.GetPile(Owner).Cards
            .Where(card => card.Type == CardType.Attack)
            .ToList();

        if (candidates.Count == 0)
            return;

        List<CardModel> selected = (await CardSelectCmd.FromSimpleGrid(
            c,
            candidates,
            Owner,
            new CardSelectorPrefs(new LocString("cards", "MYFIRSTMOD-ANGELIC_RELOAD.select"), 0, DynamicVars.Cards.IntValue))).ToList();

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
public class TerminalVolley : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(24, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Terminal Volley"), ("description", "Deal {Damage:diff()} damage to all enemies. Add {Cards:diff()} Gunsparks to your hand.")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public TerminalVolley() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (Owner.Creature?.CombatState != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState).Execute(c);

        await GeneratedTokenHelper.AddGunsparksToHand(this, DynamicVars.Cards.IntValue);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
    }
}
