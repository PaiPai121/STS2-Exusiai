using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Cards;
using MyFirstMod.Code.PotionPools;
using MyFirstMod.Code.RelicPools;
using MyFirstMod.Code.Relics;

namespace MyFirstMod.Code.Characters;

/// <summary>
/// 能天使 Exusiai - 来自明日方舟的狙击干员
/// </summary>
public class Exusiai : PlaceholderCharacterModel
{
    // ========== 角色基础属性 ==========
    public override Color NameColor => new(1f, 0.4f, 0.3f);
    public override Color EnergyLabelOutlineColor => new(0.8f, 0.2f, 0.1f);
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    // ========== 场景路径（基于 InesSilent 场景） ==========
    public override string CustomVisualPath => "res://myfirstmod/scenes/character/exusiai_default.tscn";
    public override string CustomEnergyCounterPath => "res://scenes/combat/energy_counters/ironclad_energy_counter.tscn";
    public override string CustomCharacterSelectBg => "res://myfirstmod/scenes/ui/char_select_bg_exusiai.tscn";
    public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    public override string CustomIconPath => "res://myfirstmod/scenes/ui/exusiai_icon.tscn";
    public override string CustomMerchantAnimPath => "res://myfirstmod/scenes/character/exusiai_merchant.tscn";
    public override string CustomRestSiteAnimPath => "res://myfirstmod/scenes/character/exusiai_rest_site.tscn";

    // ========== 图标路径 ==========
    public override string CustomCharacterSelectIconPath => "res://myfirstmod/images/exusiai/char_select_exusiai.png";
    public override string CustomCharacterSelectLockedIconPath => "res://myfirstmod/images/exusiai/char_select_exusiai_locked.png";
    public override string CustomIconTexturePath => "res://myfirstmod/images/exusiai/character_icon_exusiai.png";

    // ========== 音效（暂用铁甲战士的） ==========
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override bool ShouldReceiveCombatHooks => true;

    // ========== 池子 ==========
    public override CardPoolModel CardPool => ModelDb.CardPool<ExusiaiCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ExusiaiRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ExusiaiPotionPool>();

    // ========== 初始卡组 ==========
    public override IEnumerable<CardModel> StartingDeck => [
        // 5 打击
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        // 4 防御
        ModelDb.Card<ExusiaiDefend>(),
        ModelDb.Card<ExusiaiDefend>(),
        ModelDb.Card<ExusiaiDefend>(),
        ModelDb.Card<ExusiaiDefend>(),
        // 1 交叉火力
        ModelDb.Card<CardTemplate>(),
    ];

    // ========== 初始遗物 ==========
    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<SniperChipset>(),
    ];

    // ========== 攻击特效 ==========
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext context, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public override Task AfterAttack(MegaCrit.Sts2.Core.Commands.Builders.AttackCommand command)
    {
        GD.Print($"[myfirstmod] Exusiai AfterAttack attacker={(command.Attacker?.Player?.Character == this)}");
        return Task.CompletedTask;
    }

    public override Task BeforeDeath(Creature creature)
    {
        GD.Print($"[myfirstmod] Exusiai BeforeDeath player={(creature.Player?.Character == this)}");
        return Task.CompletedTask;
    }
}
