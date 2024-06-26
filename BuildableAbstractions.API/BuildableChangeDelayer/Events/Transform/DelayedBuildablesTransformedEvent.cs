using JetBrains.Annotations;
using Pustalorc.Libraries.RocketModServices.Events.Implementations.Generics;

namespace Pustalorc.Libraries.BuildableAbstractions.API.BuildableChangeDelayer.Events.Transform;

/// <inheritdoc />
/// <summary>
///     An event that is raised a period of time after the last buildable was transformed, with all the buildables that
///     were transformed since the first transformation
/// </summary>
[PublicAPI]
public class DelayedBuildablesTransformedEvent : Event<DelayedBuildablesTransformedEventArguments>;