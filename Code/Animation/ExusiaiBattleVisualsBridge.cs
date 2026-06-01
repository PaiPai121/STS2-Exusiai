using Godot;

namespace MyFirstMod.Code.Animation;

public partial class ExusiaiBattleVisualsBridge : Node
{
    public void Trigger(StringName method)
    {
        Node? parent = GetParent();
        if (parent == null)
        {
            GD.PrintErr("[exusiai] bridge missing parent");
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
