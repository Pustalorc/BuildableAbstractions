using JetBrains.Annotations;
using Pustalorc.Libraries.RocketModServices.Events.Implementations.Generics;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Destroy;

/// <inheritdoc />
/// <summary>
///     An event that is raised a period of time after the last buildable was destroyed, with all the buildables that were
///     destroyed since the first destruction
/// </summary>
[PublicAPI]
public class DelayedBuildablesDestroyedEvent : Event<DelayedBuildablesDestroyedEventArguments>;