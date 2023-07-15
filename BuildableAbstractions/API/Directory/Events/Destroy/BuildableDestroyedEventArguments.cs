using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Destroy;

/// <summary>
///     The arguments for <see cref="BuildableDestroyedEvent" />
/// </summary>
[PublicAPI]
public struct BuildableDestroyedEventArguments
{
    /// <summary>
    ///     The affected <see cref="Buildable" /> when the event was raised.
    /// </summary>
    public Buildable Buildable { get; }

    /// <summary>
    ///     Creates an instance of the arguments.
    /// </summary>
    /// <param name="buildable">The <see cref="Buildable" /> affected in the event.</param>
    public BuildableDestroyedEventArguments(Buildable buildable)
    {
        Buildable = buildable;
    }
}