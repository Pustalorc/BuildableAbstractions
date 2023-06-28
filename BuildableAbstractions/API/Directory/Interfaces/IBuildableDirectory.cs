using System.Collections.Generic;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;

public interface IBuildableDirectory
{
    public IEnumerable<T> GetBuildables<T>(GetBuildableOptions options = default) where T : Buildable;

    public T? GetBuildable<T>(Transform buildable) where T : Buildable;

    public T? GetBuildable<T>(uint instanceId) where T : Buildable;
}