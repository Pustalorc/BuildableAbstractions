using System.Collections.Generic;
using System.Threading.Tasks;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.Commands.Actions;

internal sealed class RemoveBuildableCommand : RocketCommandWithTranslations
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Player;
    public override string Name => "removeBuildable";
    public override string Help => "Removes the buildable you are staring at";
    public override string Syntax => "";

    public override List<string> Aliases => new() { "rb" };

    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { TranslationKeys.CommandExceptionKey, CommandTranslationConstants.CommandExceptionValue },
        { CommandTranslationConstants.NotLookingBuildableKey, CommandTranslationConstants.NotLookingBuildableValue }
    };

    public RemoveBuildableCommand(Dictionary<string, string> translations) : base(true, translations)
    {
    }

    public override Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        if (caller is not UnturnedPlayer player)
            return Task.CompletedTask;

        if (!Physics.Raycast(new Ray(player.Player.look.aim.position, player.Player.look.aim.forward), out var hit,
                player.Player.look.perspective == EPlayerPerspective.THIRD ? 6 : 4,
                RayMasks.BARRICADE_INTERACT | RayMasks.BARRICADE | RayMasks.STRUCTURE |
                RayMasks.STRUCTURE_INTERACT) ||
            hit.transform == null)
        {
            SendTranslatedMessage(caller, CommandTranslationConstants.NotLookingBuildableKey);
            return Task.CompletedTask;
        }

        var buildable = BuildableDirectory.Instance.GetBuildable<Buildable>(hit.transform);
        if (buildable == null)
        {
            SendTranslatedMessage(caller, CommandTranslationConstants.NotLookingBuildableKey);
            return Task.CompletedTask;
        }

        buildable.SafeDestroy();
        return Task.CompletedTask;
    }
}