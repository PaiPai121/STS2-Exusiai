using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

[Pool(typeof(ExusiaiCardPool))]
public class GunslingerRush : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move)];
    public GunslingerRush() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class TacticalSidestep : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7, ValueProp.Move), new CardsVar(1)];
    public TacticalSidestep() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class ChainReaction : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];
    public ChainReaction() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class EmergencyShield : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10, ValueProp.Move)];
    public EmergencyShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p); }
    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class RapidStance : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public RapidStance() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner); }
    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class BarrageFire : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move)];
    public BarrageFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class SuppressiveFire : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];
    public SuppressiveFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class WarfarinsPlasma : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(13, ValueProp.Move)];
    public WarfarinsPlasma() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class QuickMagazine : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public QuickMagazine() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner); }
    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class SweepMode : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move)];
    public SweepMode() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class PiercingRound : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15, ValueProp.Move)];
    public PiercingRound() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5);
}

[Pool(typeof(ExusiaiCardPool))]
public class PursuitOrder : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11, ValueProp.Move), new CardsVar(1)];
    public PursuitOrder() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class FullAuto : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([CardKeyword.Exhaust]);
    public FullAuto() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class BulletHell : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public BulletHell() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6);
}

[Pool(typeof(ExusiaiCardPool))]
public class Gunspark : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    public Gunspark() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, false) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}

[Pool(typeof(ExusiaiCardPool))]
public class StrikeCopy : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    public StrikeCopy() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, false) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class StrikeCopyPlus : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    public StrikeCopyPlus() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, false) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
}
