using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Spawn;

/// <summary>
///     The arguments for <see cref="BuildableSpawnedEvent" />
/// </summary>
[PublicAPI]
public struct BuildableSpawnedEventArguments
{
    /// <summary>
    ///     The affected <see cref="Buildable" /> when the event was raised.
    /// </summary>
    public Buildable Buildable { get; }

    /// <summary>
    ///     Creates an instance of the arguments.
    /// </summary>
    /// <param name="buildable">The <see cref="Buildable" /> affected in the event.</param>
    public BuildableSpawnedEventArguments(Buildable buildable)
    {
        Buildable = buildable;
    }
}