using Clientprefs.API;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;

public static class Cookies
{
    public static readonly PluginCapability<IClientprefsApi> g_PluginCapability = new("Clientprefs");
    public static IClientprefsApi? ClientprefsApi;

    // Cookie name -> cookie id
    public static Dictionary<string, int> equipmentCookies = new();
    // player slot -> (cookie name -> value)
    public static Dictionary<int, Dictionary<string, string>> playerCookies = new();

    public static void Register(bool hotReload)
    {
        try
        {
            ClientprefsApi = g_PluginCapability.Get();

            if (ClientprefsApi == null)
                return;

            ClientprefsApi.OnDatabaseLoaded += OnClientprefDatabaseReady;
            ClientprefsApi.OnPlayerCookiesCached += OnPlayerCookiesCached;
        }
        catch (Exception ex)
        {
            throw new Exception("[Equipments] Failed to load ClientprefsApi! | " + ex.Message);
        }

        if (hotReload && ClientprefsApi != null)
        {
            OnClientprefDatabaseReady();

            foreach (CCSPlayerController player in Utilities.GetPlayers().Where(p => !p.IsBot))
                OnPlayerCookiesCached(player);
        }
    }

    public static void Unregister()
    {
        if (ClientprefsApi == null)
            return;

        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;
    }

    public static void OnClientprefDatabaseReady()
    {
        if (ClientprefsApi == null)
            return;

        equipmentCookies.Clear();

        foreach (var (fullPath, category) in Utils.EnumerateCategories())
        {
            foreach (var equipment in category.Equipment)
            {
                string cookieName = BuildCookieName(fullPath, equipment.Name);
                int cookieId = ClientprefsApi.RegPlayerCookie(cookieName, $"{equipment.Name} in {fullPath}", CookieAccess.CookieAccess_Protected);

                if (cookieId == -1)
                {
                    Utils.LogError($"Failed to register/load Cookie for {cookieName}");
                    continue;
                }

                equipmentCookies[cookieName] = cookieId;
            }
        }
    }

    public static string BuildCookieName(string fullPath, string equipmentName)
    => $"Equipment-{fullPath}-{equipmentName}";

    public static void OnPlayerCookiesCached(CCSPlayerController player)
    {
        if (ClientprefsApi == null)
            return;

        playerCookies[player.Slot] = new();

        foreach (var (fullPath, category) in Utils.EnumerateCategories())
        {
            foreach (var equipment in category.Equipment)
            {
                string cookieName = BuildCookieName(fullPath, equipment.Name);
                if (!equipmentCookies.TryGetValue(cookieName, out var cookieId))
                    continue;

                string cookieValue = ClientprefsApi.GetPlayerCookie(player, cookieId);
                if (!string.IsNullOrEmpty(cookieValue))
                    playerCookies[player.Slot][cookieName] = cookieValue;
            }
        }
    }
}