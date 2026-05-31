using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MyFirstMod.Code.CardPools;
using MyFirstMod.Code.Cards;
using MyFirstMod.Code.PotionPools;
using MyFirstMod.Code.RelicPools;
using MyFirstMod.Code.Relics;

namespace MyFirstMod.Code.Characters;

/// <summary>
/// Exusiai playable character.
/// </summary>
public class Exusiai : PlaceholderCharacterModel
{
    // Character basics
    public override Color NameColor => new(1f, 0.4f, 0.3f);
    public override Color EnergyLabelOutlineColor => new(0.8f, 0.2f, 0.1f);
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    // Scene paths
    public override string CustomVisualPath => "res://myfirstmod/scenes/character/exusiai_default.tscn";
    public override string CustomEnergyCounterPath => "res://scenes/combat/energy_counters/ironclad_energy_counter.tscn";
    public override string CustomCharacterSelectBg => "res://myfirstmod/scenes/ui/char_select_bg_exusiai.tscn";
    public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    public override string CustomIconPath => "res://myfirstmod/scenes/ui/exusiai_icon.tscn";
    public override string CustomMerchantAnimPath => "res://myfirstmod/scenes/character/exusiai_merchant.tscn";
    // The base Ironclad rest scene is patched at runtime to show Exusiai-specific visuals.
    public override string CustomRestSiteAnimPath => "res://scenes/rest_site/characters/ironclad_rest_site.tscn";

    // Icon and preloaded asset paths
    public override string CustomCharacterSelectIconPath => "res://myfirstmod/images/exusiai/char_select_exusiai.png";
    public override string CustomCharacterSelectLockedIconPath => "res://myfirstmod/images/exusiai/char_select_exusiai_locked.png";
    public override string CustomIconTexturePath => "res://myfirstmod/images/exusiai/character_icon_exusiai.png";
    public override IEnumerable<string> ExtraAssetPaths => [
        "res://myfirstmod/images/exusiai/character_icon_exusiai.png",
        "res://myfirstmod/images/exusiai/char_select_exusiai.png",
        "res://myfirstmod/images/exusiai/char_select_exusiai_locked.png",
        "res://myfirstmod/images/exusiai/energy_exusiai.png",
        "res://myfirstmod/images/exusiai/energy_exusiai_big.png",
        "res://myfirstmod/assets/character/generated/exusiai_rest_site.png",
        "res://myfirstmod/images/powers/AngelsBlessingPower.png",
        "res://myfirstmod/images/powers/ChainReactionPower.png",
        "res://myfirstmod/images/powers/FireControlPower.png",
        "res://myfirstmod/images/powers/OverclockPower.png",
        "res://myfirstmod/images/powers/SparkCircuitPower.png",
        "res://myfirstmod/images/powers/SweepModePower.png",
    ];

    // Audio
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override bool ShouldReceiveCombatHooks => true;

    // Pools
    public override CardPoolModel CardPool => ModelDb.CardPool<ExusiaiCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ExusiaiRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ExusiaiPotionPool>();

    // Starting deck
    public override IEnumerable<CardModel> StartingDeck => [
        // 5 Strikes
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        ModelDb.Card<ExusiaiStrike>(),
        // 4 Defends
        ModelDb.Card<ExusiaiDefend>(),
        ModelDb.Card<ExusiaiDefend>(),
        ModelDb.Card<ExusiaiDefend>(),
        ModelDb.Card<ExusiaiDefend>(),
        // 1 Crossfire
        ModelDb.Card<CardTemplate>(),
    ];

    // Starting relic
    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<SniperChipset>(),
    ];

    // Attack VFX
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}
