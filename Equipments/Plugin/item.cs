using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using static Equipments.Equipments;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public static class Item
{
 
    public static bool Equip(CCSPlayerController player, Dictionary<string, string> item)
    {
        Equipments_Item_Types? type = Instance.GlobalEquipmentsItemTypes.FirstOrDefault(i => i.Type == item["type"]);

        if (type == null)
            return false;

        Equipments_Items? currentitem = Instance.GlobalEquipmentsItems.FirstOrDefault(p => p.SteamID == player.SteamID && p.Type == item["type"] && item.TryGetValue("slot", out string? slot) && !string.IsNullOrEmpty(slot) && p.Slot == int.Parse(item["slot"]));

        if (currentitem != null)
        {
            Dictionary<string, string>? citem = GetItem(currentitem.Type, currentitem.UniqueId);

            if (citem != null)
                Unequip(player, citem);
        }

        if (type.Equip(player, item) == false)
            return false;

        if (!item.TryGetValue("slot", out string? sslot) || !int.TryParse(sslot, out int islot))
            islot = 0;

        Equipments_Items playeritem = new()
        {
            SteamID = player.SteamID,
            Type = item["type"],
            UniqueId = item["uniqueid"],
            Slot = islot
        };

        Instance.GlobalEquipmentsItems.Add(playeritem);

        Server.NextFrame(() => {
            Database.SavePlayerEquipment(player, playeritem);
        });

        return true;
    }

    public static bool Unequip(CCSPlayerController player, Dictionary<string, string> item)
    {
        Equipments_Item_Types? type = Instance.GlobalEquipmentsItemTypes.FirstOrDefault(i => i.Type == item["type"]);

        if (type == null || type.Unequip(player, item) == false)
            return false;

        Instance.GlobalEquipmentsItems.RemoveAll(p => p.SteamID == player.SteamID && p.UniqueId == item["uniqueid"]);

        Server.NextFrame(() => {
            Database.RemovePlayerEquipment(player, item["uniqueid"]);
        });

        return true;
    }

    public static bool PlayerUsing(CCSPlayerController player, string type, string UniqueId)
    {
        return Instance.GlobalEquipmentsItems.Any(p => p.SteamID == player.SteamID && p.Type == type && p.UniqueId == UniqueId);
    }

    public static bool IsInJson(string type, string UniqueId)
    {
        return Instance.Config.Items.Values
            .SelectMany(dict => dict.Values)
            .Any(item => item["type"] == type && item["uniqueid"] == UniqueId);
    }

    public static Dictionary<string, string>? GetItem(string type, string UniqueId)
    {
        return Instance.Config.Items.Values
            .SelectMany(dict => dict.Values)
            .FirstOrDefault(item => item["type"] == type && item["uniqueid"] == UniqueId);
    }

    public static List<KeyValuePair<string, Dictionary<string, string>>> GetItemsByType(string type)
    {
        return Instance.Config.Items.SelectMany(wk => wk.Value.Where(kvp => kvp.Value["type"] == type)).ToList();
    }

    public static List<Equipments_Items> GetPlayerEquipments(CCSPlayerController player)
    {
        return Instance.GlobalEquipmentsItems.Where(item => item.SteamID == player.SteamID).ToList();
    }
    public static void RegisterType(string Type, Action<ResourceManifest> ServerPrecacheResources, Func<CCSPlayerController, Dictionary<string, string>, bool> Equip, Func<CCSPlayerController, Dictionary<string, string>, bool> Unequip, bool Equipable, bool? Alive)
    {
        Instance.GlobalEquipmentsItemTypes.Add(new Equipments_Item_Types
        {
            Type = Type,
            ServerPrecacheResources = ServerPrecacheResources,
            Equip = Equip,
            Unequip = Unequip
        });
    }
}