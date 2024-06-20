using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Extensions;

/// <summary>
///     Extension class for buildable directories.
/// </summary>
[PublicAPI]
public static class EnumerableBuildableExtensions
{
    private const float Tolerance = 0.1f;

    /// <summary>
    ///     Filters an <see cref="IEnumerable{T}" /> of <see cref="Buildable" />s with <see cref="GetBuildableOptions" />.
    /// </summary>
    /// <param name="buildables">The buildables to filter</param>
    /// <param name="options">The options to apply when filtering</param>
    /// <typeparam name="T">The type of buildable that the IEnumerable uses</typeparam>
    /// <returns>A new IEnumerable with filters applied</returns>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> buildables, GetBuildableOptions options = default)
        where T : Buildable
    {
        if (!options.IncludeOnVehicles)
            buildables = buildables.Where(static buildable => !buildable.IsPlanted);

        if (options.Owner != default)
            buildables = buildables.Where(buildable => buildable.Owner == options.Owner);

        if (options.Group != default)
            buildables = buildables.Where(buildable => buildable.Group == options.Group);

        if (Math.Abs(options.MinRange - float.MinValue) > Tolerance && options.Position.HasValue)
            buildables = buildables.Where(buildable =>
                (buildable.Position - options.Position.Value).sqrMagnitude >= options.MinRange);

        if (Math.Abs(options.MaxRange - float.MaxValue) > Tolerance && options.Position.HasValue)
            buildables = buildables.Where(buildable =>
                (buildable.Position - options.Position.Value).sqrMagnitude <= options.MaxRange);

        if (options.Assets.Count > 0)
            buildables = buildables.Where(buildable => options.Assets.Contains(buildable.AssetId));

        return buildables;
    }
}