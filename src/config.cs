using CounterStrikeSharp.API.Core;

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{orange}[Equipments]{default}";
    public class Config_Menu
    {
        public string Type { get; set; } = "CenterHtmlMenu";
        public List<string> Command { get; set; } = new() { "css_equipment", "css_equipments" };
        public List<string> Permission { get; set; } = new() { "@css/reservation" };
        public string Team { get; set; } = "";
    }
    public Config_Menu Menu { get; set; } = new Config_Menu();

    public Dictionary<string, MenuCategory> Categories { get; set; } = new Dictionary<string, MenuCategory>
    {
        {
            "Hats", new MenuCategory
            {
                AllowMultiple = false,
                Permission = [ "@css/reservation" ],
                Team = "CT",
                Equipment = new List<Equipment>
                {
                    new Equipment { Name = "Hat #1", Model = "models/hat_1.vmdl" },
                    new Equipment { Name = "Hat #2", Model = "models/hat_2.vmdl" }
                }
            }
        },
        {
            "Particles", new MenuCategory
            {
                AllowMultiple = true,
                Permission = [ "@css/generic" ],
                Team = "T",
                Equipment = new List<Equipment>
                {
                    new Equipment { Name = "Particle #1", Particle = "particles/particle_1.vpcf" },
                    new Equipment { Name = "Particle #2", Particle = "particles/particle_2.vpcf" }
                }
            }
        },
        {
            "Weapons", new MenuCategory
            {
                AllowMultiple = true,
                Permission = [ "@css/root" ],
                Equipment = new List<Equipment>
                {
                    new Equipment { Name = "Knife #1", Weapon = "weapon_knife:weapon_knife_subclass1" },
                    new Equipment { Name = "Knife #2", Weapon = "weapon_knife:weapon_knife_subclass2" },
                },
                SubCategories = new Dictionary<string, MenuCategory>
                {
                    {
                        "CT Examples", new MenuCategory
                        {
                            Equipment = new List<Equipment>
                            {
                                new Equipment { Name = "AWP CT", Weapon = "weapon_awp:weapon_awp_subclass_ct" }
                            }
                        }
                    },
                    {
                        "T Examples", new MenuCategory
                        {
                            Equipment = new List<Equipment>
                            {
                                new Equipment { Name = "AWP T", Weapon = "weapon_awp:weapon_awp_subclass_t" }
                            }
                        }
                    }
                }
            }
        }
    };
}

public class MenuCategory
{
    public List<string> Command { get; set; } = new() { "" };
    public bool AllowMultiple { get; set; } = false;
    public List<string> Permission { get; set; } = new() { "" };
    public string Team { get; set; } = "";
    public List<Equipment> Equipment { get; set; } = new();
    public Dictionary<string, MenuCategory> SubCategories { get; set; } = new();
}

public class Equipment
{
    public string Name { get; set; } = "Equipment Name";
    public List<string> Permission { get; set; } = new() { "" };
    public string Team { get; set; } = "";
    public string Model { get; set; } = "";
    public string Particle { get; set; } = "";
    public string Weapon { get; set; } = "";
}