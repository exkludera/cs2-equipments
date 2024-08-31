using Clientprefs.API;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Equipments";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "exkludera";

    public static Plugin Instance { get; set; } = new();

    public readonly PluginCapability<IClientprefsApi> g_PluginCapability = new("Clientprefs");
    public IClientprefsApi? ClientprefsApi;

    public Dictionary<string, int> equipmentCookies = new();
    public Dictionary<CCSPlayerController, Dictionary<string, string>> playerCookies = new();
    public Dictionary<CCSPlayerController, Dictionary<string, CBaseModelEntity>> playerEquipment = new();

    public override void Load(bool hotReload)
    {
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        RegisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);

        Instance = this;

        Menu.Load(hotReload);

        foreach (var command in Config.MenuCommands.Split(','))
            AddCommand(command.Trim(), "Open Equipments Menu", Menu.Command_OpenMenus!);
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        DeregisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);

        foreach (var command in Config.MenuCommands.Split(','))
            RemoveCommand(command.Trim(), Menu.Command_OpenMenus!);

        if (ClientprefsApi == null) return;
        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        try
        {
            ClientprefsApi = g_PluginCapability.Get();

            if (ClientprefsApi == null) return;
            ClientprefsApi.OnDatabaseLoaded += OnClientprefDatabaseReady;
            ClientprefsApi.OnPlayerCookiesCached += OnPlayerCookiesCached;
        }
        catch (Exception ex)
        {
            Logger.LogError("[Trails] Fail load ClientprefsApi! | " + ex.Message);
            throw new Exception("[Trails] Fail load ClientprefsApi! | " + ex.Message);
        }

        if (hotReload && ClientprefsApi != null)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers().Where(p => !p.IsBot))
            {
                if (!playerCookies.ContainsKey(player))
                    playerCookies[player] = new Dictionary<string, string>();

                if (!ClientprefsApi!.ArePlayerCookiesCached(player)) continue;

                foreach (var item in equipmentCookies)
                {
                    var cookieValue = ClientprefsApi.GetPlayerCookie(player, item.Value);

                    if (!string.IsNullOrEmpty(cookieValue))
                        playerCookies[player][item.Key] = cookieValue;
                }
            }
        }
    }

    public void OnClientprefDatabaseReady()
    {
        if (ClientprefsApi == null) return;

        foreach (var category in Config.Menu.Values)
        {
            string cookieName = $"Equipment-{category.Title}";
            int cookieId = ClientprefsApi.RegPlayerCookie(cookieName, $"Which Equipment in {category.Title}", CookieAccess.CookieAccess_Protected);

            if (cookieId == -1)
            {
                Logger.LogError($"[Clientprefs] Failed to register/load Cookie for {cookieName}");
                return;
            }
            equipmentCookies[cookieName] = cookieId;
        }
    }

    public void OnPlayerCookiesCached(CCSPlayerController player)
    {
        if (ClientprefsApi == null) return;

        if (!playerCookies.ContainsKey(player))
            playerCookies[player] = new Dictionary<string, string>();

        foreach (var item in equipmentCookies)
        {
            var cookieValue = ClientprefsApi.GetPlayerCookie(player, item.Value);

            if (!string.IsNullOrEmpty(cookieValue))
                playerCookies[player][item.Key] = cookieValue;
        }
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        foreach (var category in Config.Menu.Values)
        {
            foreach (var model in category.Models)
            {
                if (!string.IsNullOrEmpty(model.File))
                    manifest.AddResource(model.File);
            }
        }
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }
}
