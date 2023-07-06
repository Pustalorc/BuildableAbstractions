using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Patches;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Extensions;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;

/// <inheritdoc />
[PublicAPI]
public class DefaultBuildableDirectory : IBuildableDirectory
{
    /// <inheritdoc />
    public int BuildableCount => Buildables.Count;

    /// <summary>
    ///     All the buildables in the game currently.
    /// </summary>
    protected List<Buildable> Buildables { get; }

    /// <summary>
    ///     All the barricade buildables, but indexed by Nelson's InstanceId.
    /// </summary>
    /// <remarks>
    ///     This cannot be merged into one dictionary with <see cref="StructureBuildable" />s as nelson does not have unique
    ///     instance ids between both.
    /// </remarks>
    protected Dictionary<uint, BarricadeBuildable> InstanceIdIndexedBarricades { get; }

    /// <summary>
    ///     All the structure buildables, but indexed by Nelson's InstanceId.
    /// </summary>
    /// <remarks>
    ///     This cannot be merged into one dictionary with <see cref="BarricadeBuildable" />s as nelson does not have unique
    ///     instance ids between both.
    /// </remarks>
    protected Dictionary<uint, StructureBuildable> InstanceIdIndexedStructures { get; }

    /// <summary>
    ///     All the buildables, but indexed by Unity's <see cref="Transform" />.
    /// </summary>
    protected Dictionary<Transform, Buildable> TransformIndexedBuildables { get; }

    /// <summary>
    ///     The constructor for the directory.
    /// </summary>
    /// <remarks>
    ///     This constructor hooks onto Level.onPostLevelLoaded.
    ///     If you inherit this class, and you instantiate it when the server is fully loaded,
    ///     you will have to manually call LevelLoaded(0) yourself, as the event will not fire again.
    /// </remarks>
    public DefaultBuildableDirectory()
    {
        Buildables = new List<Buildable>();
        InstanceIdIndexedBarricades = new Dictionary<uint, BarricadeBuildable>();
        InstanceIdIndexedStructures = new Dictionary<uint, StructureBuildable>();
        TransformIndexedBuildables = new Dictionary<Transform, Buildable>();

        if (Level.isLoaded)
            // ReSharper disable once VirtualMemberCallInConstructor
            // Force call in case that this class is instantiated post-level load.
            // This should not be needed, but is there just in-case.
            // For implementations overriding this class, they should still call this overriden method this way to avoid issues.
            LevelLoaded(0);
        else
            Level.onPostLevelLoaded += LevelLoaded;

        StructureManager.onStructureSpawned += StructureSpawned;
        BarricadeManager.onBarricadeSpawned += BarricadeSpawned;
        PatchBuildablesDestroy.OnStructureDestroyed += StructureDestroyed;
        PatchBuildablesDestroy.OnBarricadeDestroyed += BarricadeDestroyed;
        PatchBuildableTransforms.OnStructureTransformed += StructureTransformed;
        PatchBuildableTransforms.OnBarricadeTransformed += BarricadeTransformed;
    }

    /// <inheritdoc />
    public virtual IEnumerable<T> GetBuildables<T>(GetBuildableOptions options = default) where T : Buildable
    {
        return Buildables.OfType<T>().Filter(options);
    }

    /// <inheritdoc />
    public virtual T? GetBuildable<T>(Transform transform) where T : Buildable
    {
        return TransformIndexedBuildables.TryGetValue(transform, out var buildable) ? buildable as T : null;
    }

    /// <inheritdoc />
    public virtual T? GetBuildable<T>(uint instanceId) where T : Buildable
    {
        var typeT = typeof(T);
        if (typeT == typeof(BarricadeBuildable))
            return InstanceIdIndexedBarricades.TryGetValue(instanceId, out var buildable) ? buildable as T : null;

        if (typeT == typeof(StructureBuildable))
            return InstanceIdIndexedStructures.TryGetValue(instanceId, out var buildable) ? buildable as T : null;


        if (InstanceIdIndexedBarricades.TryGetValue(instanceId, out var barricadeBuildable))
            return barricadeBuildable as T;

        if (InstanceIdIndexedStructures.TryGetValue(instanceId, out var structureBuildable))
            return structureBuildable as T;

        return null;
    }

