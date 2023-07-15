using JetBrains.Annotations;
using Pustalorc.Libraries.RocketModServices.Events.Implementations.Generics;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Transform;

/// <inheritdoc />
/// <summary>
///     An event that is raised when a buildable is transformed.
/// </summary>
[PublicAPI]
public class BuildableTransformedEvent : Event<BuildableTransformedEventArguments>
{
}