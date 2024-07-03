using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using static Equipments.Equipments;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public static class Item_Hats
{
    private static Dictionary<ulong, CBaseModelEntity> Hats = new();

    public static void OnPluginStart()
    {
        Item.RegisterType("hat", OnServerPrecacheResources, OnEquip, OnUnequip, true, null);
        Instance.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
    }
    public static void OnServerPrecacheResources(ResourceManifest manifest)
    {
        List<KeyValuePair<string, Dictionary<string, string>>> items = Item.GetItemsByType("hat");

        foreach (KeyValuePair<string, Dictionary<string, string>> item in items)
        {
            manifest.AddResource(item.Value["uniqueid"]);
        }
    }
    public static bool OnEquip(CCSPlayerController player, Dictionary<string, string> item)
    {
        EquipHat(player);
        return true;
    }
    public static bool OnUnequip(CCSPlayerController player, Dictionary<string, string> item)
    {
        UnEquipHat(player);
        return true;
    }

    public static void EquipHat(CCSPlayerController player)
    {
        UnEquipHat(player);
        Instance.AddTimer(0.1f, () => {
            Equipments_Items? playerItems = Instance.GlobalEquipmentsItems.FirstOrDefault(p => p.SteamID == player.SteamID && p.Type == "hat");
            if (playerItems == null) return;

            Dictionary<string, string>? itemdata = Item.GetItem(playerItems.Type, playerItems.UniqueId);
            if (itemdata == null) return;

            player.PlayerPawn.Value!.Effects = 1;
            Utilities.SetStateChanged(player.PlayerPawn.Value!, "CBaseEntity", "m_fEffects");

            CreateItem(player, playerItems.UniqueId);
        });
    }
    public static void UnEquipHat(CCSPlayerController player)
    {
        if (Hats.TryGetValue(player.SteamID, out var entity))
        {
            if (entity.IsValid) entity.Remove();
            Hats.Remove(player.SteamID);
        }
    }

    public static void CreateItem(CCSPlayerController player, string itemName)
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic_override");

        Instance.AddTimer(0.1f, () => {
            entity!.Globalname = $"{player.SteamID}({itemName})#{Functions.RandomString(6)}";
            entity.SetModel(itemName);
            entity.DispatchSpawn();
            entity.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");
            Hats[player.SteamID] = entity;
        });
    }

    public static HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        EquipHat(@event.Userid!);
        return HookResult.Continue;
    }

}