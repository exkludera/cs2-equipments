using CounterStrikeSharp.API.Core;

public static class MenuWASD
{
    private static Plugin Instance = Plugin.Instance;

    public static void Open_MainMenu(CCSPlayerController player)
    {
        var mainMenu = WasdManager.CreateMenu(Instance.Localizer["menu<title>"]);

        foreach (var category in Instance.Config.Categories)
        {
            if (!Instance.HasPermission(player, category.Value.Permission.ToLower(), category.Value.Team.ToLower()))
                continue;

            mainMenu.Add(category.Key, (player, menuOption) =>
            {
                Open_SubMenu(player, category.Value, category.Key);
            });
        }

        WasdManager.OpenMainMenu(player, mainMenu);
    }

    public static void Open_SubMenu(CCSPlayerController player, MenuCategory category, string title)
    {
        var subMenu = WasdManager.CreateMenu(title);

        var equippedItems = Instance.GetEquippedItems(player);

        foreach (var equipment in category.Equipment)
        {
            if (!Instance.HasPermission(player, equipment.Permission.ToLower(), equipment.Team.ToLower()))
                continue;

            bool isEquipped = equippedItems.Values.Any(e => e.Name.Equals(equipment.Name, StringComparison.OrdinalIgnoreCase));

            string itemTitle = isEquipped
                ? $"{equipment.Name} {Instance.Localizer["menu<equipped>"]}"
                : $"{equipment.Name}";

            subMenu.Add(itemTitle, (player, menuOption) =>
            {
                Menu.ExecuteOption(player, equipment, title);
                Open_SubMenu(player, category, title);
            });
        }

        if (Instance.Config.MenuBackButton)
        {
            subMenu.Add(Instance.Localizer["menu<back>"], (player, menuOption) =>
            {
                Open_MainMenu(player);
            });
        }

        WasdManager.OpenMainMenu(player, subMenu);
    }
}