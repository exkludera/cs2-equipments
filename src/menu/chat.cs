using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;

public static partial class Menu
{
    public static void Open_Chat(CCSPlayerController player)
    {
        var mainMenu = new ChatMenu(Instance.Localizer["menu<title>"]);

        foreach (var category in Instance.Config.Menu.Values)
        {
            if (!Instance.HasPermission(player, category.Permission.ToLower(), category.Team.ToLower()))
                continue;

            mainMenu.AddMenuOption(category.Title, (player, menuOption) =>
            {
                Open_Chat_SubMenu(player, category);
            });
        }

        MenuManager.OpenChatMenu(player, mainMenu);
    }

    public static void Open_Chat_SubMenu(CCSPlayerController player, MenuCategory category)
    {
        var subMenu = new ChatMenu(category.Title);

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

        MenuManager.OpenChatMenu(player, subMenu);
    }
}