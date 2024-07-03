using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace Equipments;

public class EquipmentsConfig : BasePluginConfig
{
    public class Config_Database
    {
        public string Host { get; set; } = string.Empty;
        public uint Port { get; set; } = 3306;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string DBTable { get; set; } = "equipments";
    }

    public class Config_Command
    {
        public string[] OpenMenu { get; set; } = ["equipments", "equipment", "hats", "wings"];
        public string[] ResetPlayer { get; set; } = ["equipments-resetplayer"];
        public string[] ResetDatabase { get; set; } = ["equipments-resetdatabase"];
    }

    public class Config_Settings
    {
        public string Flag { get; set; } = "@css/reservation";
        public bool HudMenu { get; set; }
    }

    [JsonPropertyName("Database")] public Config_Database Database { get; set; } = new Config_Database();
    [JsonPropertyName("Commands")] public Config_Command Commands { get; set; } = new Config_Command();
    [JsonPropertyName("Settings")] public Config_Settings Settings { get; set; } = new Config_Settings();
    [JsonPropertyName("Items")] public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Items { get; set; } = [];
}