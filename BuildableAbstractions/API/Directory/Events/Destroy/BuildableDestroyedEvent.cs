using JetBrains.Annotations;
using Pustalorc.Libraries.RocketModServices.Events.Implementations.Generics;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Destroy;

/// <inheritdoc />
/// <summary>
///     An event that is raised when a buildable is destroyed.
/// </summary>
[PublicAPI]
public class BuildableDestroyedEvent : Event<BuildableDestroyedEventArguments>;