namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Constants;

internal static class LoggingConstants
{
    public const string LevelLoadedHookingOntoEvents = "Level loaded, hooking onto unturned events...";

    public const string LoadingBarricades = "Registering and tracking all barricades...";

    public const string BarricadeLoadProgress =
        "Loading Barricades from all regions on the map... {0}% [{1}/{2}] {3}ms";

    public const string LoadingStructures = "Registering and tracking all structures...";

    public const string StructureLoadProgress =
        "Loading Structures from all regions on the map... {0}% [{1}/{2}] {3}ms";

    public const string BuildableLoadFinished = "Finished loading a total of {0} buildables. Took {1}ms";
}