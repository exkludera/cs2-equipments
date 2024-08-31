using CounterStrikeSharp.API.Core;

public static class WasdManager
{
    public static void OpenMainMenu(CCSPlayerController? player, IWasdMenu? menu)
    {
        if (player == null)
            return;
        Menu.WasdPlayers[player.Slot].OpenMainMenu((WasdMenu?)menu);
    }

    public static void CloseMenu(CCSPlayerController? player)
    {
        if (player == null)
            return;
        Menu.WasdPlayers[player.Slot].OpenMainMenu(null);
    }

    public static void CloseSubMenu(CCSPlayerController? player)
    {
        if (player == null)
            return;
        Menu.WasdPlayers[player.Slot].CloseSubMenu();
    }

    public static void CloseAllSubMenus(CCSPlayerController? player)
    {
        if (player == null)
            return;
        Menu.WasdPlayers[player.Slot].CloseAllSubMenus();
    }

    public static void OpenSubMenu(CCSPlayerController? player, IWasdMenu? menu)
    {
        if (player == null)
            return;
        Menu.WasdPlayers[player.Slot].OpenSubMenu(menu);
    }

    public static IWasdMenu CreateMenu(string title = "")
    {
        WasdMenu menu = new()
        {
            Title = title
        };
        return menu;
    }
}