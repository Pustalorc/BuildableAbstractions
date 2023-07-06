using HarmonyLib;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Delegates;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory;

/// <summary>
///     A static class that is initialized statically with a fully loaded and running <see cref="IBuildableDirectory" />.
///     <br />
///     Also runs patches that are necessary for some events that Nelson never added.
/// </summary>
[PublicAPI]
public static class BuildableDirectory
{
    /// <summary>
    ///     An event that is raised when a <see cref="Buildable" /> gets destroyed.
    /// </summary>
    public static event BuildableChange? OnBuildableDestroyed;

    /// <summary>
    ///     An event that is raised when a <see cref="Buildable" /> is spawned.
    /// </summary>
    public static event BuildableChange? OnBuildableSpawned;

    /// <summary>
    ///     An event that is raised when a <see cref="Buildable" /> is transformed.
    /// </summary>
    public static event BuildableChange? OnBuildableTransformed;

    private static Harmony Harmony { get; }

    /// <summary>
    ///     The currently loaded <see cref="IBuildableDirectory" />.
    ///     <br />
    ///     Other plugins could replace the instance with their own <see cref="IBuildableDirectory" /> if necessary.
    /// </summary>
    public static IBuildableDirectory Instance { get; set; }

    static BuildableDirectory()
    {
        Harmony = new Harmony("com.pustalorc.libraries.buildableAbstractions");
        Harmony.PatchAll();

        Instance = new DefaultBuildableDirectory();
    }

    /// <summary>
    ///     Raises the <see cref="OnBuildableSpawned" /> event.
    /// </summary>
    /// <param name="buildable">The <see cref="Buildable" /> that was spawned.</param>
    public static void RaiseBuildableSpawned(Buildable buildable)
    {
        OnBuildableSpawned?.Invoke(buildable);
    }

    /// <summary>
    ///     Raises the <see cref="OnBuildableDestroyed" /> event.
    /// </summary>
    /// <param name="buildable">The <see cref="Buildable" /> that was destroyed.</param>
    public static void RaiseBuildableDestroyed(Buildable buildable)
    {
        OnBuildableDestroyed?.Invoke(buildable);
    }

    /// <summary>
    ///     Raises the <see cref="OnBuildableTransformed" /> event.
    /// </summary>
    /// <param name="buildable">The <see cref="Buildable" /> that was transformed.</param>
    public static void RaiseBuildableTransformed(Buildable buildable)
    {
        OnBuildableTransformed?.Invoke(buildable);
    }
}