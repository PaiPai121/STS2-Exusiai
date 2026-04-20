using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.Keywords;

namespace MyFirstMod.Code.Cards;

/// <summary>
/// 速射卡牌基类：带有速射关键字的攻击牌，打出时额外生成一张0费复制加入手牌。
/// 复制不会再次触发速射（防止无限套娃）。
/// 子类必须在 OnPlay 末尾调用 TryGenerateRapidFireCopy。
/// </summary>
public abstract class RapidFireCardModel : MyFirstModCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.RapidFire];

    protected RapidFireCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 在 OnPlay 末尾调用此方法。仅在卡牌有速射关键字且是系列中首次打出时生成复制。
    /// </summary>
    protected async Task TryGenerateRapidFireCopy(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 只有带速射关键字的牌才触发（复制品已移除速射，不会再次触发）
        if (!CanonicalKeywords.Contains(MyKeywords.RapidFire))
            return;

        // 只在首次打出时触发（防止 ModifyCardPlayCount 导致的多次打出重复生成）
        if (!cardPlay.IsFirstInSeries)
            return;

        // 克隆当前卡牌（使用 CardModel.CreateClone，与原版遗物一致）
        CardModel copy = CreateClone();

        // 移除速射关键字（复制不再触发，防套娃）
        copy.RemoveKeyword(MyKeywords.RapidFire);

        // 设为0费（打出后恢复原费）
        copy.EnergyCost.SetUntilPlayed(0);

        // 加入手牌
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
    }
}
