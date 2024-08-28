namespace Pustalorc.Libraries.BuildableAbstractions.API.Patches.Delegates;

/// <summary>
///     A delegate that handles any notification about a buildable changing from nelson's code (being removed,
///     transformed).
/// </summary>
/// <param name="instanceId">The instance ID of the buildable.</param>
public delegate void NelsonBuildableChange(uint instanceId);