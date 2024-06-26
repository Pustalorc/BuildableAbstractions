using System.Collections.Generic;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Destroy;

/// <summary>
///     The arguments for <see cref="DelayedBuildablesDestroyedEvent" />
/// </summary>
[PublicAPI]
public struct DelayedBuildablesDestroyedEventArguments
{
    /// <summary>
    ///     The affected <see cref="Buildable" />s when the event was raised.
    /// </summary>
    public List<Buildable> Buildables { get; }

    /// <summary>
    ///     Creates an instance of the arguments.
    /// </summary>
    /// <param name="buildables">The <see cref="Buildable" />s affected in the event.</param>
    public DelayedBuildablesDestroyedEventArguments(List<Buildable> buildables)
    {
        Buildables = buildables;
    }
}