using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Keywords;

namespace MyFirstMod.Code.Cards;

/// <summary>
/// 交叉火力 — 能天使专属普通攻击牌，带速射关键字。
/// 打出时额外生成一张复制加入手牌，复制带虚无与消耗，不会再次触发速射。
/// </summary>
[Pool(typeof(ExusiaiCardPool))]
public class CardTemplate : RapidFireCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;


    public override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    public override List<(string, string)> Localization => [("title", "交叉火力"), ("description", "造成[red]{Damage}[/red]点伤害。")];

    public CardTemplate() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await TryGenerateRapidFireCopy(choiceContext, cardPlay);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}

/// <summary>
/// 覆盖射击 — 能天使专属普通攻击牌。
/// 造成轻度伤害并补一张牌，作为普通攻击续航选择。
/// </summary>
[Pool(typeof(ExusiaiCardPool))]
public class CoverFire : MyFirstModCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
        new CardsVar(1)
    ];

    public override List<(string, string)> Localization => [("title", "覆盖射击"), ("description", "造成[red]{Damage}[/red]点伤害。抽[blue]{Cards}[/blue]张牌。")];

    public CoverFire() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}

/// <summary>
/// 战术撤退 — 能天使专属普通技能牌。
/// 获得格挡并抽一张牌，兼顾防御与续航。
/// </summary>
[Pool(typeof(ExusiaiCardPool))]
public class TacticalRetreat : MyFirstModCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move),
        new CardsVar(1)
    ];

    public override List<(string, string)> Localization => [("title", "战术撤退"), ("description", "获得[green]{Block}[/green]点格挡。抽[blue]{Cards}[/blue]张牌。")];

    public TacticalRetreat() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
