using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

public static partial class Menu
{
    public static void Open_HTML(CCSPlayerController player)
    {
        var mainMenu = new CenterHtmlMenu(Instance.Localizer["menu<title>"], Instance);

        foreach (var category in Instance.Config.Menu.Values)
        {
            if (!Instance.HasPermission(player, category.Permission.ToLower(), category.Team.ToLower()))
                continue;

            mainMenu.AddMenuOption(category.Title, (player, menuOption) =>
            {
                Open_HTML_SubMenu(player, category);
            });
        }

        MenuManager.OpenCenterHtmlMenu(Instance, player, mainMenu);
    }

    public static void Open_HTML_SubMenu(CCSPlayerController player, MenuCategory category)
    {
        var subMenu = new CenterHtmlMenu(category.Title, Instance);

        foreach (var model in category.Models)
        {
            if (!Instance.HasPermission(player, model.Permission.ToLower(), model.Team.ToLower()))
                continue;

            subMenu.AddMenuOption(model.Name, (player, menuOption) =>
            {
                ExecuteOption(player, model, category.Title);
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