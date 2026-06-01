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
        _visuals = GetNodeOrNull<AnimatedSprite2D>("Visuals");
        if (_visuals == null)
        {
            GD.PrintErr("[exusiai] visuals controller missing Visuals node");
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
        if (!EnsureVisuals())
            return;

        SetFrames(IdleAnimation);
        _visuals!.Play(DefaultClip);
    }

    public void PlayAttack()
    {
        if (!EnsureVisuals())
            return;

        SetFrames(AttackAnimation);
        _visuals!.Play(DefaultClip);
    }

    public void PlayDie()
    {
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
        SpriteFrames? frames = ResourceLoader.Load<SpriteFrames>(path);
        if (frames == null)
        {
            GD.PrintErr($"[exusiai] failed to load frames {path}");
            return;
        }

        _visuals!.SpriteFrames = frames;
    }

    private void OnAnimationFinished()
    {
        if (_visuals?.SpriteFrames == null)
            return;

        if (_visuals.SpriteFrames.GetAnimationLoop(DefaultClip))
            return;

        if (_visuals.SpriteFrames.GetFrameCount(DefaultClip) <= 24)
            PlayIdle();
    }
}
