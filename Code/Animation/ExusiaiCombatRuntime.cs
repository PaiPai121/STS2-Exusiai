using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MyFirstMod.Code.Characters;

namespace MyFirstMod.Code.Animation;

internal static class ExusiaiCombatRuntime
{
    private static readonly StringName DefaultClip = new("default");
    private static readonly StringName AttackAnimation = new("attack");
    private static readonly StringName IdleAnimation = new("idle");
    private static readonly StringName DieAnimation = new("die");
    private static ulong _attackPlaybackVersion;

    public static void TryTriggerAttack(AttackCommand command)
    {
        Creature? attacker = command.Attacker;
        if (attacker?.Player?.Character is not Exusiai)
            return;

        GD.Print("[myfirstmod] runtime TryTriggerAttack matched Exusiai attacker");
        if (!TryGetAnimatedSprite(attacker, out AnimatedSprite2D? sprite))
            return;

        SpriteFrames? frames = LoadFrames(AttackAnimation);
        if (frames == null || sprite == null)
            return;

        ulong playbackVersion = ++_attackPlaybackVersion;
        ApplyFrames(sprite, frames, AttackAnimation);
        ScheduleIdleRestore(sprite, playbackVersion, frames);
    }

    public static void TryTriggerDeath(Creature creature)
    {
        if (creature.Player?.Character is not Exusiai)
            return;

        GD.Print("[myfirstmod] runtime TryTriggerDeath matched Exusiai creature");
        if (!TryGetAnimatedSprite(creature, out AnimatedSprite2D? sprite))
            return;

        SpriteFrames? frames = LoadFrames(DieAnimation);
        if (frames == null || sprite == null)
            return;

        ApplyFrames(sprite, frames, DieAnimation);
    }

    private static bool TryGetAnimatedSprite(Creature creature, out AnimatedSprite2D? sprite)
    {
        sprite = NCombatRoom.Instance?
            .GetCreatureNode(creature)?
            .Visuals?
            .GetCurrentBody() as AnimatedSprite2D;

        if (sprite == null || !GodotObject.IsInstanceValid(sprite))
        {
            GD.Print("[myfirstmod] runtime missing AnimatedSprite2D");
            sprite = null;
            return false;
        }

        return true;
    }

    private static SpriteFrames? LoadFrames(StringName animationName)
    {
        string path = $"res://myfirstmod/scenes/character/exusiai_{animationName}_sprite_frames.tres";
        SpriteFrames? frames = ResourceLoader.Load<SpriteFrames>(path);
        if (frames == null)
        {
            GD.Print($"[myfirstmod] runtime failed to load frames {path}");
            return null;
        }

        return frames;
    }

    private static void ApplyFrames(AnimatedSprite2D sprite, SpriteFrames frames, StringName animationName)
    {
        string path = $"res://myfirstmod/scenes/character/exusiai_{animationName}_sprite_frames.tres";
        GD.Print($"[myfirstmod] runtime applying frames {path} to {sprite.GetPath()}");
        sprite.SpriteFrames = frames;
        sprite.Play(DefaultClip);
    }

    private static async void ScheduleIdleRestore(AnimatedSprite2D sprite, ulong playbackVersion, SpriteFrames attackFrames)
    {
        double durationSeconds = ComputeAnimationDurationSeconds(attackFrames);
        GD.Print($"[myfirstmod] runtime scheduling idle restore in {durationSeconds:0.###} sec version={playbackVersion}");

        SceneTree? tree = sprite.GetTree();
        if (tree == null)
            return;

        await sprite.ToSignal(tree.CreateTimer(durationSeconds), SceneTreeTimer.SignalName.Timeout);

        if (!GodotObject.IsInstanceValid(sprite))
            return;

        if (playbackVersion != _attackPlaybackVersion)
        {
            GD.Print($"[myfirstmod] runtime skip idle restore due to newer attack version current={_attackPlaybackVersion} expected={playbackVersion}");
            return;
        }

        SpriteFrames? idleFrames = LoadFrames(IdleAnimation);
        if (idleFrames == null)
            return;

        GD.Print("[myfirstmod] runtime restoring idle after attack delay");
        ApplyFrames(sprite, idleFrames, IdleAnimation);
    }

    private static double ComputeAnimationDurationSeconds(SpriteFrames frames)
    {
        int frameCount = frames.GetFrameCount(DefaultClip);
        double speed = frames.GetAnimationSpeed(DefaultClip);
        if (frameCount <= 0)
            return 0.35;
        if (speed <= 0.0)
            return 0.35;

        double duration = frameCount / speed;
        return Math.Max(0.35, duration);
    }
}

[HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.Execute))]
internal static class ExusiaiAttackCommandExecutePatch
{
    private static void Prefix(AttackCommand __instance)
    {
        GD.Print("[myfirstmod] harmony AttackCommand.Execute prefix");
        ExusiaiCombatRuntime.TryTriggerAttack(__instance);
    }
}

[HarmonyPatch(typeof(Creature), nameof(Creature.InvokeDiedEvent))]
internal static class ExusiaiCreatureInvokeDiedEventPatch
{
    private static void Prefix(Creature __instance)
    {
        GD.Print("[myfirstmod] harmony Creature.InvokeDiedEvent prefix");
        ExusiaiCombatRuntime.TryTriggerDeath(__instance);
    }
}