    /// <summary>
    ///     Method that hooks onto the LevelLoaded event.
    /// </summary>
    protected virtual void LevelLoaded(int id)
    {
        var barricadeRegions = BarricadeManager.regions.Cast<BarricadeRegion>().Concat(BarricadeManager.vehicleRegions)
            .ToList();

        foreach (var region in barricadeRegions)
        foreach (var drop in region.drops)
            BarricadeSpawned(region, drop);

        var structureRegions = StructureManager.regions.Cast<StructureRegion>().ToList();

        foreach (var region in structureRegions)
        foreach (var drop in region.drops)
            StructureSpawned(region, drop);
    }

    /// <summary>
    ///     Event hooked onto the patches for nelson's code in order to know when a structure has been destroyed.
    /// </summary>
    /// <param name="instanceId">The instance id of the structure destroyed.</param>
    protected virtual void StructureDestroyed(uint instanceId)
    {
        if (!InstanceIdIndexedStructures.TryGetValue(instanceId, out var buildable) ||
            !InstanceIdIndexedStructures.Remove(instanceId))
            return;

        BuildableDirectory.RaiseBuildableDestroyed(buildable);
    }

    /// <summary>
    ///     Event hooked onto the patches for nelson's code in order to know when a barricade has been destroyed.
    /// </summary>
    /// <param name="instanceId">The instance id of the barricade destroyed.</param>
    protected virtual void BarricadeDestroyed(uint instanceId)
    {
        if (!InstanceIdIndexedBarricades.TryGetValue(instanceId, out var buildable) ||
            !InstanceIdIndexedBarricades.Remove(instanceId))
            return;

        BuildableDirectory.RaiseBuildableDestroyed(buildable);
    }

    /// <summary>
    ///     Event hooked onto the patches for nelson's code in order to know when a structure has been transformed.
    /// </summary>
    /// <param name="instanceId">The instance id of the structure transformed.</param>
    protected virtual void StructureTransformed(uint instanceId)
    {
        if (!InstanceIdIndexedStructures.TryGetValue(instanceId, out var buildable))
            return;

        BuildableDirectory.RaiseBuildableTransformed(buildable);
    }

    /// <summary>
    ///     Event hooked onto the patches for nelson's code in order to know when a barricade has been transformed.
    /// </summary>
    /// <param name="instanceId">The instance id of the barricade transformed.</param>
    protected virtual void BarricadeTransformed(uint instanceId)
    {
        if (!InstanceIdIndexedBarricades.TryGetValue(instanceId, out var buildable))
            return;

        BuildableDirectory.RaiseBuildableTransformed(buildable);
    }

    /// <summary>
    ///     Event hooked onto nelson's code to know when a new structure has been spawned.
    /// </summary>
    /// <param name="region">The region the structure was spawned in.</param>
    /// <param name="drop">The structure that was spawned in.</param>
    protected virtual void StructureSpawned(StructureRegion region, StructureDrop drop)
    {
        var buildable = new StructureBuildable(drop);

        if (!InstanceIdIndexedStructures.ContainsKey(buildable.InstanceId))
            InstanceIdIndexedStructures.Add(buildable.InstanceId, buildable);

        if (!TransformIndexedBuildables.TryGetValue(buildable.Model, out var storedBuild))
            TransformIndexedBuildables.Add(buildable.Model, buildable);
        else
            Logger.LogWarning(
                $"Warning! Buildable model already indexed! Is unity being weird again? Stored model: {buildable.Model}. Stored buildable: {storedBuild}");

        BuildableDirectory.RaiseBuildableSpawned(buildable);
    }

    /// <summary>
    ///     Event hooked onto nelson's code to know when a new barricade has been spawned.
    /// </summary>
    /// <param name="region">The region the barricade was spawned in.</param>
    /// <param name="drop">The barricade that was spawned in.</param>
    protected virtual void BarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
    {
        var buildable = new BarricadeBuildable(drop);

        if (!InstanceIdIndexedStructures.ContainsKey(buildable.InstanceId))
            InstanceIdIndexedBarricades.Add(buildable.InstanceId, buildable);

        if (!TransformIndexedBuildables.TryGetValue(buildable.Model, out var storedBuild))
            TransformIndexedBuildables.Add(buildable.Model, buildable);
        else
            Logger.LogWarning(
                $"Warning! Buildable model already indexed! Is unity being weird again? Stored model: {buildable.Model}. Stored buildable: {storedBuild}");

        BuildableDirectory.RaiseBuildableSpawned(buildable);
    }
}