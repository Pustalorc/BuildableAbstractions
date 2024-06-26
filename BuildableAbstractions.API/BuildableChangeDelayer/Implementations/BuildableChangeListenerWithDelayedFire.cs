extern alias JetBrainsAnnotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrainsAnnotations::JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Destroy;
using Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Spawn;
using Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Transform;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Destroy;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Spawn;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Transform;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.RocketModServices.Events.Bus;
using Pustalorc.Plugins.AsynchronousTaskDispatcher.Dispatcher;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Implementations;

/// <summary>
///     A simple utility that listens to <see cref="Buildable" /> change events from the <see cref="IBuildableDirectory" />
///     and defers + groups every change performed before raising it.
/// </summary>
[PublicAPI]
public class BuildableChangeListenerWithDelayedFire
{
    private int m_BuildableChangeDeferredTimeMilliseconds;
    private ConcurrentQueue<Buildable> DeferredTransformed { get; }
    private ConcurrentQueue<Buildable> DeferredDestroyed { get; }
    private ConcurrentQueue<Buildable> DeferredSpawned { get; }

    private Action? CancelQueuedTask { get; set; }

    /// <summary>
    ///     The amount of time
    /// </summary>
    public ushort BuildableChangeDeferredTimeMilliseconds
    {
        get => (ushort)Math.Min(Math.Max(m_BuildableChangeDeferredTimeMilliseconds, 1), 1000);
        set => m_BuildableChangeDeferredTimeMilliseconds = Math.Min(Math.Max((int)value, 1), 1000);
    }

    /// <summary>
    ///     Creates a new instance of the change listener with the specified configuration.
    /// </summary>
    public BuildableChangeListenerWithDelayedFire()
    {
        DeferredTransformed = new ConcurrentQueue<Buildable>();
        DeferredDestroyed = new ConcurrentQueue<Buildable>();
        DeferredSpawned = new ConcurrentQueue<Buildable>();
    }

    /// <summary>
    ///     Hooks onto the buildable events.
    /// </summary>
    public virtual void HookToEvents()
    {
        EventBus.Subscribe<BuildableSpawnedEvent>((object)BuildableSpawned);
        EventBus.Subscribe<BuildableDestroyedEvent>((object)BuildableDestroyed);
        EventBus.Subscribe<BuildableTransformedEvent>((object)BuildableTransformed);
    }

    /// <summary>
    ///     Unhooks from the buildable events.
    /// </summary>
    public virtual void UnhookFromEvents()
    {
        EventBus.Unsubscribe<BuildableTransformedEvent>((object)BuildableTransformed);
        EventBus.Unsubscribe<BuildableDestroyedEvent>((object)BuildableDestroyed);
        EventBus.Unsubscribe<BuildableSpawnedEvent>((object)BuildableSpawned);
    }

    private void BuildableTransformed(BuildableTransformedEventArguments eventArgs)
    {
        CancelQueuedTask?.Invoke();
        DeferredTransformed.Enqueue(eventArgs.Buildable);
        CancelQueuedTask = AsyncTaskDispatcher.QueueAnonymousTask(ProcessDeferredBuildableChanges,
            BuildableChangeDeferredTimeMilliseconds);
    }

    private void BuildableSpawned(BuildableSpawnedEventArguments eventArgs)
    {
        CancelQueuedTask?.Invoke();
        DeferredSpawned.Enqueue(eventArgs.Buildable);
        CancelQueuedTask = AsyncTaskDispatcher.QueueAnonymousTask(ProcessDeferredBuildableChanges,
            BuildableChangeDeferredTimeMilliseconds);
    }

    private void BuildableDestroyed(BuildableDestroyedEventArguments eventArgs)
    {
        CancelQueuedTask?.Invoke();
        DeferredDestroyed.Enqueue(eventArgs.Buildable);
        CancelQueuedTask = AsyncTaskDispatcher.QueueAnonymousTask(ProcessDeferredBuildableChanges,
            BuildableChangeDeferredTimeMilliseconds);
    }

    private Task ProcessDeferredBuildableChanges(CancellationToken arg)
    {
        var deferredSpawned = new List<Buildable>();
        while (DeferredSpawned.TryDequeue(out var element))
            deferredSpawned.Add(element);

        var deferredDestroyed = new List<Buildable>();
        while (DeferredDestroyed.TryDequeue(out var element))
            deferredDestroyed.Add(element);

        var deferredTransformed = new List<Buildable>();
        while (DeferredTransformed.TryDequeue(out var element))
            deferredTransformed.Add(element);

        EventBus.Publish<DelayedBuildablesTransformedEvent>(new DelayedBuildablesTransformedEventArguments(deferredTransformed));
        EventBus.Publish<DelayedBuildablesSpawnedEvent>(new DelayedBuildablesSpawnedEventArguments(deferredSpawned));
        EventBus.Publish<DelayedBuildablesDestroyedEvent>(new DelayedBuildablesDestroyedEventArguments(deferredDestroyed));
        return Task.CompletedTask;
    }
}