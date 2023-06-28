using System.Collections.Generic;
using System.Threading.Tasks;
using Pustalorc.Libraries.BuildableAbstractions.API.Buildables.Abstraction;
using Pustalorc.Libraries.BuildableAbstractions.API.Directory;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
/*
#pragma warning disable 1591

namespace Pustalorc.Libraries.BuildableAbstractions.Commands;

public sealed class RemoveBuildableCommand : RocketCommandWithTranslations
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Player;
    public override string Name => "removebuildable";
    public override string Help => "Removes the buildable you are staring at";
    public override string Syntax => "";

    public override Dictionary<string, string> DefaultTranslations=> new Dictionary<string, string>()
    {
        
    }

    public RemoveBuildableCommand(bool multiThreaded, Dictionary<string, string> translations) : base(multiThreaded, translations)
    {
    }

    public override async Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        if (caller is not UnturnedPlayer player) return;

        if (!Physics.Raycast(new Ray(player.Player.look.aim.position, player.Player.look.aim.forward), out var hit,
                player.Player.look.perspective == EPlayerPerspective.THIRD ? 6 : 4,
                RayMasks.BARRICADE_INTERACT | RayMasks.BARRICADE | RayMasks.STRUCTURE |
                RayMasks.STRUCTURE_INTERACT) ||
            hit.transform == null)
        {
            SendTranslatedMessage(caller, ("not_looking_buildable"));
            return;
        }

        var buildable = BuildableDirectory.Instance.GetBuildable<Buildable>(hit.transform);
        if (buildable == null)
        {
            SendTranslatedMessage(caller, ("not_looking_buildable"));
            return;
        }

        buildable.SafeDestroy();
    }
}*/