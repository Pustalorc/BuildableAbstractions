using HarmonyLib;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Patches.Delegates;
using SDG.Unturned;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Patches;

/// <summary>
///     A patch for barricades and structures being destroyed so that appropriate events can be fired.
/// </summary>
[PublicAPI]
public static class PatchBuildablesDestroy
{
    /// <summary>
    ///     An event that is raised when a barricade is destroyed.
    /// </summary>
    public static event NelsonBuildableChange? OnBarricadeDestroyed;

    /// <summary>
    ///     An event that is raised when a structure is destroyed.
    /// </summary>
    public static event NelsonBuildableChange? OnStructureDestroyed;

    static PatchBuildablesDestroy()
    {
        _ = HarmonyInstance.Harmony;
    }

    [HarmonyPatch]
    internal static class InternalPatches
    {
        [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveDestroyBarricade))]
        [HarmonyPrefix]
        [UsedImplicitly]
        private static void BarricadeDestroyed(NetId netId)
        {
            var barricade = NetIdRegistry.Get<BarricadeDrop>(netId);

            if (barricade == null)
                return;

            OnBarricadeDestroyed?.Invoke(barricade.instanceID);
        }

        [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveDestroyStructure))]
        [HarmonyPrefix]
        [UsedImplicitly]
        private static void StructureDestroyed(NetId netId)
        {
            var structure = NetIdRegistry.Get<StructureDrop>(netId);

            if (structure == null)
                return;

            OnStructureDestroyed?.Invoke(structure.instanceID);
        }
    }
}