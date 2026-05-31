using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MyFirstMod.Code.Characters;

namespace MyFirstMod.Code.Animation;

[HarmonyPatch(typeof(NRestSiteCharacter), nameof(NRestSiteCharacter.Create))]
internal static class ExusiaiRestSiteCreatePatch
{
    private const string CharacterTexturePath = "res://myfirstmod/assets/character/generated/exusiai_rest_site.png";
    private const string SelectionReticlePath = "res://scenes/ui/selection_reticle.tscn";
    private const int ShadowPointCount = 48;

    private static bool Prefix(Player player, int characterIndex, ref NRestSiteCharacter __result)
    {
        if (player.Character is not Exusiai)
            return true;

        NRestSiteCharacter root = new()
        {
            Name = "ExusiaiRestSite",
            Position = new Vector2(-2, 42),
            Scale = new Vector2(0.760006f, 0.760006f),
            Player = player,
            _characterIndex = characterIndex
        };

        Polygon2D groundShadow = new()
        {
            Name = "GroundShadow",
            Position = new Vector2(-48, 136),
            Polygon = CreateEllipsePolygon(92f, 18f),
            Color = new Color(0.035f, 0.028f, 0.02f, 0.24f),
            ZIndex = -1
        };
        root.AddChild(groundShadow);

        Sprite2D visual = new()
        {
            Name = "Visuals",
            Position = new Vector2(-18, -52),
            Scale = new Vector2(0.36f, 0.36f),
            Texture = ResourceLoader.Load<Texture2D>(CharacterTexturePath),
            Modulate = new Color(0.92f, 0.84f, 0.74f, 0.98f)
        };
        root.AddChild(visual);

        Control controlRoot = new()
        {
            Name = "ControlRoot",
            LayoutMode = 3,
            OffsetLeft = 68.4205f,
            OffsetTop = -9.21045f,
            OffsetRight = 68.4205f,
            OffsetBottom = -9.21045f
        };
        root.AddChild(controlRoot);

        NSelectionReticle reticle = PreloadManager.Cache
            .GetScene(SelectionReticlePath)
            .Instantiate<NSelectionReticle>(PackedScene.GenEditState.Disabled);
        reticle.Name = "SelectionReticle";
        reticle.UniqueNameInOwner = true;
        reticle.LayoutMode = 0;
        reticle.OffsetLeft = -294.629f;
        reticle.OffsetTop = -409.523f;
        reticle.OffsetRight = 100.371f;
        reticle.OffsetBottom = 280.477f;
        controlRoot.AddChild(reticle);

        Control hitbox = CreateAnchor("Hitbox", -302.629f, -410.523f, 102.371f, 284.477f);
        Control thoughtBubbleRight = CreateAnchor("ThoughtBubbleRight", 44.7365f, -313.155f, 44.7365f, -313.155f);
        Control thoughtBubbleLeft = CreateAnchor("ThoughtBubbleLeft", -218.419f, -315.787f, -218.419f, -315.787f);
        controlRoot.AddChild(hitbox);
        controlRoot.AddChild(thoughtBubbleRight);
        controlRoot.AddChild(thoughtBubbleLeft);

        __result = root;
        return false;
    }

    private static Control CreateAnchor(string name, float left, float top, float right, float bottom)
    {
        return new Control
        {
            Name = name,
            UniqueNameInOwner = true,
            OffsetLeft = left,
            OffsetTop = top,
            OffsetRight = right,
            OffsetBottom = bottom
        };
    }

    private static Vector2[] CreateEllipsePolygon(float radiusX, float radiusY)
    {
        Vector2[] points = new Vector2[ShadowPointCount];
        for (int i = 0; i < ShadowPointCount; i++)
        {
            float angle = Mathf.Tau * i / ShadowPointCount;
            points[i] = new Vector2(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY);
        }

        return points;
    }
}
