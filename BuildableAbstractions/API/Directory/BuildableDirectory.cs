using HarmonyLib;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Delegates;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory;

[PublicAPI]
public static class BuildableDirectory
{
    public static event BuildableChange? OnBuildableDestroyed;

    public static event BuildableChange? OnBuildableSpawned;
    public static event BuildableChange? OnBuildableTransformed;
    private static Harmony Harmony { get; }

    public static IBuildableDirectory Instance { get; set; }


    static BuildableDirectory()
    {
        Harmony = new Harmony("com.pustalorc.libraries.buildableAbstractions");
        Harmony.PatchAll();

        Instance = new DefaultBuildableDirectory();
    }

    public static void RaiseBuildableSpawned(Buildable buildable)
    {
        OnBuildableSpawned?.Invoke(buildable);
    }

    public static void RaiseBuildableDestroyed(Buildable buildable)
    {
        OnBuildableDestroyed?.Invoke(buildable);
    }

    public static void RaiseBuildableTransformed(Buildable buildable)
    {
        OnBuildableTransformed?.Invoke(buildable);
    }
}