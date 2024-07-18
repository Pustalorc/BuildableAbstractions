using HarmonyLib;
using Pustalorc.Libraries.BuildableAbstractions.API.Patches.Constants;
using Pustalorc.Libraries.Logging.Manager;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Patches;

internal static class HarmonyInstance
{
    internal static Harmony Harmony { get; }

    static HarmonyInstance()
    {
        LogManager.Debug(LoggingConstants.ApplyingHarmonyPatch);
        Harmony = new Harmony("com.pustalorc.libraries.buildableAbstractions.api");
        Harmony.PatchAll();
        LogManager.Information(LoggingConstants.AppliedHarmonyPatch);
    }
}