﻿using System.Collections.Generic;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Actions;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Information;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Wreck;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Pustalorc.Libraries.RocketModCommandsExtended.Extensions;
using Pustalorc.Libraries.RocketModServices.Services;
using Rocket.Core.Plugins;

namespace Pustalorc.Libraries.BuildableAbstractions;

/// <inheritdoc />
public sealed class BuildableAbstractionsPlugin : RocketPlugin
{
    private List<MultiThreadedRocketCommand> Commands { get; }

    /// <inheritdoc />
    public BuildableAbstractionsPlugin()
    {
        var translations = this.GetCurrentTranslationsForCommands();

        Commands = new List<MultiThreadedRocketCommand>
        {
            new FindBuildsCommand(translations),
            new RemoveBuildableCommand(translations),
            new TeleportToBuildCommand(translations),
            new TopBuildersCommand(translations),
            new WreckCommand(translations),
            new WreckVehicleCommand(translations)
        };

        if (RocketModService<IBuildableDirectory>.TryGetService() == default)
            RocketModService<IBuildableDirectory>.RegisterService(new DefaultBuildableDirectory());

        Commands.LoadAndRegisterCommands(this);
    }

    /// <inheritdoc />
    protected override void Load()
    {
        Commands.LoadAndRegisterCommands(this);
    }
}