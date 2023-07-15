using System.Collections.Generic;
using System.Globalization;
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
using SDG.Unturned;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.Commands.Wreck;

internal sealed class WreckCommand : RocketCommandWithTranslations
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Both;
    public override string Name => "wreck";
    public override string Help => "Destroys buildables from the map.";

    public override string Syntax =>
        "confirm | abort | b [radius] | s [radius] | <item> [radius] | v [item] [radius] | <player> [item] [radius]";

    public override List<string> Aliases => new() { "w" };

    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { TranslationKeys.CommandExceptionKey, CommandTranslationConstants.CommandExceptionValue },
        { CommandTranslationConstants.TopBuilderFormatKey, CommandTranslationConstants.TopBuilderFormatValue }
    };

    private Dictionary<string, WreckAction> WreckActions { get; }

    public WreckCommand(Dictionary<string, string> translations) : base(true, translations)
    {
        WreckActions = new Dictionary<string, WreckAction>();
    }

    public override async Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        var args = command.ToList();

        if (args.Count == 0)
        {
            SendTranslatedMessage(caller, TranslationKeys.CommandUsageKey);
            return;
        }

        var abort = args.CheckArgsIncludeString("abort", out var index);
        if (index > -1)
            args.RemoveAt(index);

        var confirm = args.CheckArgsIncludeString("confirm", out index);
        if (index > -1)
            args.RemoveAt(index);

        var plants = args.CheckArgsIncludeString("v", out index);
        if (index > -1)
            args.RemoveAt(index);

        var barricades = args.CheckArgsIncludeString("b", out index);
        if (index > -1)
            args.RemoveAt(index);

        var structs = args.CheckArgsIncludeString("s", out index);
        if (index > -1)
            args.RemoveAt(index);

        var target = args.GetIRocketPlayer(out index);
        if (index > -1)
            args.RemoveAt(index);

        var itemAssetInput = Translate(CommandTranslationConstants.NotAvailableKey);
        var itemAssets = args.GetMultipleItemAssets(out index);
        if (index > -1)
        {
            itemAssetInput = args[index];
            args.RemoveAt(index);
        }

        var radius = args.GetFloat(out index);
        if (index > -1)
            args.RemoveAt(index);

        if (abort)
        {
            await Abort(caller);
            return;
        }

        if (confirm)
        {
            await Confirm(caller);
            return;
        }

        await CreateNewWreck(caller, radius, itemAssetInput, barricades, structs, plants, itemAssets, target);
    }

    private Task Abort(IRocketPlayer caller)
    {
        if (!WreckActions.Remove(caller.Id))
        {
            SendTranslatedMessage(caller, "no_action_queued");
            return Task.CompletedTask;
        }

        SendTranslatedMessage(caller, "action_cancelled");
        return Task.CompletedTask;
    }

    private async Task Confirm(IRocketPlayer caller)
    {
        var notAvailableText = Translate("not_available");
        var callerId = caller.Id;
        var targetName = notAvailableText;
        var radiusText = notAvailableText;

        if (!WreckActions.TryGetValue(callerId, out var action))
        {
            SendTranslatedMessage(caller, "no_action_queued");
            return;
        }

        WreckActions.Remove(callerId);

        ulong owner = default;

        if (action.TargetPlayer != null && ulong.TryParse(action.TargetPlayer.Id, out var pId))
        {
            targetName = action.TargetPlayer.DisplayName;
            owner = pId;
        }

        var maxRadius = float.MaxValue;

        if (!float.IsNegativeInfinity(action.MaxRadius))
        {
            radiusText = action.MaxRadius.ToString(CultureInfo.CurrentCulture);
            maxRadius = Mathf.Pow(action.MaxRadius, 2);
        }

        var buildables = (await GetBuildablesToWreck(action.FilterForBarricades, action.FilterForStructures, owner,
            action.IncludeVehicles, maxRadius, action.Center, action.ItemAssets)).ToList();
        if (!buildables.Any())
        {
            SendTranslatedMessage(caller, "cannot_wreck_no_builds");
            return;
        }

        foreach (var build in buildables)
            build.SafeDestroy();

        SendTranslatedMessage(caller, "wrecked", buildables.Count, action.ItemAssetName, radiusText, targetName,
            action.IncludeVehicles, action.FilterForBarricades, action.FilterForStructures);
    }

    private async Task CreateNewWreck(IRocketPlayer caller, float radius, string itemAssetInput, bool barricades,
        bool structs, bool plants, IReadOnlyCollection<ItemAsset> itemAssets, IRocketPlayer? target)
    {
        var notAvailableText = Translate("not_available");
        var callerId = caller.Id;
        var targetName = notAvailableText;
        var radiusText = notAvailableText;
        var itemAssetName = notAvailableText;
        Vector3? center = default;
        var maxRadius = float.MaxValue;

        ulong owner = default;

        if (target != null && ulong.TryParse(target.Id, out var pId))
        {
            targetName = target.DisplayName;
            owner = pId;
        }

        if (!float.IsNegativeInfinity(radius))
        {
            if (caller is not UnturnedPlayer cPlayer)
            {
                SendTranslatedMessage(caller, "cannot_be_executed_from_console");
                return;
            }

            radiusText = radius.ToString(CultureInfo.CurrentCulture);
            maxRadius = Mathf.Pow(radius, 2);
            center = cPlayer.Position;
        }

        itemAssetName = itemAssets.Count switch
        {
            1 => itemAssets.First().itemName,
            > 1 => itemAssetInput,
            _ => itemAssetName
        };

        var assets = itemAssets.Select(itemAsset => itemAsset.id).ToHashSet();
        var count = (await GetBuildablesToWreck(barricades, structs, owner, plants, maxRadius, center, assets)).Count();
        if (count <= 0)
        {
            SendTranslatedMessage(caller, "cannot_wreck_no_builds");
            return;
        }

        var wreckAction = new WreckAction(plants, barricades, structs, target, center, assets, radius, itemAssetName);

        string translationKey;
        if (WreckActions.TryGetValue(callerId, out _))
        {
            WreckActions[callerId] = wreckAction;
            translationKey = "wreck_action_queued_new";
        }
        else
        {
            WreckActions.Add(callerId, wreckAction);
            translationKey = "wreck_action_queued";
        }

        SendTranslatedMessage(caller, translationKey, itemAssetName, radiusText, targetName, plants, barricades,
            structs, count);
    }

    private static Task<IEnumerable<Buildable>> GetBuildablesToWreck(bool barricades, bool structures, ulong owner,
        bool plants, float maxRadius, Vector3? center, HashSet<ushort> itemAssets)
    {
        var options = new GetBuildableOptions(owner, default, plants, maxRadius, float.MinValue, center, itemAssets);
        IEnumerable<Buildable> builds;

        if (barricades)
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<BarricadeBuildable>(options);
        else if (structures)
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<StructureBuildable>(options);
        else
            builds = RocketModService<IBuildableDirectory>.GetService().GetBuildables<Buildable>(options);

        return Task.FromResult(builds);
    }
}