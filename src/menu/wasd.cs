using CounterStrikeSharp.API.Core;

public static partial class Menu
{
    public static void Open_WASD(CCSPlayerController player)
    {
        var mainMenu = WasdManager.CreateMenu(Instance.Localizer["menu<title>"]);

        foreach (var category in Instance.Config.Categories)
        {
            if (!Instance.HasPermission(player, category.Value.Permission.ToLower(), category.Value.Team.ToLower()))
                continue;

            mainMenu.Add(category.Key, (player, menuOption) =>
            {
                Open_WASD_SubMenu(player, category.Value, category.Key);
            });
        }

        WasdManager.OpenMainMenu(player, mainMenu);
    }

    public static void Open_WASD_SubMenu(CCSPlayerController player, MenuCategory category, string title)
    {
        var subMenu = WasdManager.CreateMenu(title);

        foreach (var equipment in category.Equipment)
        {
            if (!Instance.HasPermission(player, equipment.Permission.ToLower(), equipment.Team.ToLower()))
                continue;

            subMenu.Add(equipment.Name, (player, menuOption) =>
            {
                ExecuteOption(player, equipment, title);
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