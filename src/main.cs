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
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "exkludera";

    public static Plugin Instance { get; set; } = new();

    public readonly PluginCapability<IClientprefsApi> g_PluginCapability = new("Clientprefs");
    public IClientprefsApi? ClientprefsApi;

    public Dictionary<string, int> equipmentCookies = new();
    public Dictionary<int, Dictionary<string, string>> playerCookies = new();

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        foreach (var category in Config.Categories.Values)
        {
            foreach (var equipment in category.Equipment)
            {
                if (!string.IsNullOrEmpty(equipment.Model))
                    manifest.AddResource(equipment.Model);

                if (!string.IsNullOrEmpty(equipment.Particle))
                    manifest.AddResource(equipment.Particle);

                if (!string.IsNullOrEmpty(equipment.Weapon))
                {
                    var weaponpart = equipment.Weapon.Split(':');
                    if (weaponpart.Length != 2)
                        continue;

                    manifest.AddResource(weaponpart[1]);
                }
            }
        }
    }

    public override void Load(bool hotReload)
    {
        Instance = this;

        RegisterListener<OnTick>(OnTick);
        RegisterListener<OnEntityCreated>(OnEntityCreated);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        RegisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        RegisterEventHandler<EventItemEquip>(EventItemEquip);

        Menu.Load(hotReload);

        foreach (var command in Config.MenuCommands.Split(','))
            AddCommand(command.Trim(), "Open Equipments Menu", Menu.Command_OpenMenus!);
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnTick>(OnTick);
        RemoveListener<OnEntityCreated>(OnEntityCreated);
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        DeregisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        DeregisterEventHandler<EventItemEquip>(EventItemEquip);

        Menu.Unload();

        foreach (var command in Config.MenuCommands.Split(','))
            RemoveCommand(command.Trim(), Menu.Command_OpenMenus!);

        if (ClientprefsApi == null)
            return;

        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;
    }

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
