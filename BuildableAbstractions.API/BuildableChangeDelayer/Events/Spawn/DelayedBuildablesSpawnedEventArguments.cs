using System.Collections.Generic;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Spawn;

/// <summary>
///     The arguments for <see cref="DelayedBuildablesSpawnedEvent" />
/// </summary>
[PublicAPI]
public struct DelayedBuildablesSpawnedEventArguments
{
    /// <summary>
    ///     The affected <see cref="Buildable" />s when the event was raised.
    /// </summary>
    public List<Buildable> Buildables { get; }

    /// <summary>
    ///     Creates an instance of the arguments.
    /// </summary>
    /// <param name="buildables">The <see cref="Buildable" />s affected in the event.</param>
    public DelayedBuildablesSpawnedEventArguments(List<Buildable> buildables)
    {
        Buildables = buildables;
    }
}