using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

public static partial class Menu
{
    public static void Open_Chat(CCSPlayerController player)
    {
        var mainMenu = new ChatMenu(Instance.Localizer["menu<title>"]);

        foreach (var category in Instance.Config.Categories)
        {
            if (!Instance.HasPermission(player, category.Value.Permission.ToLower(), category.Value.Team.ToLower()))
                continue;

            mainMenu.AddMenuOption(category.Key, (player, menuOption) =>
            {
                Open_Chat_SubMenu(player, category.Value, category.Key);
            });
        }

        MenuManager.OpenChatMenu(player, mainMenu);
    }

    public static void Open_Chat_SubMenu(CCSPlayerController player, MenuCategory category, string title)
    {
        var subMenu = new ChatMenu(title);

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
                Open_Chat(player);
            });
        }

        MenuManager.OpenChatMenu(player, subMenu);
    }
}