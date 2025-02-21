using CounterStrikeSharp.API.Core;
using CS2ScreenMenuAPI;
using CS2ScreenMenuAPI.Internal;

public static partial class Menu
{
    public static class Screen
    {
        public static void MainMenu(CCSPlayerController player)
        {
            ScreenMenu mainMenu = new ScreenMenu(Instance.Localizer["menu<title>"], Instance);

            foreach (var category in Instance.Config.Categories)
            {
                if (!Instance.HasPermission(player, category.Value.Permission.ToLower(), category.Value.Team.ToLower()))
                    continue;

                mainMenu.AddOption(category.Key, (player, menuOption) =>
                {
                    SubMenu(player, category.Value, category.Key);
                });
            }

            MenuAPI.OpenMenu(Instance, player, mainMenu);
        }

        public static void SubMenu(CCSPlayerController player, MenuCategory category, string title)
        {
            ScreenMenu subMenu = new ScreenMenu(title, Instance);

            var equippedItems = Instance.GetEquippedItems(player);

            foreach (var equipment in category.Equipment)
            {
                if (!Instance.HasPermission(player, equipment.Permission.ToLower(), equipment.Team.ToLower()))
                    continue;

                bool isEquipped = equippedItems.Values.Any(e => e.Name.Equals(equipment.Name, StringComparison.OrdinalIgnoreCase));

                string itemTitle = isEquipped
                    ? $"{equipment.Name} {Instance.Localizer["menu<equipped>"]}"
                    : $"{equipment.Name}";

                subMenu.AddOption(itemTitle, (player, menuOption) =>
                {
                    ExecuteOption(player, equipment, title);
                    SubMenu(player, category, title);
                });
            }

            if (Instance.Config.MenuBackButton)
            {
                subMenu.AddOption(Instance.Localizer["menu<back>"], (player, menuOption) =>
                {
                    MainMenu(player);
                });
            }

            MenuAPI.OpenMenu(Instance, player, subMenu);
        }
    }
}