using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

public static class Utils
{
    private static readonly Plugin Instance = Plugin.Instance;

    public static void LogError(string message)
    {
        Instance.Logger.LogError(message);
    }

    public static bool HasPermission(CCSPlayerController player, List<string> permissions, string team = "")
    {
        bool requireCheck = permissions != null && permissions.Any(p => !string.IsNullOrWhiteSpace(p));
        bool hasPermission = !requireCheck;

        if (requireCheck)
        {
            foreach (string permission in permissions!)
            {
                if (!string.IsNullOrWhiteSpace(permission) && permission.StartsWith("@") && AdminManager.PlayerHasPermissions(player, permission)) { hasPermission = true; break; }
                if (!string.IsNullOrWhiteSpace(permission) && permission.StartsWith("#") && AdminManager.PlayerInGroup(player, permission)) { hasPermission = true; break; }
            }
        }

        team = (team ?? string.Empty).ToLower();
        bool isTeamValid =
            ((team == "t" || team == "terrorist") && player.Team == CsTeam.Terrorist) ||
            ((team == "ct" || team == "counterterrorist") && player.Team == CsTeam.CounterTerrorist) ||
            string.IsNullOrEmpty(team) || team == "both" || team == "all";

        return hasPermission && isTeamValid;
    }

    // Traverse all categories recursively and yield fullPath -> MenuCategory
    public static IEnumerable<(string fullPath, MenuCategory category)> EnumerateCategories()
    {
        foreach (var kv in Instance.Config.Categories)
        {
            foreach (var item in EnumerateCategoriesRecursive(kv.Key, kv.Value))
                yield return item;
        }
    }

    private static IEnumerable<(string fullPath, MenuCategory category)> EnumerateCategoriesRecursive(string path, MenuCategory category)
    {
        yield return (path, category);
        if (category.SubCategories == null) yield break;
        foreach (var kv in category.SubCategories)
        {
            var newPath = string.IsNullOrEmpty(path) ? kv.Key : $"{path}-{kv.Key}";
            foreach (var item in EnumerateCategoriesRecursive(newPath, kv.Value))
                yield return item;
        }
    }

    public static bool TryFindCategory(string fullPath, out MenuCategory category)
    {
        foreach (var (p, c) in EnumerateCategories())
        {
            if (string.Equals(p, fullPath, StringComparison.OrdinalIgnoreCase))
            {
                category = c;
                return true;
            }
        }
        category = default!; 
        return false;
    }

    // Get currently equipped selections for a player across all categories
    public static IEnumerable<(string fullPath, Equipment equipment, MenuCategory category)> GetEquippedSelections(CCSPlayerController player)
    {
        if (!Cookies.playerCookies.TryGetValue(player.Slot, out var cookies) || cookies.Count ==0)
            yield break;

        foreach (var (fullPath, category) in EnumerateCategories())
        {
            // Skip categories the player no longer has access to
            if (!HasPermission(player, category.Permission, category.Team))
                continue;
            foreach (var eq in category.Equipment)
            {
                // Skip equipment the player no longer has access to
                if (!HasPermission(player, eq.Permission, eq.Team))
                    continue;
                var cookieName = Cookies.BuildCookieName(fullPath, eq.Name);
                if (cookies.TryGetValue(cookieName, out var value) && !string.IsNullOrEmpty(value))
                    yield return (fullPath, eq, category);
            }
        }
    }

    // Toggle an equipment selection and persist via Clientprefs
    public static void ToggleEquipment(CCSPlayerController player, string fullPath, MenuCategory category, Equipment equipment)
    {
        if (Cookies.ClientprefsApi == null) return;

        // Respect current access when toggling
        if (!HasPermission(player, category.Permission, category.Team)) return;
        if (!HasPermission(player, equipment.Permission, equipment.Team)) return;
        var cookieName = Cookies.BuildCookieName(fullPath, equipment.Name);
        if (!Cookies.equipmentCookies.TryGetValue(cookieName, out var cookieId)) return;

        Cookies.playerCookies.TryAdd(player.Slot, new());
        var cookies = Cookies.playerCookies[player.Slot];
        bool isEnabled = cookies.TryGetValue(cookieName, out var value) && !string.IsNullOrEmpty(value);

        if (category.AllowMultiple)
        {
            // Toggle this item only
            if (isEnabled)
            {
                Cookies.ClientprefsApi.SetPlayerCookie(player, cookieId, "");
                cookies.Remove(cookieName);
                RemoveEquipment(player, category, equipment);
            }
            else
            {
                Cookies.ClientprefsApi.SetPlayerCookie(player, cookieId, "1");
                cookies[cookieName] = "1";
                ApplyEquipment(player, category, equipment);
            }
        }
        else
        {
            // Single-choice: if the clicked item is already enabled, clear all in this category (deselect)
            if (isEnabled)
            {
                foreach (var eq in category.Equipment)
                {
                    var cn = Cookies.BuildCookieName(fullPath, eq.Name);
                    if (!Cookies.equipmentCookies.TryGetValue(cn, out var id)) continue;
                    Cookies.ClientprefsApi.SetPlayerCookie(player, id, "");
                    if (cookies.ContainsKey(cn)) { cookies.Remove(cn); RemoveEquipment(player, category, eq); }
                }
                return;
            }

            // Otherwise, enable selected and disable others in the same category
            foreach (var eq in category.Equipment)
            {
                var cn = Cookies.BuildCookieName(fullPath, eq.Name);
                if (!Cookies.equipmentCookies.TryGetValue(cn, out var id)) continue;

                if (eq.Name.Equals(equipment.Name, StringComparison.OrdinalIgnoreCase))
                {
                    Cookies.ClientprefsApi.SetPlayerCookie(player, id, "1");
                    cookies[cn] = "1";
                    ApplyEquipment(player, category, eq);
                }
                else
                {
                    Cookies.ClientprefsApi.SetPlayerCookie(player, id, "");
                    if (cookies.ContainsKey(cn)) { cookies.Remove(cn); RemoveEquipment(player, category, eq); }
                }
            }
        }
    }

    public static void ApplyEquipment(CCSPlayerController player, MenuCategory category, Equipment equipment)
    {
        if (!string.IsNullOrEmpty(equipment.Model))
            Models.Equip(player, category, equipment.Model);

        if (!string.IsNullOrEmpty(equipment.Particle))
            Particles.Equip(player, category, player.PlayerPawn.Value?.AbsOrigin!, equipment.Particle);

        if (!string.IsNullOrEmpty(equipment.Weapon))
            Weapons.Equip(player, category, equipment.Weapon);
    }

    public static void RemoveEquipment(CCSPlayerController player, MenuCategory category, Equipment equipment)
    {
        if (!string.IsNullOrEmpty(equipment.Model))
            Models.Unequip(player, category, equipment.Model);

        if (!string.IsNullOrEmpty(equipment.Particle))
            Particles.Unequip(player, category, equipment.Particle);

        if (!string.IsNullOrEmpty(equipment.Weapon))
            Weapons.Unequip(player, category, equipment.Weapon);
    }
}