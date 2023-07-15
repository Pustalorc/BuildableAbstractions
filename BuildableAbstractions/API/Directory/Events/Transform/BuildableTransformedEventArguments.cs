using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Transform;

/// <summary>
///     The arguments for <see cref="BuildableTransformedEvent" />
/// </summary>
[PublicAPI]
public struct BuildableTransformedEventArguments
{
    /// <summary>
    ///     The affected <see cref="Buildable" /> when the event was raised.
    /// </summary>
    public Buildable Buildable { get; }

    /// <summary>
    ///     Creates an instance of the arguments.
    /// </summary>
    /// <param name="buildable">The <see cref="Buildable" /> affected in the event.</param>
    public BuildableTransformedEventArguments(Buildable buildable)
    {
        Buildable = buildable;
    }
}