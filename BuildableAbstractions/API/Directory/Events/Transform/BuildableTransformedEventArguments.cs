using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Events.Transform;

public struct BuildableTransformedEventArguments
{
    public Buildable Buildable { get; }

    public BuildableTransformedEventArguments(Buildable buildable)
    {
        Buildable = buildable;
    }
}