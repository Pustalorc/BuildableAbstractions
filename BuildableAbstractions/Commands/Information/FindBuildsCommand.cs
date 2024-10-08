﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Pustalorc.Libraries.RocketModServices.Services;
using Pustalorc.Plugins.BuildableAbstractions.Commands.Extensions;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Pustalorc.Plugins.BuildableAbstractions.Commands.Information;

internal sealed class FindBuildsCommand(Dictionary<string, string> translations)
    : RocketCommandWithTranslations(true, translations)
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Both;
    public override string Name => "findBuilds";
    public override string Help => "Finds buildables around the map";

    public override string Syntax =>
        "b [radius] | s [radius] | [id] [radius] | v [id] [radius] | [player] [id] [radius] | [player] b [radius] | [player] s [radius] | [player] v [id] [radius]";

    public override List<string> Aliases => ["fb"];

    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { TranslationKeys.CommandExceptionKey, CommandTranslationConstants.CommandExceptionValue },
        { CommandTranslationConstants.NotAvailableKey, CommandTranslationConstants.NotAvailableValue },
        {
            CommandTranslationConstants.CannotExecuteFromConsoleKey,
            CommandTranslationConstants.CannotExecuteFromConsoleValue
        },
        { CommandTranslationConstants.BuildCountKey, CommandTranslationConstants.BuildCountValue }
    };

    public override Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        var args = command.ToList();
        var notAvailableText = Translate(CommandTranslationConstants.NotAvailableKey);

        var itemAssetInput = notAvailableText;
        var itemAssetName = notAvailableText;
        var radiusStr = notAvailableText;
        var targetStr = notAvailableText;

        var barricades = args.CheckArgsIncludeString("b", out var index);
        if (index > -1)
            args.RemoveAt(index);

        var structs = args.CheckArgsIncludeString("s", out index);
        if (index > -1)
            args.RemoveAt(index);

        var plants = args.CheckArgsIncludeString("v", out index);
        if (index > -1)
            args.RemoveAt(index);

        var target = args.GetIRocketPlayer(out index);
        if (index > -1)
            args.RemoveAt(index);

        var itemAssets = args.GetMultipleItemAssets(out index);
        var assetCount = itemAssets.Count;
        if (index > -1)
        {
            itemAssetInput = args[index];
            args.RemoveAt(index);
        }

        itemAssetName = assetCount switch
        {
            1 => itemAssets.First().itemName,
            > 1 => itemAssetInput,
            _ => itemAssetName
        };

        var radius = args.GetFloat(out index);
        if (index > -1)
            args.RemoveAt(index);

        ulong owner = default;

        if (target != null && ulong.TryParse(target.Id, out var pId))
        {
            targetStr = target.DisplayName;
            owner = pId;
        }

        var maxRange = float.MaxValue;
        Vector3? position = default;

        if (!float.IsNegativeInfinity(radius))
        {
            radiusStr = radius.ToString(CultureInfo.InvariantCulture);
            if (caller is not UnturnedPlayer cPlayer)
            {
                SendTranslatedMessage(caller, CommandTranslationConstants.CannotExecuteFromConsoleKey);
                return Task.CompletedTask;
            }

            maxRange = Mathf.Pow(radius, 2);
            position = cPlayer.Position;
        }

        var options = new GetBuildableOptions(owner, default, plants, maxRange, float.MinValue, position,
            itemAssets.Select(static itemAsset => itemAsset.id).ToHashSet());
        IEnumerable<Buildable> builds;

        if (barricades)
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<BarricadeBuildable>(options);
        else if (structs)
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<StructureBuildable>(options);
        else
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<Buildable>(options);

        SendTranslatedMessage(caller, CommandTranslationConstants.BuildCountKey, builds.Count(), itemAssetName,
            radiusStr, targetStr, plants,
            barricades, structs);
        return Task.CompletedTask;
    }
}