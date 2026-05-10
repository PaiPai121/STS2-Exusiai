using System.Linq;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

static class GeneratedTokenHelper
{
    public static async Task AddTokenToHandAndRegister(CardModel token, MyFirstModCardModel source)
    {
        token.Owner = source.Owner;
        await CardPileCmd.Add(token, PileType.Hand, CardPilePosition.Top, source, skipVisuals: false);
        var combatState = source.Owner?.Creature?.CombatState;
        Godot.GD.Print($"[myfirstmod] token register start type={token.GetType().Name} id={token.Id} combatState_null={combatState == null}");
        if (combatState != null)
        {
            try
            {
                var addMethod = combatState.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                    .FirstOrDefault(m => m.Name.Contains("Add") && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.IsAssignableFrom(typeof(CardModel)));
                if (addMethod != null)
                {
                    addMethod.Invoke(combatState, [token]);
                    Godot.GD.Print($"[myfirstmod] token register invoked method={addMethod.Name} type={token.GetType().Name} id={token.Id}");
                }
                else
                {
                    Godot.GD.Print("[myfirstmod] token register add method not found");
                }
            }
            catch (Exception ex)
            {
                Godot.GD.Print($"[myfirstmod] token register exception {ex}");
            }
        }
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class GunslingerRush : RapidFireCardModel
{

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new BlockVar(2, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "枪手突进"), ("description", "造成[red]{Damage}[/red]点伤害。获得[green]{Block}[/green]点格挡。速射。")];
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
    public override List<(string, string)> Localization => [("title", "战术侧闪"), ("description", "获得[green]{Block}[/green]点格挡。抽[blue]{Cards}[/blue]张牌。将1张枪火火花加入手牌。该牌获得虚无与消耗。")];
    public TacticalSidestep() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);

        CardModel spark = ModelDb.Card<Gunspark>().ToMutable();
        await GeneratedTokenHelper.AddTokenToHandAndRegister(spark, this);
    }
    public override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class ChainReaction : MyFirstModCardModel
{
    private bool _chainReactionActive;

    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "连锁反应"), ("description", "造成[red]{Damage}[/red]点伤害。本回合中，你打出的攻击牌费用至少为1。")];
    public ChainReaction() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }

        _chainReactionActive = true;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext c, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (Owner == player)
            _chainReactionActive = false;

        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay p)
    {
        if (!_chainReactionActive)
            return Task.CompletedTask;

        if (p.Card == null)
            return Task.CompletedTask;

        if (p.Card == this)
            return Task.CompletedTask;

        if (p.Card.Owner != Owner)
            return Task.CompletedTask;

        if (p.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        p.Card.SetStarCostUntilPlayed(1);
        return Task.CompletedTask;
    }

    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class EmergencyShield : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "应急护盾"), ("description", "获得[green]{Block}[/green]点格挡。抽[blue]{Cards}[/blue]张牌。")];
    public EmergencyShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class RapidStance : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new BlockVar(3, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "速射架势"), ("description", "抽[blue]{Cards}[/blue]张牌。获得[green]{Block}[/green]点格挡。")];
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
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "弹幕射击"), ("description", "造成[red]{Damage}[/red]点伤害。抽[blue]{Cards}[/blue]张牌。速射。")];
    public BarrageFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}

[Pool(typeof(ExusiaiCardPool))]
public class SuppressiveFire : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move), new BlockVar(4, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "压制射击"), ("description", "造成[red]{Damage}[/red]点伤害。获得[green]{Block}[/green]点格挡。")];
    public SuppressiveFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class WarfarinsPlasma : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(13, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "华法琳特调"), ("description", "造成[red]{Damage}[/red]点伤害。抽[blue]{Cards}[/blue]张牌。")];
    public WarfarinsPlasma() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class QuickMagazine : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "快速换弹"), ("description", "抽[blue]{Cards}[/blue]张牌。将1张枪火火花加入手牌。该牌获得虚无与消耗。")];
    public QuickMagazine() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);

        CardModel spark = ModelDb.Card<Gunspark>().ToMutable();
        await GeneratedTokenHelper.AddTokenToHandAndRegister(spark, this);
    }
    public override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}

[Pool(typeof(ExusiaiCardPool))]
public class SweepMode : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14, ValueProp.Move), new BlockVar(6, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "扫射模式"), ("description", "造成[red]{Damage}[/red]点伤害。获得[green]{Block}[/green]点格挡。")];
    public SweepMode() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}

[Pool(typeof(ExusiaiCardPool))]
public class PiercingRound : MyFirstModCardModel
{
    public override string PortraitPath => "res://myfirstmod/images/cards/PiercingRound.png";
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "穿甲弹"), ("description", "造成[red]{Damage}[/red]点伤害。")];
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
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11, ValueProp.Move), new CardsVar(1)];
    public override List<(string, string)> Localization => [("title", "追猎指令"), ("description", "造成[red]{Damage}[/red]点伤害。抽[blue]{Cards}[/blue]张牌。将1张枪火火花加入手牌。该牌获得虚无与消耗。")];
    public PursuitOrder() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);

        CardModel spark = ModelDb.Card<Gunspark>().ToMutable();
        await GeneratedTokenHelper.AddTokenToHandAndRegister(spark, this);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class FullAuto : RapidFireCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move), new CardsVar(2)];
    public override List<(string, string)> Localization => [("title", "全自动"), ("description", "造成[red]{Damage}[/red]点伤害。抽[blue]{Cards}[/blue]张牌。速射。消耗。")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([CardKeyword.Exhaust]);
    public FullAuto() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target != null)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);
        }
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await TryGenerateRapidFireCopy(c, p);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}

[Pool(typeof(ExusiaiCardPool))]
public class BulletHell : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "枪林弹雨"), ("description", "造成[red]{Damage}[/red]点伤害。将2张枪火火花加入手牌。它们获得虚无与消耗。消耗。")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public BulletHell() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (p.Target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c);

        CardModel firstSpark = ModelDb.Card<Gunspark>().ToMutable();
        await GeneratedTokenHelper.AddTokenToHandAndRegister(firstSpark, this);

        CardModel secondSpark = ModelDb.Card<Gunspark>().ToMutable();
        await GeneratedTokenHelper.AddTokenToHandAndRegister(secondSpark, this);
    }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6);
}

[Pool(typeof(TokenCardPool))]
public class Gunspark : MyFirstModCardModel
{
    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move)];
    public override List<(string, string)> Localization => [("title", "枪火火花"), ("description", "造成[red]{Damage}[/red]点伤害。虚无。消耗。")];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Exhaust];
    public Gunspark() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, false) { }
    public override async Task OnPlay(PlayerChoiceContext c, CardPlay p) { if (p.Target != null) await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(p.Target).Execute(c); }
    public override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2);
}
