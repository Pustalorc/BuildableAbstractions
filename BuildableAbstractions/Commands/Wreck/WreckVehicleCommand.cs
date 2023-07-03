using System.Collections.Generic;
using System.Threading.Tasks;
using Pustalorc.Libraries.RocketModCommandsExtended.Abstractions;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Pustalorc.Libraries.BuildableAbstractions.Commands.Wreck;

internal sealed class WreckVehicleCommand : RocketCommandWithTranslations
{
    public override AllowedCaller AllowedCaller => AllowedCaller.Player;
    public override string Name => "wreckVehicle";
    public override string Help => "Wrecks all the buildables on the vehicle that you are looking at.";
    public override string Syntax => "";

    public override List<string> Aliases => new() { "wv" };


    public override Dictionary<string, string> DefaultTranslations => new()
    {
        { TranslationKeys.CommandExceptionKey, CommandTranslationConstants.CommandExceptionValue }
    };

    public WreckVehicleCommand(Dictionary<string, string> translations) : base(true, translations)
    {
    }

    public override Task ExecuteAsync(IRocketPlayer caller, string[] command)
    {
        var player = (UnturnedPlayer)caller;
        var raycastInfo = DamageTool.raycast(new Ray(player.Player.look.aim.position, player.Player.look.aim.forward),
            10f, RayMasks.VEHICLE);

        if (raycastInfo.vehicle == null)
        {
            SendTranslatedMessage(caller, "no_vehicle_found");
            return Task.CompletedTask;
        }

        if (raycastInfo.vehicle.isDead)
        {
            SendTranslatedMessage(caller, "vehicle_dead");
            return Task.CompletedTask;
        }

        if (!BarricadeManager.tryGetPlant(raycastInfo.transform, out var x, out var y, out var plant, out var region))
        {
            SendTranslatedMessage(caller, "vehicle_no_plant");
            return Task.CompletedTask;
        }

        for (var i = region.drops.Count - 1; i > 0; i--)
            BarricadeManager.destroyBarricade(region.drops[i], x, y, plant);

        SendTranslatedMessage(caller, "vehicle_wreck",
            raycastInfo.vehicle.asset.vehicleName ?? raycastInfo.vehicle.asset.name, raycastInfo.vehicle.id,
            raycastInfo.vehicle.instanceID, raycastInfo.vehicle.lockedOwner.ToString());
        return Task.CompletedTask;
    }
}