using HarmonyLib;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Delegates;
using SDG.Unturned;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Patches;

/// <summary>
///     A patch for barricades and structures being destroyed so that appropriate events can be fired.
/// </summary>
[PublicAPI]
public static class PatchBuildablesDestroy
{
    public static event NelsonBuildableChange? OnBarricadeDestroyed;
    public static event NelsonBuildableChange? OnStructureDestroyed;

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