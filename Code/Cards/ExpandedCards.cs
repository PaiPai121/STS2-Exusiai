using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Powers;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class PointBlankShot : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "贴身点射"), ("description", "造成[red]{Damage}[/red]点伤害。")];
    public PointBlankShot() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class CoverReload : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "掩护换弹"), ("description", "获得[green]{Block}[/green]点格挡。将1张枪火火花加入手牌。")];
    public CoverReload() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class InterleavedFire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new BlockVar(5, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "交错射击"), ("description", "造成[red]{Damage}[/red]点伤害。获得[green]{Block}[/green]点格挡。速射。")];
    public InterleavedFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class SparkCircuit : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "火花回路"), ("description", "获得[green]{Block}[/green]点格挡。每打出3张枪火火花，抽1张牌。")];
    public SparkCircuit() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<SparkCircuitPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class BreakthroughVector : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "突破向量"), ("description", "造成[red]{Damage}[/red]点伤害。将1张枪火火花加入手牌。速射。")];
    public BreakthroughVector() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await GeneratedTokenHelper.AddGunsparkToHand(this);
        await TryGenerateRapidFireCopy(c, p);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class HaloCover : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "光环掩护"), ("description", "获得[green]{Block}[/green]点格挡。将1张枪火火花加入手牌。消耗。")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public HaloCover() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class FireControl : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "火控校准"), ("description", "获得[green]{Block}[/green]点格挡。每回合开始时，将1张枪火火花加入手牌。")];
    public FireControl() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<FireControlPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class AngelicReload : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "天使装填"), ("description", "获得[green]{Block}[/green]点格挡。将2张枪火火花加入手牌。消耗。")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public AngelicReload() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }

    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class TerminalVolley : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(26, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "终端齐射"), ("description", "造成[red]{Damage}[/red]点伤害。将2张枪火火花加入手牌。消耗。")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public TerminalVolley() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }

    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await GeneratedTokenHelper.AddGunsparkToHand(this);
        await GeneratedTokenHelper.AddGunsparkToHand(this);
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(7);
}
