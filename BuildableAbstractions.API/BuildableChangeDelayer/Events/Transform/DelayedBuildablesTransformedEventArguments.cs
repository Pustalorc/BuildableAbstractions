using System.Collections.Generic;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Transform;

/// <summary>
///     The arguments for <see cref="DelayedBuildablesTransformedEvent" />
/// </summary>
[PublicAPI]
public struct DelayedBuildablesTransformedEventArguments
{
    /// <summary>
    ///     The affected <see cref="Buildable" />s when the event was raised.
    /// </summary>
    public List<Buildable> Buildables { get; }

    /// <summary>
    ///     Creates an instance of the arguments.
    /// </summary>
    /// <param name="buildables">The <see cref="Buildable" />s affected in the event.</param>
    public DelayedBuildablesTransformedEventArguments(List<Buildable> buildables)
    {
        Buildables = buildables;
    }
}