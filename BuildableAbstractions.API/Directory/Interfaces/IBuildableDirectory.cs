using System.Collections.Generic;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using Pustalorc.Libraries.RocketModServices.Services.Interfaces;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;

/// <inheritdoc />
/// <summary>
///     An <see cref="T:Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces.IBuildableDirectory" /> should
///     handles all of nelson's events related to buildables,
///     hold a basic list of all buildables in the map, index them, and raise events for buildable changes.
/// </summary>
[PublicAPI]
public interface IBuildableDirectory : IService
{
    /// <summary>
    ///     The number of buildables tracked by the directory.
    /// </summary>
    public int BuildableCount { get; }

    /// <summary>
    ///     Gets all the buildables of the specified type and filters them with some options.
    /// </summary>
    /// <param name="options">The options to filter the results with.</param>
    /// <typeparam name="T">
    ///     The type of buildable you wish to retrieve.
    ///     Should be one of the following 3 options:
    ///     <br />
    ///     <see cref="Buildable" /> - For all buildables
    ///     <br />
    ///     <see cref="BarricadeBuildable" /> - For barricades only
    ///     <br />
    ///     <see cref="StructureBuildable" /> - For structures only
    /// </typeparam>
    /// <returns>An <see cref="IEnumerable{T}" /> with all the buildables found.</returns>
    public IEnumerable<T> GetBuildables<T>(GetBuildableOptions options = default) where T : Buildable;

    /// <summary>
    ///     Gets a <see cref="Buildable"/> of the specified type that has a specific transform.
    /// </summary>
    /// <param name="transform">The transform of the buildable</param>
    /// <typeparam name="T">
    ///     The type of buildable you wish to retrieve.
    ///     Should be one of the following 3 options:
    ///     <br />
    ///     <see cref="Buildable" /> - For all buildables
    ///     <br />
    ///     <see cref="BarricadeBuildable" /> - For barricades only
    ///     <br />
    ///     <see cref="StructureBuildable" /> - For structures only
    /// </typeparam>
    /// <returns>null if no buildable was found, otherwise the instance of that buildable.</returns>
    public T? GetBuildable<T>(Transform transform) where T : Buildable;

    /// <summary>
    ///     Gets a <see cref="Buildable"/> of the specified type that has a specific instance id.
    /// </summary>
    /// <param name="instanceId">The instance id of the buildable</param>
    /// <typeparam name="T">
    ///     The type of buildable you wish to retrieve.
    ///     Should be one of the following 3 options:
    ///     <br />
    ///     <see cref="Buildable" /> - For all buildables
    ///     <br />
    ///     <see cref="BarricadeBuildable" /> - For barricades only
    ///     <br />
    ///     <see cref="StructureBuildable" /> - For structures only
    /// </typeparam>
    /// <returns>null if no buildable was found, otherwise the instance of that buildable.</returns>
    public T? GetBuildable<T>(uint instanceId) where T : Buildable;
}