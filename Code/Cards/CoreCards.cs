using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Powers;

namespace MyFirstMod.Code.Cards;

static class GeneratedTokenHelper
{
    public static async Task AddGunsparkToHand(MyFirstModCardModel source)
    {
        await AddGunsparksToHand(source, 1);
    }

    public static async Task AddGunsparksToHand(MyFirstModCardModel source, int count)
    {
        await AddGunsparksToPile(source, count, PileType.Hand);
    }

    public static async Task AddGunsparksToPile(MyFirstModCardModel source, int count, PileType pileType)
    {
        if (count <= 0)
            return;

        var owner = source.Owner;
        var combatState = owner?.Creature?.CombatState;
        if (owner == null || combatState == null)
            return;

        if (!CombatGuards.HasLivingEnemy(combatState))
            return;

        for (int i = 0; i < count; i++)
        {
            CardModel spark = combatState.CreateCard<Gunspark>(owner);
            await CardPileCmd.AddGeneratedCardToCombat(spark, pileType, addedByPlayer: true);
        }
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class GunslingerRush : RapidFireCardModel
{

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new BlockVar(2, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Gunslinger Rush"), ("description", "Deal {Damage:diff()} damage. Gain {Block:diff()} Block.")];
    public GunslingerRush() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Block.UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class TacticalSidestep : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Tactical Sidestep"), ("description", "Gain {Block:diff()} Block. Add {Cards:diff()} Gunspark to your hand.")];
    public TacticalSidestep() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparksToHand(this, DynamicVars.Cards.IntValue);
    }
    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class ChainReaction : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Chain Reaction"), ("description", "Deal {Damage:diff()} damage. This turn, whenever you play an Attack, deal {Cards:diff()} extra damage to its target.")];
    public ChainReaction() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }

        await PowerCmd.Apply<ChainReactionPower>(Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class EmergencyShield : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Emergency Shield"), ("description", "Gain {Block:diff()} Block. Draw {Cards:diff()} card.")];
    public EmergencyShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class RapidStance : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new BlockVar(4, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Rapid Stance"), ("description", "Draw {Cards:diff()} card. Gain {Block:diff()} Block.")];
    public RapidStance() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class BarrageFire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Barrage Fire"), ("description", "Deal {Damage:diff()} damage to all enemies.")];
    public BarrageFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (Owner.Creature?.CombatState != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState).Execute(c);

        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class SuppressiveFire : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new BlockVar(6, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Suppressive Fire"), ("description", "Deal {Damage:diff()} damage. Gain {Block:diff()} Block.")];
    public SuppressiveFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class WarfarinsPlasma : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Warfarin's Special"), ("description", "Lose [red]3[/red] HP. Draw {Cards:diff()} cards.")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public WarfarinsPlasma() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.Damage(c, Owner.Creature, 3, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);

        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class QuickMagazine : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new BlockVar(3, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Quick Magazine"), ("description", "Draw {Cards:diff()} card. Gain {Block:diff()} Block. Add 1 Gunspark to your hand.")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public QuickMagazine() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }
    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class SweepMode : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Sweep Mode"), ("description", "Whenever you play an Attack, deal {Damage:diff()} damage to ALL enemies.")];
    public SweepMode() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<SweepModePower>(Owner.Creature, (int)DynamicVars.Damage.BaseValue, Owner.Creature, this);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class PiercingRound : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Piercing Round"), ("description", "Deal {Damage:diff()} damage.")];
    public PiercingRound() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5);
}

[Pool(typeof(ExusiaiCardPool))]
public class PursuitOrder : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "Pursuit Order"), ("description", "Deal {Damage:diff()} damage. Add {Cards:diff()} Gunspark to your hand.")];
    public PursuitOrder() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
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
public class FullAuto : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new CardsVar(3)];
    public override List<(string, string)> Localization => [("title", "Full Auto"), ("description", "Deal {Damage:diff()} damage {Cards:diff()} times.")];
    public FullAuto() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class BulletHell : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "Bullet Hell"), ("description", "Deal {Damage:diff()} damage. Add {Cards:diff()} Gunsparks to your hand.")];
    public BulletHell() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        await GeneratedTokenHelper.AddGunsparksToHand(this, DynamicVars.Cards.IntValue);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

[Pool(typeof(TokenCardPool))]
public class Gunspark : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "Gunspark"), ("description", "Deal {Damage:diff()} damage.")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    public Gunspark() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, false) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            int bonusDamage = Owner.Creature.GetPower<IgnitionProtocolPower>()?.Amount ?? 0;
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonusDamage).FromCard(this).Targeting(p.Target).Execute(c);
        }
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}
