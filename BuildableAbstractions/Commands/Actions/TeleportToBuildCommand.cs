using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Implementations;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Interfaces;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory.Options;
using Pustalorc.Libraries.BuildableAbstractions.Commands.Extensions;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Pustalorc.Libraries.RocketModServices.Services;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.Commands.Actions;

internal sealed class TeleportToBuildCommand(Dictionary<string, string> translations)
    : RocketCommandWithTranslations(true, translations)
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Player;
    public override string Name => "teleportToBuild";
    public override string Help => "Teleports you to a random buildable on the map based on filters.";
    public override string Syntax => "b [player] | s [player] | v [player] | [player] [id]";

    public override List<string> Aliases => ["tpb"];

    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { TranslationKeys.CommandExceptionKey, CommandTranslationConstants.CommandExceptionValue },
        { CommandTranslationConstants.NotAvailableKey, CommandTranslationConstants.NotAvailableValue },
        {
            CommandTranslationConstants.CannotTeleportNoBuildsKey,
            CommandTranslationConstants.CannotTeleportNoBuildsValue
        },
        {
            CommandTranslationConstants.CannotTeleportBuildsTooCloseKey,
            CommandTranslationConstants.CannotTeleportBuildsTooCloseValue
        }
    };

    public override Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        if (caller is not UnturnedPlayer player) return Task.CompletedTask;

        var notAvailableText = Translate(CommandTranslationConstants.NotAvailableKey);

        var itemAssetInput = notAvailableText;
        var itemAssetName = notAvailableText;
        var targetStr = notAvailableText;

        var args = command.ToList();

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

        ulong owner = default;
        var minRange = float.MinValue;

        if (target != null && ulong.TryParse(target.Id, out var pId))
        {
            targetStr = target.DisplayName;
            owner = pId;
        }
        else
        {
            minRange = 400;
        }

        var options = new GetBuildableOptions(owner, default, plants, float.MaxValue, minRange, player.Position,
            itemAssets.Select(static itemAsset => itemAsset.id).ToHashSet());
        IEnumerable<Buildable> builds;

        if (barricades)
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<BarricadeBuildable>(options);
        else if (structs)
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<StructureBuildable>(options);
        else
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<Buildable>(options);

        var buildsL = builds.ToList();
        if (!buildsL.Any())
        {
            SendTranslatedMessage(caller, CommandTranslationConstants.CannotTeleportNoBuildsKey, itemAssetName,
                targetStr, plants, barricades, structs);
            return Task.CompletedTask;
        }

        var build = buildsL[Random.Range(0, buildsL.Count - 1)];

        if (build != null)
        {
            var offset = new Vector3(0, plants ? 4 : 2, 0);

            while (!player.Player.stance.wouldHaveHeightClearanceAtPosition(build.Position + offset, 0.5f))
                offset.y++;

            player.Teleport(build.Position + offset, player.Rotation);
        }
        else
        {
            SendTranslatedMessage(caller, CommandTranslationConstants.CannotTeleportBuildsTooCloseKey, itemAssetName,
                targetStr, plants, barricades, structs);
        }

        return Task.CompletedTask;
    }
}