using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using EquipmentsAPI;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public class EquipmentsAPI : IEquipmentsAPI
{
    public EquipmentsAPI()
    {
    }

    public string GetDatabaseString()
    {
        return Database.GlobalDatabaseConnectionString;
    }

    public bool Item_Equip(CCSPlayerController player, Dictionary<string, string> item)
    {
        return Item.Equip(player, item);
    }

    public bool Item_Unequip(CCSPlayerController player, Dictionary<string, string> item)
    {
        return Item.Unequip(player, item);
    }

    public Dictionary<string, string>? GetItem(string type, string UniqueId)
    {
        return Item.GetItem(type, UniqueId);
    }

    public List<KeyValuePair<string, Dictionary<string, string>>> GetItemsByType(string type)
    {
        return Item.GetItemsByType(type);
    }

    public List<Equipments_Items> GetPlayerEquipments(CCSPlayerController player)
    {
        return Item.GetPlayerEquipments(player);
    }

    public void RegisterType(string Type, Action<ResourceManifest> ServerPrecacheResources, Func<CCSPlayerController, Dictionary<string, string>, bool> Equip, Func<CCSPlayerController, Dictionary<string, string>, bool> Unequip, bool Equipable, bool? Alive)
    {
        Item.RegisterType(Type, ServerPrecacheResources, Equip, Unequip, Equipable, Alive);
    }
}