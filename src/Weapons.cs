using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

public static class Weapons
{
    public static readonly Dictionary<nint, string> oldDesignerName = new();

    public static void Equip(CCSPlayerController player, MenuCategory category, string weaponSpec)
    {
        if (string.IsNullOrEmpty(weaponSpec)) return;
        var parts = weaponSpec.Split(':');
        if (parts.Length <2) return; // require base:subclass

        var weaponName = parts[0];
        var newSubclass = parts[1];

        var weapon = GetWeapon(player, weaponName);
        if (weapon == null || !weapon.IsValid) return;

        SetSubclass(weapon, weaponName, newSubclass);
    }

    public static void Unequip(CCSPlayerController player, MenuCategory category, string weaponSpec)
    {
        if (string.IsNullOrEmpty(weaponSpec)) return;
        var parts = weaponSpec.Split(':');
        if (parts.Length == 0) return;

        var baseName = parts[0];
        CPlayer_WeaponServices? weaponServices = player.PlayerPawn?.Value?.WeaponServices;
        if (weaponServices == null) return;

        foreach (var w in weaponServices.MyWeapons)
        {
            var weapon = w.Value;
            if (weapon == null || !weapon.IsValid) continue;
            if (GetDesignerName(weapon) != baseName) continue;

            var handle = weapon.Handle;
            if (oldDesignerName.TryGetValue(handle, out var oldSubclass))
            {
                if (!string.IsNullOrEmpty(oldSubclass))
                    weapon.AcceptInput("ChangeSubclass", weapon, weapon, oldSubclass);
                oldDesignerName.Remove(handle);
            }
        }
    }

    public static void ProcessEntity(CBasePlayerWeapon weapon)
    {
        CCSPlayerController? player = FindTargetFromWeapon(weapon);
        if (player == null) return;

        var equipments = Utils.GetEquippedSelections(player);

        foreach (var item in equipments)
        {
            var equipment = item.equipment;

            var parts = equipment.Weapon.Split(':');
            if (parts.Length < 2) return; // require base:subclass

            var weaponName = parts[0];
            var newSubclass = parts[1];

            if (weaponName == GetDesignerName(weapon))
            {
                SetSubclass(weapon, weaponName, newSubclass);
                break;
            }
        }
    }

    public static void SetSubclass(CBasePlayerWeapon weapon, string oldSubclass, string newSubclass)
    {
        var handle = weapon.Handle;
        oldDesignerName[handle] = oldSubclass;

        if (!string.IsNullOrEmpty(newSubclass))
            weapon.AcceptInput("ChangeSubclass", weapon, weapon, newSubclass);
    }

    private static CCSPlayerController? FindTargetFromWeapon(CBasePlayerWeapon weapon)
    {
        SteamID steamId = new(weapon.OriginalOwnerXuidLow);

        CCSPlayerController? player = steamId.IsValid()
                ? Utilities.GetPlayers().FirstOrDefault(p => p.IsValid && p.SteamID == steamId.SteamId64) ?? Utilities.GetPlayerFromSteamId(weapon.OriginalOwnerXuidLow)
                : Utilities.GetPlayerFromIndex((int)weapon.OwnerEntity.Index) ?? Utilities.GetPlayerFromIndex((int)weapon.As<CCSWeaponBaseGun>().OwnerEntity.Value!.Index);

        return !string.IsNullOrEmpty(player?.PlayerName) ? player : null;
    }

    public static string GetDesignerName(CBasePlayerWeapon weapon)
    {
        string weaponDesignerName = weapon.DesignerName;
        ushort weaponIndex = weapon.AttributeManager.Item.ItemDefinitionIndex;

        return (weaponDesignerName, weaponIndex) switch
        {
            var (name, _) when name.Contains("bayonet") => "weapon_knife",
            ("weapon_m4a1", 60) => "weapon_m4a1_silencer",
            ("weapon_hkp2000", 61) => "weapon_usp_silencer",
            ("weapon_mp7", 23) => "weapon_mp5sd",
            _ => weaponDesignerName
        };
    }

    public static CBasePlayerWeapon? GetWeapon(CCSPlayerController player, string weaponName)
    {
        CPlayer_WeaponServices? weaponServices = player.PlayerPawn?.Value?.WeaponServices;
        if (weaponServices == null) return null;

        CBasePlayerWeapon? activeWeapon = weaponServices.ActiveWeapon?.Value;
        return activeWeapon != null && GetDesignerName(activeWeapon) == weaponName
            ? activeWeapon
            : (weaponServices.MyWeapons.SingleOrDefault(p => p.Value != null && GetDesignerName(p.Value) == weaponName)?.Value);
    }
}