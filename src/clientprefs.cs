using Clientprefs.API;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using Microsoft.Extensions.Logging;

public partial class Plugin
{
    public readonly PluginCapability<IClientprefsApi> g_PluginCapability = new("Clientprefs");
    public IClientprefsApi? ClientprefsApi;

    public Dictionary<string, int> equipmentCookies = new();
    public Dictionary<int, Dictionary<string, string>> playerCookies = new();

    public override void OnAllPluginsLoaded(bool hotReload)
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

    public void UnloadClientprefs()
    {
        if (ClientprefsApi == null)
            return;

        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;
    }

    public void OnClientprefDatabaseReady()
    {
        if (ClientprefsApi == null)
            return;

        foreach (var category in Config.Categories)
        {
            if (category.Value.AllowMultiple)
            {
                foreach (var equipment in category.Value.Equipment)
                {
                    string cookieName = $"Equipment-{category.Key}-{equipment.Name}";
                    int cookieId = ClientprefsApi.RegPlayerCookie(cookieName, $"{equipment.Name} in {category.Key}", CookieAccess.CookieAccess_Protected);

                    if (cookieId == -1)
                    {
                        Logger.LogError($"[Equipments] Failed to register/load Cookie for {cookieName}");
                        return;
                    }

                    equipmentCookies[cookieName] = cookieId;
                }
            }
            else
            {
                string cookieName = $"Equipment-{category.Key}";
                int cookieId = ClientprefsApi.RegPlayerCookie(cookieName, $"Which Equipment in {category.Key}", CookieAccess.CookieAccess_Protected);

                if (cookieId == -1)
                {
                    Logger.LogError($"[Equipments] Failed to register/load Cookie for {cookieName}");
                    return;
                }

                equipmentCookies[cookieName] = cookieId;
            }
        }
    }

    public void OnPlayerCookiesCached(CCSPlayerController player)
    {
        if (ClientprefsApi == null)
            return;

        playerCookies[player.Slot] = new Dictionary<string, string>();

        foreach (var category in Config.Categories)
        {
            if (category.Value.AllowMultiple)
            {
                foreach (var equipment in category.Value.Equipment)
                {
                    string cookieName = $"Equipment-{category.Key}-{equipment.Name}";
                    string cookieValue = ClientprefsApi.GetPlayerCookie(player, equipmentCookies[cookieName]);

                    if (!string.IsNullOrEmpty(cookieValue))
                        playerCookies[player.Slot][cookieName] = cookieValue;
                }
            }
            else
            {
                string cookieName = $"Equipment-{category.Key}";
                var cookieValue = ClientprefsApi.GetPlayerCookie(player, equipmentCookies[cookieName]);

                if (!string.IsNullOrEmpty(cookieValue))
                    playerCookies[player.Slot][cookieName] = cookieValue;
            }
        }
    }
}