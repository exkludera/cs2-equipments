using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

public static partial class Menu
{
    public static void Open_HTML(CCSPlayerController player)
    {
        var mainMenu = new CenterHtmlMenu(Instance.Localizer["menu<title>"], Instance);

        foreach (var category in Instance.Config.Categories)
        {
            if (!Instance.HasPermission(player, category.Value.Permission.ToLower(), category.Value.Team.ToLower()))
                continue;

            mainMenu.AddMenuOption(category.Key, (player, menuOption) =>
            {
                Open_HTML_SubMenu(player, category.Value, category.Key);
            });
        }

        MenuManager.OpenCenterHtmlMenu(Instance, player, mainMenu);
    }

    public static void Open_HTML_SubMenu(CCSPlayerController player, MenuCategory category, string title)
    {
        var subMenu = new CenterHtmlMenu(title, Instance);

        foreach (var equipment in category.Equipment)
        {
            if (!Instance.HasPermission(player, equipment.Permission.ToLower(), equipment.Team.ToLower()))
                continue;

            subMenu.AddMenuOption(equipment.Name, (player, menuOption) =>
            {
                ExecuteOption(player, equipment, title);
            });
        }

        if (Instance.Config.MenuBackButton)
        {
            subMenu.AddMenuOption(Instance.Localizer["menu<back>"], (player, menuOption) =>
            {
                Open_HTML(player);
            });
        }

        MenuManager.OpenCenterHtmlMenu(Instance, player, subMenu);
    }
}