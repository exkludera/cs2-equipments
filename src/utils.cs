using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public bool HasPermission(CCSPlayerController player, string Permission, string Team)
    {
        string permission = Permission.ToLower();
        string team = Team.ToLower();

        bool isTeamValid = (team == "t" || team == "terrorist") && player.Team == CsTeam.Terrorist ||
                    (team == "ct" || team == "counterterrorist") && player.Team == CsTeam.CounterTerrorist ||
                    (team == "" || team == "both" || team == "all");

        return (string.IsNullOrEmpty(permission) || AdminManager.PlayerHasPermissions(player, permission)) && isTeamValid;
    }

    public string DetermineEquipmentType(Equipment equipment)
    {
        if (!string.IsNullOrEmpty(equipment.Model)) return "model";
        if (!string.IsNullOrEmpty(equipment.Particle)) return "particle";
        if (!string.IsNullOrEmpty(equipment.Weapon)) return "weapon";
        return "unknown";
    }

    public string GetEquipmentFilePath(Equipment equipment, string type)
    {
        return type switch
        {
            "model" => equipment.Model,
            "particle" => equipment.Particle,
            "weapon" => equipment.Weapon,
            _ => throw new ArgumentException($"Unknown equipment type: {type}")
        };
    }

    public Dictionary<string, Equipment> GetEquippedItems(CCSPlayerController player)
    {
        var equippedItems = new Dictionary<string, Equipment>();

        if (playerCookies.TryGetValue(player, out var cookies))
        {
            foreach (var category in Config.Categories)
            {
                if (category.Value.AllowMultiple)
                {
                    foreach (var equipment in category.Value.Equipment)
                    {
                        string cookieName = $"Equipment-{category.Key}-{equipment.Name}";
                        if (cookies.TryGetValue(cookieName, out var equippedName) && equippedName == equipment.Name)
                            equippedItems[cookieName] = equipment;
                    }
                }
                else
                {
                    string cookieName = $"Equipment-{category.Key}";
                    if (cookies.TryGetValue(cookieName, out var equippedName))
                    {
                        var equipment = category.Value.Equipment.FirstOrDefault(m => m.Name.Equals(equippedName, StringComparison.OrdinalIgnoreCase));
                        if (equipment != null)
                            equippedItems[cookieName] = equipment;
                    }
                }
            }
        }

        return equippedItems;
    }


    public void EquipBasedOnType(CCSPlayerController player, Equipment equipment, string category)
    {
        if (!string.IsNullOrEmpty(equipment.Model))
        {
            EquipModel(player, equipment.Model, category);
        }
        else if (!string.IsNullOrEmpty(equipment.Particle))
        {
            //EquipParticle(player, equipment.Particle, category);
        }
        else if (!string.IsNullOrEmpty(equipment.Weapon))
        {
            EquipWeapon(player, equipment.Weapon);
        }
    }

    public void UnequipBasedOnType(CCSPlayerController player, string type, string category, string file)
    {
        switch (type)
        {
            case "model":
                UnequipModel(player, category, file);
                break;
            case "particle":
                //UnequipParticle(player, category, file);
                break;
            case "weapon":
                UnequipWeapon(player, file, true);
                break;
            default:
                throw new ArgumentException($"Unknown equipment type: {type}");
        }
    }
}