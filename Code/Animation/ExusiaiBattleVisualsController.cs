using Godot;

namespace MyFirstMod.Code.Animation;

public partial class ExusiaiBattleVisualsController : Node2D
{
    private static readonly StringName AttackAnimation = new("attack");
    private static readonly StringName IdleAnimation = new("idle");
    private static readonly StringName DieAnimation = new("die");
    private static readonly StringName DefaultClip = new("default");

    private AnimatedSprite2D? _visuals;

    public override void _Ready()
    {
        GD.Print("[myfirstmod] Exusiai C# visuals controller ready");
        _visuals = GetNodeOrNull<AnimatedSprite2D>("Visuals");
        if (_visuals == null)
        {
            GD.Print("[myfirstmod] Exusiai C# visuals controller missing Visuals node");
            return;
        }

        _visuals.AnimationFinished += OnAnimationFinished;
        PlayIdle();
    }

    public override void _ExitTree()
    {
        if (_visuals != null)
            _visuals.AnimationFinished -= OnAnimationFinished;
    }

    public void PlayIdle()
    {
        GD.Print("[myfirstmod] Exusiai C# play_idle");
        if (!EnsureVisuals())
            return;

        SetFrames(IdleAnimation);
        _visuals!.Play(DefaultClip);
    }

    public void PlayAttack()
    {
        GD.Print("[myfirstmod] Exusiai C# play_attack");
        if (!EnsureVisuals())
            return;

        SetFrames(AttackAnimation);
        _visuals!.Play(DefaultClip);
    }

    public void PlayDie()
    {
        GD.Print("[myfirstmod] Exusiai C# play_die");
        if (!EnsureVisuals())
            return;

        SetFrames(DieAnimation);
        _visuals!.Play(DefaultClip);
    }

    private bool EnsureVisuals()
    {
        _visuals ??= GetNodeOrNull<AnimatedSprite2D>("Visuals");
        return _visuals != null;
    }

    private void SetFrames(StringName animName)
    {
        string path = $"res://myfirstmod/scenes/character/exusiai_{animName}_sprite_frames.tres";
        GD.Print($"[myfirstmod] Exusiai C# set_frames {path}");
        SpriteFrames? frames = ResourceLoader.Load<SpriteFrames>(path);
        if (frames == null)
        {
            GD.Print($"[myfirstmod] Exusiai C# failed to load frames {path}");
            return;
        }

        _visuals!.SpriteFrames = frames;
    }

    private void OnAnimationFinished()
    {
        GD.Print("[myfirstmod] Exusiai C# animation_finished");
        if (_visuals?.SpriteFrames == null)
            return;

        if (_visuals.SpriteFrames.GetAnimationLoop(DefaultClip))
            return;

        if (_visuals.SpriteFrames.GetFrameCount(DefaultClip) <= 24)
            PlayIdle();
    }
}
