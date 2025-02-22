using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using static CounterStrikeSharp.API.Core.Listeners;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Equipments";
    public override string ModuleVersion => "1.0.4";
    public override string ModuleAuthor => "exkludera";

    public static Plugin Instance { get; set; } = new();

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }

    public override void Load(bool hotReload)
    {
        Instance = this;

        RegisterListener<OnTick>(OnTick);
        RegisterListener<OnEntityCreated>(OnEntityCreated);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        RegisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        RegisterEventHandler<EventItemEquip>(EventItemEquip);

        foreach (var command in Config.MenuCommands.Split(','))
            AddCommand(command.Trim(), "", Menu.Open);
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnTick>(OnTick);
        RemoveListener<OnEntityCreated>(OnEntityCreated);
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);
        DeregisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        DeregisterEventHandler<EventItemEquip>(EventItemEquip);

        foreach (var command in Config.MenuCommands.Split(','))
            RemoveCommand(command.Trim(), Menu.Open);

        UnloadClientprefs();
    }
}
