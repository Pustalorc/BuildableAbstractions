using JetBrains.Annotations;
using Pustalorc.Libraries.RocketModServices.Events.Implementations.Generics;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Spawn;

/// <inheritdoc />
/// <summary>
///     An event that is raised a period of time after the last buildable was spawned, with all the buildables that were
///     spawned since the first spawn.
/// </summary>
[PublicAPI]
public class DelayedBuildablesSpawnedEvent : Event<DelayedBuildablesSpawnedEventArguments>;