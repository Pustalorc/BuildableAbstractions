using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Spawn;

public struct BuildableSpawnedEventArguments
{
    public Buildable Buildable { get; }

    public BuildableSpawnedEventArguments(Buildable buildable)
    {
        Buildable = buildable;
    }
}