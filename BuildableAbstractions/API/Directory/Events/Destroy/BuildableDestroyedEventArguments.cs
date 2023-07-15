using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Destroy;

public struct BuildableDestroyedEventArguments
{
    public Buildable Buildable { get; }

    public BuildableDestroyedEventArguments(Buildable buildable)
    {
        Buildable = buildable;
    }
}