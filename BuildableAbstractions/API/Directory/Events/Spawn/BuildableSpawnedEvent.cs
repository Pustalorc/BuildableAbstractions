using JetBrains.Annotations;
using Pustalorc.Libraries.RocketModServices.Events.Implementations.Generics;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Spawn;

/// <inheritdoc />
/// <summary>
///     An event that is raised when a buildable is spawned.
/// </summary>
[PublicAPI]
public class BuildableSpawnedEvent : Event<BuildableSpawnedEventArguments>;