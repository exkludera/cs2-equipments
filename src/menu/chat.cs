using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

public static partial class Menu
{
    public static class Chat
    {
        public static void MainMenu(CCSPlayerController player)
        {
            var mainMenu = new ChatMenu(Instance.Localizer["menu<title>"]);

            foreach (var category in Instance.Config.Categories)
            {
                if (!Instance.HasPermission(player, category.Value.Permission.ToLower(), category.Value.Team.ToLower()))
                    continue;

                mainMenu.AddMenuOption(category.Key, (player, menuOption) =>
                {
                    SubMenu(player, category.Value, category.Key);
                });
            }

            MenuManager.OpenChatMenu(player, mainMenu);
        }

        public static void SubMenu(CCSPlayerController player, MenuCategory category, string title)
        {
            var subMenu = new ChatMenu(title);

            var equippedItems = Instance.GetEquippedItems(player);

            foreach (var equipment in category.Equipment)
            {
                if (!Instance.HasPermission(player, equipment.Permission.ToLower(), equipment.Team.ToLower()))
                    continue;

                bool isEquipped = equippedItems.Values.Any(e => e.Name.Equals(equipment.Name, StringComparison.OrdinalIgnoreCase));

                string itemTitle = isEquipped
                    ? $"{equipment.Name} {Instance.Localizer["menu<equipped>"]}"
                    : $"{equipment.Name}";

                subMenu.AddMenuOption(itemTitle, (player, menuOption) =>
                {
                    ExecuteOption(player, equipment, title);

                    if (category.MenuEquipOpenMain) MainMenu(player);
                    else SubMenu(player, category, title);
                });
            }

            if (category.MenuBackButton)
            {
                subMenu.AddMenuOption(Instance.Localizer["menu<back>"], (player, menuOption) =>
                {
                    MainMenu(player);
                });
            }

            MenuManager.OpenChatMenu(player, subMenu);
        }
    }
}