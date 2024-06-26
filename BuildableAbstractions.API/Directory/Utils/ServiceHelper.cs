extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.RocketModServices.Services;

namespace Pustalorc.Libraries.BuildableAbstractions.API.Directory.Utils;

/// <summary>
/// A helper static class to get the server or use the default.
/// </summary>
/// <remarks>
/// Recommended to try this method at the start of the program, but it is optional.
/// </remarks>
[PublicAPI]
public static class ServiceHelper
{
    /// <summary>
    /// Gets the <see cref="IBuildableDirectory"/> service currently in use. If none is in use, <see cref="DefaultBuildableDirectory"/> will be instantiated and registered.
    /// </summary>
    /// <returns>The current <see cref="IBuildableDirectory" /> registered service.</returns>
    public static IBuildableDirectory GetServiceOrUseDefault()
    {
        var service = RocketModService<IBuildableDirectory>.TryGetService();

        if (service != null)
            return service;

        service = new DefaultBuildableDirectory();
        RocketModService<IBuildableDirectory>.RegisterService(service);

        return service;
    }
}