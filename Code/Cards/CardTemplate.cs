using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MyFirstMod.Code.Cards;

/// <summary>
/// 卡牌模板 - 复制此文件创建新卡牌
///
/// 步骤：
/// 1. 复制此文件并重命名（如 MyNewCard.cs）
/// 2. 修改类名和命名空间
/// 3. 修改卡牌属性（耗能、类型、稀有度等）
/// 4. 实现 OnPlay 效果
/// 5. 添加卡图：myfirstmod/images/cards/{ClassName}.png
/// 6. 添加本地化：myfirstmod/localization/zhs/cards.json
/// 7. 重新编译和导出 PCK
/// </summary>
// [Pool(typeof(ColorlessCardPool))]  // 修改卡池：ColorlessCardPool, IroncladCardPool, SilentCardPool 等
// 注意：模板文件不应该注册到卡池，使用时取消注释并重命名类
public abstract class CardTemplate : MyFirstModCardModel
{
    // ========== 基础属性 ==========
    private const int energyCost = 1;                           // 耗能
    private const CardType type = CardType.Attack;              // 类型：Attack, Skill, Power
    private const CardRarity rarity = CardRarity.Common;        // 稀有度：Common, Uncommon, Rare
    private const TargetType targetType = TargetType.AnyEnemy;  // 目标：AnyEnemy, AllEnemies, Self, None
    private const bool shouldShowInCardLibrary = true;          // 是否在图鉴中显示

    // ========== 卡牌数值 ==========
    // 支持的数值类型：
    // - DamageVar(value, ValueProp.Move)           // 伤害
    // - BlockVar(value, ValueProp.Move)            // 格挡
    // - MagicNumberVar(value)                      // 魔法数字（通用数值）
    // - DrawVar(value)                             // 抽牌数
    // - DiscardVar(value)                          // 弃牌数
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move)  // 12点伤害
    ];

    public CardTemplate() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // ========== 打出效果 ==========
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 空指针检查
        if (cardPlay.Target == null) return;

        // 造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 其他常用效果示例：

        // 获得格挡
        // await BlockCmd.GainBlock(DynamicVars.Block.BaseValue)
        //     .FromCard(this)
        //     .Execute(choiceContext);

        // 抽牌
        // await DrawCmd.Draw(DynamicVars.Draw.BaseValue)
        //     .Execute(choiceContext);

        // 施加能力（如力量）
        // await ApplyPowerCmd.ApplyPower<StrengthPower>(DynamicVars.MagicNumber.BaseValue)
        //     .FromCard(this)
        //     .Targeting(choiceContext.Player)
        //     .Execute(choiceContext);
    }

    // ========== 升级效果 ==========
    protected override void OnUpgrade()
    {
        // 升级伤害 +4
        DynamicVars.Damage.UpgradeValueBy(4);

        // 其他升级方式：
        // DynamicVars.Block.UpgradeValueBy(3);        // 格挡 +3
        // DynamicVars.MagicNumber.UpgradeValueBy(1);  // 魔法数字 +1
        // UpgradeEnergyCost(-1);                      // 降低耗能
    }
}

/*
本地化文件示例（myfirstmod/localization/zhs/cards.json）：

{
    "MYFIRSTMOD-CARD_TEMPLATE.title": "卡牌模板",
    "MYFIRSTMOD-CARD_TEMPLATE.description": "造成{Damage:diff()}点伤害。"
}

描述中的占位符：
- {Damage:diff()}     显示伤害值（带升级差异）
- {Block:diff()}      显示格挡值
- {MagicNumber:diff()} 显示魔法数字
- {Draw}              显示抽牌数
- {Discard}           显示弃牌数

卡牌ID规则：
- 命名空间第一段大写 + 类名SNAKE_CASE
- 例如：MyFirstMod.Code.Cards.CardTemplate → MYFIRSTMOD-CARD_TEMPLATE
*/
