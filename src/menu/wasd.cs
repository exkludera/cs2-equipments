using CounterStrikeSharp.API.Core;

public static partial class Menu
{
    public static void Open_WASD(CCSPlayerController player)
    {
        var mainMenu = WasdManager.CreateMenu(Instance.Localizer["menu<title>"]);

        foreach (var category in Instance.Config.Menu.Values)
        {
            if (!Instance.HasPermission(player, category.Permission.ToLower(), category.Team.ToLower()))
                continue;

            mainMenu.Add(category.Title, (player, menuOption) =>
            {
                Open_WASD_SubMenu(player, category);
            });
        }

        WasdManager.OpenMainMenu(player, mainMenu);
    }

    public static void Open_WASD_SubMenu(CCSPlayerController player, MenuCategory category)
    {
        var subMenu = WasdManager.CreateMenu(category.Title);

        foreach (var model in category.Models)
        {
            if (!Instance.HasPermission(player, model.Permission.ToLower(), model.Team.ToLower()))
                continue;

            subMenu.Add(model.Name, (player, menuOption) =>
            {
                ExecuteOption(player, model, category.Title);
            });
        }

        if (Instance.Config.MenuBackButton)
        {
            subMenu.Add(Instance.Localizer["menu<back>"], (player, menuOption) =>
            {
                Open_WASD(player);
            });
        }

        WasdManager.OpenMainMenu(player, subMenu);
    }
}