using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Delegates;

/// <summary>
///     A delegate that handles any notification about a buildable changing (being added, removed and transformed).
/// </summary>
/// <param name="buildable">The affected <see cref="Buildable" />.</param>
public delegate void BuildableChange(Buildable buildable);