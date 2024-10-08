﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Pustalorc.Libraries.RocketModServices.Services;
using Pustalorc.Plugins.BuildableAbstractions.Commands.Extensions;
using Rocket.API;

namespace Pustalorc.Plugins.BuildableAbstractions.Commands.Information;

internal sealed class TopBuildersCommand(Dictionary<string, string> translations)
    : RocketCommandWithTranslations(true, translations)
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Both;
    public override string Name => "topBuilders";
    public override string Help => "Displays the top 5 builders in the game.";
    public override string Syntax => "v";

    public override List<string> Aliases => ["topB"];

    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { TranslationKeys.CommandExceptionKey, CommandTranslationConstants.CommandExceptionValue },
        { CommandTranslationConstants.TopBuilderFormatKey, CommandTranslationConstants.TopBuilderFormatValue }
    };

    public override Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        var args = command.ToList();

        var plants = args.CheckArgsIncludeString("v", out var index);
        if (index > -1)
            args.RemoveAt(index);

        var builds = RocketModService<IBuildableDirectory>.GetService()
            .GetBuildables<Buildable>(new GetBuildableOptions(default, default, plants));

        var topBuilders = builds.GroupBy(static k => k.Owner).OrderByDescending(static k => k.Count()).Take(5).ToList();

        for (var i = 0; i < topBuilders.Count; i++)
        {
            var builder = topBuilders.ElementAt(i);

            SendTranslatedMessage(caller, CommandTranslationConstants.TopBuilderFormatKey, i + 1, builder.Key,
                builder.Count());
        }

        return Task.CompletedTask;
    }
}