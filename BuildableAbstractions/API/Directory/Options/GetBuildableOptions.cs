using JetBrains.Annotations;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;

[PublicAPI]
public readonly struct GetBuildableOptions
{
    public ulong Owner { get; }
    public ulong Group { get; }
    public bool IncludeOnVehicles { get; }

    public GetBuildableOptions(ulong owner = default, ulong group = default, bool includeOnVehicles = true)
    {
        Owner = owner;
        Group = group;
        IncludeOnVehicles = includeOnVehicles;
    }
}