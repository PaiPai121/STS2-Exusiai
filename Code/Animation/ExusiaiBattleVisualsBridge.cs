using Godot;

namespace MyFirstMod.Code.Animation;

public partial class ExusiaiBattleVisualsBridge : Node
{
    public override void _Ready()
    {
        GD.Print("[myfirstmod] Exusiai bridge ready");
    }

    public override void _ExitTree()
    {
        GD.Print("[myfirstmod] Exusiai bridge exit");
    }

    public void Trigger(StringName method)
    {
        GD.Print($"[myfirstmod] Exusiai bridge trigger {method}");
        Node? parent = GetParent();
        if (parent == null)
        {
            GD.Print("[myfirstmod] Exusiai bridge missing parent");
            return;
        }

        if (method == "play_attack")
        {
            parent.CallDeferred(nameof(ExusiaiBattleVisualsController.PlayAttack));
            return;
        }

        if (method == "play_die")
        {
            parent.CallDeferred(nameof(ExusiaiBattleVisualsController.PlayDie));
            return;
        }

        parent.CallDeferred(nameof(ExusiaiBattleVisualsController.PlayIdle));
    }
}
