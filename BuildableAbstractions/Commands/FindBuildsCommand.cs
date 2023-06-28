using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using Pustalorc.Libraries.BuildableAbstractions.Extensions;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.Commands;

public sealed class FindBuildsCommand : RocketCommandWithTranslations
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Both;

    public override string Name => "findBuilds";

    public override string Help => "Finds buildables around the map";

    public override string Syntax =>
        "b [radius] | s [radius] | [id] [radius] | v [id] [radius] | [player] [id] [radius] | [player] b [radius] | [player] s [radius] | [player] v [id] [radius]";

    public override List<string> Aliases => new() { "fb" };

    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { "command_exception", "The command failed to execute. Error: " }
    };

    public FindBuildsCommand(StringComparer stringComparer) : base(true, stringComparer)
    {
    }

    public override Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        var args = command.ToList();
        var notAvailableText = Translate("not_available");

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

        var itemAssets = args.GetMultipleItemAssets(out index).ToDictionary(k => k.id);
        var assetCount = itemAssets.Count;
        if (index > -1)
        {
            itemAssetInput = args[index];
            args.RemoveAt(index);
        }

        var radius = args.GetFloat(out index);
        if (index > -1)
            args.RemoveAt(index);

        var owner = 0UL;

        if (target != null && ulong.TryParse(target.Id, out var id))
        {
            targetStr = target.DisplayName;
            owner = id;
        }

        var options = new GetBuildableOptions(owner, default, plants);
        IEnumerable<Buildable> builds;

        if (barricades)
            builds = BuildableDirectory.Instance.GetBuildables<BarricadeBuildable>(options);
        else if (structs)
            builds = BuildableDirectory.Instance.GetBuildables<StructureBuildable>(options);
        else
            builds = BuildableDirectory.Instance.GetBuildables<Buildable>(options);

        if (assetCount > 0)
            builds = builds.Where(k => itemAssets.ContainsKey(k.AssetId));

        if (!float.IsNegativeInfinity(radius))
        {
            radiusStr = radius.ToString(CultureInfo.InvariantCulture);
            if (caller is not UnturnedPlayer cPlayer)
            {
                SendTranslatedMessage(caller, "cannot_be_executed_from_console");
                return Task.CompletedTask;
            }

            builds = builds.Where(k => (k.Position - cPlayer.Position).sqrMagnitude <= Mathf.Pow(radius, 2));
        }

        itemAssetName = assetCount switch
        {
            1 => itemAssets.First().Value.itemName,
            > 1 => itemAssetInput,
            _ => itemAssetName
        };

        SendTranslatedMessage(caller, "build_count", builds.Count(), itemAssetName, radiusStr, targetStr, plants,
            barricades, structs);
        return Task.CompletedTask;
    }
}