using HarmonyLib;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Patches;

internal static class HarmonyInstance
{
    internal static Harmony Harmony { get; }

    static HarmonyInstance()
    {
        Harmony = new Harmony("com.pustalorc.libraries.buildableAbstractions");
        Harmony.PatchAll();
    }
}