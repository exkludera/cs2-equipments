using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CS2MenuManager.API.Class;
using CS2MenuManager.API.Interface;

public static class Menu
{
    private static readonly Plugin Instance = Plugin.Instance;

    [CommandHelper(minArgs:0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Open(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null)
            return;

        if (!Utils.HasPermission(player, Instance.Config.Menu.Permission, Instance.Config.Menu.Team))
        {
            player.PrintToChat(Instance.Config.Prefix + Instance.Localizer["chat<nopermission>"]);
            return;
        }

        MainMenu(player).Display(player, 0);
    }

    public static IMenu MainMenu(CCSPlayerController player)
    {
        IMenu mainMenu = MenuManager.MenuByType(Instance.Config.Menu.Type, Instance.Localizer["menu<title>"], Instance);

        foreach (var category in Instance.Config.Categories)
        {
            if (!Utils.HasPermission(player, category.Value.Permission, category.Value.Team.ToLower()))
                continue;

            var display = category.Key;
            mainMenu.AddItem(display, (p, menuOption) =>
            {
                SubMenu(p, category.Value, category.Key, mainMenu);
            });
        }

        return mainMenu;
    }

    public static void SubMenu(CCSPlayerController player, MenuCategory category, string fullPath, IMenu? prevMenu)
    {
        IMenu subMenu = MenuManager.MenuByType(Instance.Config.Menu.Type, fullPath, Instance);

        // Items
        foreach (var equipment in category.Equipment)
        {
            if (!Utils.HasPermission(player, equipment.Permission, equipment.Team))
                continue;

            var cookieName = Cookies.BuildCookieName(fullPath, equipment.Name);
            bool isEquipped = Cookies.playerCookies.TryGetValue(player.Slot, out var cookies) && cookies.ContainsKey(cookieName) && !string.IsNullOrEmpty(cookies[cookieName]);
            string itemTitle = isEquipped ? $"{equipment.Name} {Instance.Localizer["menu<equipped>"]}" : equipment.Name;

            subMenu.AddItem(itemTitle, (p, mo) =>
            {
                Utils.ToggleEquipment(p, fullPath, category, equipment);
                // Reopen current menu to reflect changes
                SubMenu(p, category, fullPath, prevMenu);
            });
        }

        // Subcategories
        if (category.SubCategories != null)
        {
            foreach (var subCategory in category.SubCategories)
            {
                if (!Utils.HasPermission(player, subCategory.Value.Permission, subCategory.Value.Team))
                    continue;

                var newPath = $"{fullPath}-{subCategory.Key}";
                subMenu.AddItem(subCategory.Key, (p, mo) =>
                {
                    SubMenu(p, subCategory.Value, newPath, subMenu);
                });
            }
        }

        subMenu.PrevMenu = prevMenu;
        subMenu.Display(player, 0);
    }
}