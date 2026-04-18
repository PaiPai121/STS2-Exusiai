using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace MyFirstMod.Code;

[ModInitializer(nameof(Init))]
public partial class Entry : Node
{
    public const string ModId = "myfirstmod";

    public static void Init()
    {
        GD.Print($"[{ModId}] Initializing...");

        Harmony harmony = new(ModId);
        harmony.PatchAll();

        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);

        GD.Print($"[{ModId}] Initialized successfully!");
    }
}
