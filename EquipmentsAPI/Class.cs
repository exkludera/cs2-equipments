using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace EquipmentsAPI;

public abstract class Equipments
{
    public class Equipments_Items
    {
        public required ulong SteamID { get; set; }
        public required string Type { get; set; }
        public required string UniqueId { get; set; }
        public int Slot { get; set; }
    }

    public class Equipments_Item_Types
    {
        public required string Type;
        public required Action<ResourceManifest> ServerPrecacheResources;
        public required Func<CCSPlayerController, Dictionary<string, string>, bool> Equip;
        public required Func<CCSPlayerController, Dictionary<string, string>, bool> Unequip;
    }
}