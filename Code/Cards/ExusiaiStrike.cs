using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstMod.Code.CardPools;

namespace MyFirstMod.Code.Cards;

/// <summary>
/// 能天使专属打击 — 基础攻击牌，带 Strike 标签
/// </summary>
[Pool(typeof(ExusiaiCardPool))]
public class ExusiaiStrike : MyFirstModCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;


    public override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];
    public override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    public override List<(string, string)> Localization => [("title", "打击"), ("description", "造成[red]{Damage}[/red]点伤害。")];

    public ExusiaiStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
