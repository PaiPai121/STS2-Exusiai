using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.Keywords;

namespace MyFirstMod.Code.Cards;

/// <summary>
/// 速射卡牌基类：带有速射关键字的攻击牌，打出时额外生成一张复制加入手牌。
/// 复制不会再次触发速射（防止无限套娃）。
/// 子类必须在 OnPlay 末尾调用 TryGenerateRapidFireCopy。
/// </summary>
public abstract class RapidFireCardModel : MyFirstModCardModel
{
    private bool _hasRapidFire = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => _hasRapidFire ? [MyKeywords.RapidFire] : [];

    protected RapidFireCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 在 OnPlay 末尾调用此方法。仅在卡牌有速射关键字时生成复制。
    /// 如果战斗中所有敌人都已死亡，不再生成复制（避免击杀后追加异步操作导致回合卡住）。
    /// </summary>
    protected async Task TryGenerateRapidFireCopy(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 只有带速射关键字的牌才触发（复制品已移除速射，不会再次触发）
        if (!_hasRapidFire)
            return;

        // 如果敌人已全部死亡（战斗即将结束），不再生成复制
        var combatState = CombatState;
        if (combatState != null)
        {
            bool allEnemiesDead = true;
            foreach (var enemy in combatState.Enemies)
            {
                if (enemy.IsAlive)
                {
                    allEnemiesDead = false;
                    break;
                }
            }
            if (allEnemiesDead)
                return;
        }

        // 克隆当前卡牌（使用 CardModel.CreateClone，与原版遗物一致）
        CardModel copy = CreateClone();

        // 禁用速射关键字（复制不再触发，防套娃）
        if (copy is RapidFireCardModel rapidFireCopy)
        {
            rapidFireCopy._hasRapidFire = false;
        }
        if (copy.Keywords.Contains(MyKeywords.RapidFire))
        {
            copy.RemoveKeyword(MyKeywords.RapidFire);
        }

        // 复制牌设为虚无（回合结束未打出则消耗）和消耗（打出后移除）
        copy.AddKeyword(CardKeyword.Ethereal);
        copy.AddKeyword(CardKeyword.Exhaust);

        // 加入手牌
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
    }
}
