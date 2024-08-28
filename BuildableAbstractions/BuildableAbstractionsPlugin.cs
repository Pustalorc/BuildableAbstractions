using System.Collections.Generic;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.Logging.Manager;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Pustalorc.Libraries.RocketModCommandsExtended.Extensions;
using Pustalorc.Libraries.RocketModServices.Services;
using Pustalorc.Plugins.BuildableAbstractions.Commands.Actions;
using Pustalorc.Plugins.BuildableAbstractions.Commands.Information;
using Pustalorc.Plugins.BuildableAbstractions.Commands.Wreck;
using Pustalorc.Plugins.BuildableAbstractions.Constants;
using Rocket.Core.Plugins;

namespace Pustalorc.Plugins.BuildableAbstractions;

/// <inheritdoc />
public sealed class BuildableAbstractionsPlugin : RocketPlugin
{
    private List<MultiThreadedRocketCommand> Commands { get; }

    /// <inheritdoc />
    public BuildableAbstractionsPlugin()
    {
        var translations = this.GetCurrentTranslationsForCommands();

        Commands =
        [
            new FindBuildsCommand(translations),
            new RemoveBuildableCommand(translations),
            new TeleportToBuildCommand(translations),
            new TopBuildersCommand(translations),
            new WreckCommand(translations),
            new WreckVehicleCommand(translations)
        ];

        if (RocketModService<IBuildableDirectory>.TryGetService() == default)
            RocketModService<IBuildableDirectory>.RegisterService(new DefaultBuildableDirectory());

        Commands.LoadAndRegisterCommands(this);
    }

    /// <inheritdoc />
    protected override void Load()
    {
        Commands.ReloadCommands(this);
        LogManager.Information(LoggingConstants.PluginLoaded);
    }

    /// <inheritdoc />
    protected override void Unload()
    {
        LogManager.Information(LoggingConstants.PluginUnloaded);
    }
}