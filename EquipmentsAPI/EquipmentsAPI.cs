using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils;
using static EquipmentsAPI.Equipments;

namespace EquipmentsAPI;

public interface IEquipmentsAPI
{
    public static readonly PluginCapability<IEquipmentsAPI?> Capability = new("equipments:api");

    public string GetDatabaseString();
    public bool Item_Equip(CCSPlayerController player, Dictionary<string, string> item);
    public bool Item_Unequip(CCSPlayerController player, Dictionary<string, string> item);
    public Dictionary<string, string>? GetItem(string type, string uniqueId);
    public List<KeyValuePair<string, Dictionary<string, string>>> GetItemsByType(string type);
    public List<Equipments_Items> GetPlayerEquipments(CCSPlayerController player);
    public void RegisterType(string type, Action<ResourceManifest> ServerPrecacheResources, Func<CCSPlayerController, Dictionary<string, string>, bool> equip, Func<CCSPlayerController, Dictionary<string, string>, bool> unequip, bool equipable, bool? alive = false);
}