using HarmonyLib;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Delegates;
using SDG.Unturned;

// ReSharper disable InconsistentNaming
// Patches have inconsistent naming due to harmony rules.

namespace Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Patches;

/// <summary>
///     A patch for barricades and structures being transformed so that appropriate events can be fired.
/// </summary>
[PublicAPI]
public static class PatchBuildableTransforms
{
    /// <summary>
    ///     An event that is raised when a barricade is transformed.
    /// </summary>
    public static event NelsonBuildableChange? OnBarricadeTransformed;

    /// <summary>
    ///     An event that is raised when a structure is transformed.
    /// </summary>
    public static event NelsonBuildableChange? OnStructureTransformed;

    [HarmonyPatch]
    internal static class InternalPatches
    {
        [HarmonyPatch(typeof(BarricadeDrop), nameof(BarricadeDrop.ReceiveTransform))]
        [HarmonyPostfix]
        [UsedImplicitly]
        internal static void ReceiveTransformBarricade(BarricadeDrop __instance)
        {
            OnBarricadeTransformed?.Invoke(__instance.instanceID);
        }

        [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransform))]
        [HarmonyPostfix]
        [UsedImplicitly]
        internal static void ReceiveTransformStructure(StructureDrop __instance)
        {
            OnStructureTransformed?.Invoke(__instance.instanceID);
        }
    }
}