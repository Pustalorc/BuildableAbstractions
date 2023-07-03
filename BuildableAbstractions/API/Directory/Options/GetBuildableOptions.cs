using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;

[PublicAPI]
public readonly struct GetBuildableOptions
{
    public ulong Owner { get; }
    public ulong Group { get; }
    public bool IncludeOnVehicles { get; }

    public float MaxRange { get; }
    public float MinRange { get; }
    public Vector3? Position { get; }

    public HashSet<ushort> Assets { get; }

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