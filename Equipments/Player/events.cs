using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using static CounterStrikeSharp.API.Core.Listeners;

using static Equipments.Equipments;

namespace Equipments;

public static class Event
{
    public static void Unload()
    {
        Instance.RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        Instance.RemoveListener<OnClientAuthorized>(OnClientAuthorized);
    }

    public static void Load()
    {
        Instance.RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        Instance.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        Instance.RegisterListener<OnClientAuthorized>(OnClientAuthorized);
    }

    public static void OnServerPrecacheResources(ResourceManifest manifest)
    {
        Instance.GlobalEquipmentsItemTypes.ForEach((type) => {
            type.ServerPrecacheResources(manifest);
        });
    }

    private static void OnClientAuthorized(int playerSlot, SteamID steamId)
    {
        CCSPlayerController? player = Utilities.GetPlayerFromSlot(playerSlot);

        if (player == null)
            return;

        Database.LoadPlayer(player);
    }

    public static HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;

        if (player == null)
            return HookResult.Continue;

        Instance.GlobalEquipmentsItems.RemoveAll(e => e.SteamID == player.SteamID);

        return HookResult.Continue;
    }

}
