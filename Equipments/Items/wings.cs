using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using static Equipments.Equipments;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public static class Item_Wings
{
    private static Dictionary<ulong, CBaseModelEntity> Equipment = new();

    public static void OnPluginStart()
    {
        Item.RegisterType("wings", OnServerPrecacheResources, OnEquip, OnUnequip, true, null);
        Instance.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
    }
    public static void OnServerPrecacheResources(ResourceManifest manifest)
    {
        List<KeyValuePair<string, Dictionary<string, string>>> items = Item.GetItemsByType("wings");

        foreach (KeyValuePair<string, Dictionary<string, string>> item in items)
        {
            manifest.AddResource(item.Value["uniqueid"]);
        }
    }
    public static bool OnEquip(CCSPlayerController player, Dictionary<string, string> item)
    {
        Equip(player);
        return true;
    }
    public static bool OnUnequip(CCSPlayerController player, Dictionary<string, string> item)
    {
        UnEquip(player);
        return true;
    }

    public static void Equip(CCSPlayerController player)
    {
        UnEquip(player);

        Instance.AddTimer(0.1f, () => {

            if (!Functions.PlayerAlive(player))
                return;

            Equipments_Items? playerItems = Instance.GlobalEquipmentsItems.FirstOrDefault(p => p.SteamID == player.SteamID && p.Type == "wings");
            if (playerItems == null) return;

            Dictionary<string, string>? itemdata = Item.GetItem(playerItems.Type, playerItems.UniqueId);
            if (itemdata == null) return;

            player.PlayerPawn.Value!.Effects = 1;
            Utilities.SetStateChanged(player.PlayerPawn.Value!, "CBaseEntity", "m_fEffects");

            CreateItem(player, playerItems.UniqueId);
        });
    }
    public static void UnEquip(CCSPlayerController player)
    {
        if (Equipment.TryGetValue(player.SteamID, out var entity))
        {
            if (entity.IsValid) entity.Remove();
            Equipment.Remove(player.SteamID);
        }
    }

    public static void CreateItem(CCSPlayerController player, string itemName)
    {
        var entity = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic_override");

        Instance.AddTimer(0.1f, () => {
            entity!.Globalname = $"{player.SteamID}({itemName})#{Functions.RandomString(6)}";
            entity.SetModel(itemName);
            entity.DispatchSpawn();
            entity.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");
            Equipment[player.SteamID] = entity;
        });
    }

    public static HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        Equip(@event.Userid!);
        return HookResult.Continue;
    }

}