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
/// 能天使专属防御 — 基础技能牌，带 Defend 标签
/// </summary>
[Pool(typeof(ExusiaiCardPool))]
public class ExusiaiDefend : MyFirstModCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];
    public override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    public ExusiaiDefend() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
