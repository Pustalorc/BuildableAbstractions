using System.Collections.Generic;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Actions;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Information;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Wreck;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Pustalorc.Libraries.RocketModCommandsExtended.Extensions;
using Rocket.Core.Plugins;

namespace Pustalorc.Libraries.BuildableAbstractions;

public sealed class BuildableAbstractionsPlugin : RocketPlugin
{
    private List<MultiThreadedRocketCommand> Commands { get; }

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

        Commands.LoadAndRegisterCommands(this);
    }

    protected override void Load()
    {
        Commands.LoadAndRegisterCommands(this);
    }
}