using CounterStrikeSharp.API.Core;

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{orange}[Equipments]{default}";
    public string MenuCommands { get; set; } = "css_equipments,css_equipment";
    public string MenuType { get; set; } = "html";
    public bool MenuBackButton { get; set; } = false;
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
    public Dictionary<string, MenuCategory> Menu { get; set; } = new Dictionary<string, MenuCategory>
    {
        {
            "1", new MenuCategory
            {
                Title = "Hats",
                Permission = "",
                Team = "",
                Models = new List<Model>
                {
                    new Model
                    {
                        Name = "hat",
                        File = "models/hat.vmdl"
                    }
                }
            }
        },
        {
            "2", new MenuCategory
            {
                Title = "Backpacks",
                Permission = "",
                Team = "",
                Models = new List<Model>
                {
                    new Model
                    {
                        Name = "backpack",
                        File = "models/backpack.vmdl"
                    }
                }
            }
        }
    };
}

public class MenuCategory
{
    public string Title { get; set; } = "Subcategory Title";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
    public List<Model> Models { get; set; } = new List<Model>();
}

public class Model
{
    public string Name { get; set; } = "Model Name";
    public string File { get; set; } = "models/example.vmdl";
    public string Permission { get; set; } = "";
    public string Team { get; set; } = "";
}
