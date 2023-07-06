using System.Collections.Generic;
using JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;

/// <summary>
///     A struct for all options to filter when getting one or more <see cref="Buildable" />s.
/// </summary>
[PublicAPI]
public readonly struct GetBuildableOptions
{
    /// <summary>
    ///     The owner to filter by. This should be the Steam64ID of the owner.
    /// </summary>
    public ulong Owner { get; }

    /// <summary>
    ///     The group to filter by. This should be the Steam64ID of the group.
    /// </summary>
    public ulong Group { get; }

    /// <summary>
    ///     If <see cref="Buildable" />s on vehicles should be included.
    /// </summary>
    public bool IncludeOnVehicles { get; }

    /// <summary>
    ///     The maximum range to filter by. Any <see cref="Buildable" />s farther this range won't be included.
    /// </summary>
    /// <remarks>
    ///     Requires <see cref="Position" /> to be set.
    /// </remarks>
    public float MaxRange { get; }

    /// <summary>
    ///     The minimum range to filter by. Any <see cref="Buildable" />s closer this range won't be included.
    /// </summary>
    /// <remarks>
    ///     Requires <see cref="Position" /> to be set.
    /// </remarks>
    public float MinRange { get; }

    /// <summary>
    ///     The position to use when filtering with <see cref="MaxRange" /> and/or <see cref="MinRange" />.
    /// </summary>
    public Vector3? Position { get; }

    /// <summary>
    ///     A <see cref="HashSet{T}" /> of asset ids to filter by.
    /// </summary>
    public HashSet<ushort> Assets { get; }

    /// <summary>
    ///     Creates an instance of the options with the default values.
    /// </summary>
    /// <param name="owner">The owner to filter by. This should be the Steam64ID of the owner.</param>
    /// <param name="group">The group to filter by. This should be the Steam64ID of the group.</param>
    /// <param name="includeOnVehicles">If <see cref="Buildable" />s on vehicles should be included.</param>
    /// <param name="maxRange">
    ///     The maximum range to filter by. Any <see cref="Buildable" />s farther this range won't be
    ///     included.
    /// </param>
    /// <param name="minRange">
    ///     The minimum range to filter by. Any <see cref="Buildable" />s closer this range won't be
    ///     included.
    /// </param>
    /// <param name="position">The position to use when filtering with <see cref="MaxRange" /> and/or <see cref="MinRange" />.</param>
    /// <param name="assets">A <see cref="HashSet{T}" /> of asset ids to filter by.</param>
    /// <remarks>
    ///     <paramref name="maxRange" /> and <paramref name="minRange" /> require <see cref="Position" /> to be set.
    /// </remarks>
    public GetBuildableOptions(ulong owner = default, ulong group = default, bool includeOnVehicles = true,
        float maxRange = float.MaxValue, float minRange = float.MinValue, Vector3? position = default,
        HashSet<ushort>? assets = default)
    {
        Owner = owner;
        Group = group;
        IncludeOnVehicles = includeOnVehicles;

        MaxRange = Mathf.Max(minRange, maxRange);
        MinRange = Mathf.Min(minRange, maxRange);
        Position = position;

        Assets = assets ?? new HashSet<ushort>();
    }
}