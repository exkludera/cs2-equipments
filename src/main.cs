using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Equipments";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "exkludera";

    public static Plugin Instance { get; set; } = new();

    public override void Load(bool hotReload)
    {
        Instance = this;

        Events.Register();

        Commands.Register();

        Server.NextWorldUpdate(() => Cookies.Register(hotReload));
    }

    public override void Unload(bool hotReload)
    {
        Events.Unregister();

        Commands.Unregister();

        Cookies.Unregister();
    }

    public Config Config { get; set; } = new();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(Config.Prefix);
    }
}
