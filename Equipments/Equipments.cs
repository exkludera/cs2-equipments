using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;

using EquipmentsAPI;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public class Equipments : BasePlugin, IPluginConfig<EquipmentsConfig>
{
    public override string ModuleName => "Equipments";
    public override string ModuleVersion => "0.0.4";
    public override string ModuleAuthor => "exkludera";

    public EquipmentsConfig Config { get; set; } = new EquipmentsConfig();
    public List<Equipments_Items> GlobalEquipmentsItems { get; set; } = [];
    public List<Equipments_Item_Types> GlobalEquipmentsItemTypes { get; set; } = [];
    public int GlobalTickrate { get; set; } = 0;
    public static Equipments Instance { get; set; } = new();

    public override void Load(bool hotReload)
    {
        Capabilities.RegisterPluginCapability(IEquipmentsAPI.Capability, () => new EquipmentsAPI());

        Instance = this;

        Event.Load();
        Command.Load();

        Item_Hat.OnPluginStart();
        Item_Wings.OnPluginStart();
        Item_Backpack.OnPluginStart();
        Item_Trail.OnPluginStart();

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                Database.LoadPlayer(player);
            }
        }
    }

    public override void Unload(bool hotReload)
    {
        Event.Unload();
    }

    public void OnConfigParsed(EquipmentsConfig config)
    {
        if (string.IsNullOrEmpty(config.Database.Host) || string.IsNullOrEmpty(config.Database.DBName) || string.IsNullOrEmpty(config.Database.User))
            throw new Exception("Setup Database in config!");

        Task.Run(async () => {
            await Database.CreateDatabaseAsync(config);
        });

        Config = config;
    }
}
