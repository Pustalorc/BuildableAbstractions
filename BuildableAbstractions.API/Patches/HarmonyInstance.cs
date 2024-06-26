using HarmonyLib;
using Pustalorc.Libraries.Logging.API.Manager;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Patches;

internal static class HarmonyInstance
{
    internal static Harmony Harmony { get; }

    static HarmonyInstance()
    {
        LogManager.Debug("Applying harmony patches...");
        Harmony = new Harmony("com.pustalorc.libraries.buildableAbstractions.api");
        Harmony.PatchAll();
        LogManager.Information("Harmony patches are applied.");
    }
}