using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Constants;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Destroy;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Spawn;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Transform;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Extensions;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using Pustalorc.Libraries.BuildableAbstractions.API.Patches;
using Pustalorc.Libraries.Logging.API.Loggers.Configuration.Interfaces;
using Pustalorc.Libraries.Logging.API.LogLevels.Implementations;
using Pustalorc.Libraries.Logging.API.Manager;
using Pustalorc.Libraries.RocketModServices.Events.Bus;
using SDG.Unturned;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;

/// <inheritdoc cref="Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces.IBuildableDirectory" />
[PublicAPI]
public class DefaultBuildableDirectory : BuildableChangeListenerWithDelayedFire, IBuildableDirectory
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

    /// <inheritdoc />
    public DefaultBuildableDirectory()
    {
        Buildables = new List<Buildable>();
        InstanceIdIndexedBarricades = new Dictionary<uint, BarricadeBuildable>();
        InstanceIdIndexedStructures = new Dictionary<uint, StructureBuildable>();
        TransformIndexedBuildables = new Dictionary<Transform, Buildable>();
        LogManager.UpdateConfiguration(new LogConfiguration());
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

    /// <inheritdoc />
    public virtual void Load()
    {
        if (Level.isLoaded)
            LevelLoaded(0);
        else
            Level.onLevelLoaded += LevelLoaded;
    }

    /// <inheritdoc />
    public virtual void Unload()
    {
        Level.onLevelLoaded -= LevelLoaded;
        UnhookFromEvents();
    }

    /// <summary>
    ///     Method that hooks onto the LevelLoaded event.
    /// </summary>
    protected virtual void LevelLoaded(int id)
    {
        const double logPercentage = 0.085;

        LogManager.Debug(LoggingConstants.LevelLoadedHookingOntoEvents);
        HookToEvents();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        LogManager.Debug(LoggingConstants.LoadingBarricades);
        var barricadeRegions = BarricadeManager.regions.Cast<BarricadeRegion>().Concat(BarricadeManager.vehicleRegions)
            .ToList();

        var regionCount = barricadeRegions.Count;
        var regionLogRate = Math.Floor(regionCount * logPercentage);

        LogManager.Information(string.Format(LoggingConstants.BarricadeLoadProgress, 0, 0, regionCount,
            stopwatch.ElapsedMilliseconds));

        for (var regionIndex = 0; regionIndex < regionCount; regionIndex++)
        {
            var position = regionIndex + 1;
            var region = barricadeRegions[regionIndex];

            foreach (var drop in region.drops)
                BarricadeSpawned(region, drop);

            if (position % regionLogRate != 0)
                continue;

            var percentageCompleted = Math.Ceiling(position / (double)regionCount * 100);
            LogManager.Information(string.Format(LoggingConstants.BarricadeLoadProgress, percentageCompleted, position,
                regionCount, stopwatch.ElapsedMilliseconds));
        }

        LogManager.Information(string.Format(LoggingConstants.BarricadeLoadProgress, 100, regionCount, regionCount, stopwatch.ElapsedMilliseconds));

        LogManager.Debug(LoggingConstants.LoadingStructures);
        var structureRegions = StructureManager.regions.Cast<StructureRegion>().ToList();

        regionCount = structureRegions.Count;
        regionLogRate = Math.Floor(regionCount * logPercentage);

        LogManager.Information(string.Format(LoggingConstants.StructureLoadProgress, 0, 0, regionCount,
            stopwatch.ElapsedMilliseconds));

        for (var regionIndex = 0; regionIndex < regionCount; regionIndex++)
        {
            var position = regionIndex + 1;
            var region = structureRegions[regionIndex];

            foreach (var drop in region.drops)
                StructureSpawned(region, drop);

            if (position % regionLogRate != 0)
                continue;

            var percentageCompleted = Math.Ceiling(position / (double)regionCount * 100);
            LogManager.Information(string.Format(LoggingConstants.StructureLoadProgress, percentageCompleted, position,
                regionCount, stopwatch.ElapsedMilliseconds));
        }

        LogManager.Information(string.Format(LoggingConstants.StructureLoadProgress, 100, regionCount, regionCount, stopwatch.ElapsedMilliseconds));
        LogManager.Information(string.Format(LoggingConstants.BuildableLoadFinished, Buildables.Count));
    }

    /// <inheritdoc />
    public override void HookToEvents()
    {
        base.HookToEvents();

        StructureManager.onStructureSpawned += StructureSpawned;
        BarricadeManager.onBarricadeSpawned += BarricadeSpawned;
        PatchBuildablesDestroy.OnStructureDestroyed += StructureDestroyed;
        PatchBuildablesDestroy.OnBarricadeDestroyed += BarricadeDestroyed;
        PatchBuildableTransforms.OnStructureTransformed += StructureTransformed;
        PatchBuildableTransforms.OnBarricadeTransformed += BarricadeTransformed;
    }

    /// <inheritdoc />
    public override void UnhookFromEvents()
    {
        PatchBuildableTransforms.OnBarricadeTransformed -= BarricadeTransformed;
        PatchBuildableTransforms.OnStructureTransformed -= StructureTransformed;
        PatchBuildablesDestroy.OnBarricadeDestroyed -= BarricadeDestroyed;
        PatchBuildablesDestroy.OnStructureDestroyed -= StructureDestroyed;
        BarricadeManager.onBarricadeSpawned -= BarricadeSpawned;
        StructureManager.onStructureSpawned -= StructureSpawned;

        base.UnhookFromEvents();
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

        EventBus.Publish<BuildableDestroyedEvent>(new BuildableDestroyedEventArguments(buildable));
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

        EventBus.Publish<BuildableDestroyedEvent>(new BuildableDestroyedEventArguments(buildable));
    }

    /// <summary>
    ///     Event hooked onto the patches for nelson's code in order to know when a structure has been transformed.
    /// </summary>
    /// <param name="instanceId">The instance id of the structure transformed.</param>
    protected virtual void StructureTransformed(uint instanceId)
    {
        if (!InstanceIdIndexedStructures.TryGetValue(instanceId, out var buildable))
            return;

        EventBus.Publish<BuildableTransformedEvent>(new BuildableTransformedEventArguments(buildable));
    }

    /// <summary>
    ///     Event hooked onto the patches for nelson's code in order to know when a barricade has been transformed.
    /// </summary>
    /// <param name="instanceId">The instance id of the barricade transformed.</param>
    protected virtual void BarricadeTransformed(uint instanceId)
    {
        if (!InstanceIdIndexedBarricades.TryGetValue(instanceId, out var buildable))
            return;

        EventBus.Publish<BuildableTransformedEvent>(new BuildableTransformedEventArguments(buildable));
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
            LogManager.Warning(
                $"Warning! Buildable model already indexed! Is unity being weird again? Stored model: {buildable.Model}. Stored buildable: {storedBuild}");

        Buildables.Add(buildable);
        EventBus.Publish<BuildableSpawnedEvent>(new BuildableSpawnedEventArguments(buildable));
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
            LogManager.Warning(
                $"Warning! Buildable model already indexed! Is unity being weird again? Stored model: {buildable.Model}. Stored buildable: {storedBuild}");

        Buildables.Add(buildable);
        EventBus.Publish<BuildableSpawnedEvent>(new BuildableSpawnedEventArguments(buildable));
    }

    private class LogConfiguration : ILoggerConfiguration
    {
        public byte MaxLogLevel => LogLevel.Debug.Level;
    }
}