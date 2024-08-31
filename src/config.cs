using CounterStrikeSharp.API.Core;

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{orange}[Equipments]{default}";
    public string MenuCommands { get; set; } = "css_equipments,css_equipment";
    public string MenuType { get; set; } = "html";
    public bool MenuBackButton { get; set; } = false;
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";

    public Dictionary<string, MenuCategory> Categories { get; set; } = new Dictionary<string, MenuCategory>
    {
        {
            "Hats", new MenuCategory
            {
                AllowMultiple = false,
                Permission = "@css/reservation",
                Team = "CT",
                Equipment = new List<Equipment>
                {
                    new Equipment
                    {
                        Name = "Hat #1",
                        Model = "models/hat_1.vmdl"
                    },
                    new Equipment
                    {
                        Name = "Hat #2",
                        Model = "models/hat_2.vmdl"
                    }
                }
            }
        },
        {
            "Particles", new MenuCategory
            {
                AllowMultiple = true,
                Permission = "@css/generic",
                Team = "T",
                Equipment = new List<Equipment>
                {
                    new Equipment
                    {
                        Name = "Particle #1",
                        Particle = "particles/particle_1.vpcf"
                    },
                    new Equipment
                    {
                        Name = "Particle #2",
                        Particle = "particles/particle_2.vpcf"
                    }
                }
            }
        },
        {
            "Weapons", new MenuCategory
            {
                AllowMultiple = true,
                Permission = "@css/root",
                Equipment = new List<Equipment>
                {
                    new Equipment
                    {
                        Name = "Custom AWP",
                        Weapon = "weapon_awp:models/awp.vmdl"
                    },
                    new Equipment
                    {
                        Name = "Custom AK47",
                        Weapon = "weapon_ak47:models/ak47.vmdl"
                    }
                }
            }
        }
    };
}

public class MenuCategory
{
    public bool AllowMultiple { get; set; } = false;
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
    public List<Equipment> Equipment { get; set; } = new List<Equipment>();
}

public class Equipment
{
    public string Name { get; set; } = "Equipment Name";
    public string Model { get; set; } = "";
    public string Particle { get; set; } = "";
    public string Weapon { get; set; } = "";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
}