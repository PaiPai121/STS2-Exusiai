using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MyFirstMod.Code.Relics;

/// <summary>
/// 遗物模板 - 复制此文件创建新遗物
///
/// 步骤：
/// 1. 复制此文件并重命名（如 MyNewRelic.cs）
/// 2. 修改类名和命名空间
/// 3. 修改遗物属性（稀有度、遗物池等）
/// 4. 实现触发效果
/// 5. 添加遗物图标：myfirstmod/images/relics/{ClassName}.png
/// 6. 添加本地化：myfirstmod/localization/zhs/relics.json
/// 7. 重新编译和导出 PCK
/// </summary>
// [Pool(typeof(SharedRelicPool))]  // 修改遗物池：SharedRelicPool, IroncladRelicPool, SilentRelicPool 等
// 注意：模板文件不应该注册到遗物池，使用时取消注释并重命名类
public abstract class RelicTemplate : MyFirstModRelicModel
{
    // ========== 基础属性 ==========
    public override RelicRarity Rarity => RelicRarity.Common;  // 稀有度：Common, Uncommon, Rare, Boss, Shop, Special

    // ========== 遗物数值 ==========
    // 支持的数值类型：
    // - CardsVar(value)           // 抽牌数
    // - DamageVar(value, prop)    // 伤害
    // - BlockVar(value, prop)     // 格挡
    // - MagicNumberVar(value)     // 魔法数字（通用数值）
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)  // 抽1张牌
    ];

    // ========== 触发时机 ==========
    // 常用触发方法（按需覆写）：
    //
    // 玩家回合开始时
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 抽牌
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
    }

    // 其他触发时机示例（取消注释需要的）：

    // 玩家回合结束时
    // public override async Task AfterPlayerTurnEnd(PlayerChoiceContext choiceContext, Player player)
    // {
    //     await base.AfterPlayerTurnEnd(choiceContext, player);
    // }

    // 战斗开始时
    // public override async Task AfterBattleStart(PlayerChoiceContext choiceContext, Player player)
    // {
    //     await base.AfterBattleStart(choiceContext, player);
    // }

    // 获得遗物时
    // public override async Task OnEquip(PlayerChoiceContext choiceContext, Player player)
    // {
    //     await base.OnEquip(choiceContext, player);
    // }

    // 失去遗物时
    // public override async Task OnUnequip(PlayerChoiceContext choiceContext, Player player)
    // {
    //     await base.OnUnequip(choiceContext, player);
    // }

    // 受到伤害时
    // public override async Task AfterPlayerHurt(PlayerChoiceContext choiceContext, Player player, int damageAmount)
    // {
    //     await base.AfterPlayerHurt(choiceContext, player, damageAmount);
    // }
}

/*
本地化文件示例（myfirstmod/localization/zhs/relics.json）：

{
    "MYFIRSTMOD-RELIC_TEMPLATE.title": "遗物模板",
    "MYFIRSTMOD-RELIC_TEMPLATE.description": "每回合开始时，抽[blue]{Cards}[/blue]张牌。",
    "MYFIRSTMOD-RELIC_TEMPLATE.flavor": "这是一段风味文字。"
}

描述中的占位符：
- {Cards}              显示抽牌数
- {Damage:diff()}      显示伤害值
- {Block:diff()}       显示格挡值
- {MagicNumber:diff()} 显示魔法数字

遗物ID规则：
- 命名空间第一段大写 + 类名SNAKE_CASE
- 例如：MyFirstMod.Code.Relics.RelicTemplate → MYFIRSTMOD-RELIC_TEMPLATE
*/
