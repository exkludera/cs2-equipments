using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

public static partial class Menu
{
    private static Plugin Instance = Plugin.Instance;

    public static readonly Dictionary<int, WasdMenuPlayer> WasdPlayers = [];

    public static void Load(bool hotReload)
    {
        Instance.RegisterListener<OnTick>(OnTick);

        Instance.RegisterEventHandler<EventPlayerActivate>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            WasdPlayers[player.Slot] = new WasdMenuPlayer
            {
                player = player,
                Buttons = 0
            };

            return HookResult.Continue;
        });

        Instance.RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            WasdPlayers.Remove(player.Slot);

            return HookResult.Continue;
        });

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player.IsBot)
                    continue;

                WasdPlayers[player.Slot] = new WasdMenuPlayer
                {
                    player = player,
                    Buttons = player.Buttons
                };
            }
        }
    }

    public static void Unload()
    {
        Instance.RemoveListener<OnTick>(OnTick);
    }

    public static void OnTick()
    {
        foreach (WasdMenuPlayer? player in WasdPlayers.Values.Where(p => p.MainMenu != null))
        {
            if ((player.Buttons & PlayerButtons.Forward) == 0 && (player.player.Buttons & PlayerButtons.Forward) != 0)
                player.ScrollUp();

            else if ((player.Buttons & PlayerButtons.Back) == 0 && (player.player.Buttons & PlayerButtons.Back) != 0)
                player.ScrollDown();

            else if ((player.Buttons & PlayerButtons.Moveright) == 0 && (player.player.Buttons & PlayerButtons.Moveright) != 0)
                player.Choose();

            else if ((player.Buttons & PlayerButtons.Moveleft) == 0 && (player.player.Buttons & PlayerButtons.Moveleft) != 0)
                player.CloseSubMenu();

            if (((long)player.player.Buttons & 8589934592) == 8589934592)
                player.OpenMainMenu(null);

            player.Buttons = player.player.Buttons;

            if (player.CenterHtml != "")
            {
                Server.NextFrame(() =>
                    player.player.PrintToCenterHtml(player.CenterHtml)
                );
            }
        }
    }

    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Command_OpenMenus(CCSPlayerController player, CommandInfo info)
    {
        if (!Instance.HasPermission(player, Instance.Config.Permission.ToLower(), Instance.Config.Team.ToLower()))
        {
            player.PrintToChat(Instance.Config.Prefix + Instance.Localizer["NoPermission"]);
            return;
        }

        switch (Instance.Config.MenuType.ToLower())
        {
            case "chat":
            case "text":
                Open_Chat(player);
                break;
            case "html":
            case "center":
            case "centerhtml":
            case "hud":
                Open_HTML(player);
                break;
            case "wasd":
            case "wasdmenu":
                Open_WASD(player);
                break;
            default:
                Open_HTML(player);
                break;
        }
    }

    private static void ExecuteOption(CCSPlayerController player, Equipment equipment, string category)
    {
        bool allowMultiple = Instance.Config.Categories[category].AllowMultiple;

        string cookieName = allowMultiple ?
            $"Equipment-{category}-{equipment.Name}" :
            $"Equipment-{category}";

        string equippedType = Instance.DetermineEquipmentType(equipment);
        string filePath = Instance.GetEquipmentFilePath(equipment, equippedType);

        if (Instance.playerCookies.TryGetValue(player, out var playerCookieDict))
        {
            if (!allowMultiple)
            {
                if (playerCookieDict.TryGetValue(cookieName, out var currentEquippedName) && currentEquippedName == equipment.Name)
                {
                    Instance.UnequipBasedOnType(player, equippedType, category, filePath);
                    playerCookieDict.Remove(cookieName);
                    player.PrintToChat(Instance.Config.Prefix + Instance.Localizer[$"chat<unequip>", equipment.Name]);
                    return;
                }
                else
                {
                    var keysToRemove = new List<string>();

                    foreach (var key in playerCookieDict.Keys.ToList())
                    {
                        if (key.StartsWith($"Equipment-{category}"))
                        {
                            Instance.UnequipBasedOnType(player, equippedType, category, null!);
                            keysToRemove.Add(key);
                            //player.PrintToChat(Instance.Config.Prefix + Instance.Localizer[$"chat<unequip>", playerCookieDict[key]]);
                        }
                    }

                    foreach (var key in keysToRemove)
                        playerCookieDict.Remove(key);
                }
            }
            else
            {
                if (playerCookieDict.TryGetValue(cookieName, out var currentEquippedName) && currentEquippedName == equipment.Name)
                {
                    Instance.UnequipBasedOnType(player, equippedType, category, filePath);
                    playerCookieDict.Remove(cookieName);
                    player.PrintToChat(Instance.Config.Prefix + Instance.Localizer[$"chat<unequip>", equipment.Name]);
                    return;
                }
            }
        }

        if (Instance.equipmentCookies.TryGetValue(cookieName, out var cookieId))
            Instance.ClientprefsApi?.SetPlayerCookie(player, cookieId, equipment.Name);

        else Instance.Logger.LogError($"Cookie ID not found for key {cookieName}. Ensure the cookies are correctly created and registered.");

        Instance.playerCookies[player][cookieName] = equipment.Name;
        Instance.EquipBasedOnType(player, equipment, category);

        player.PrintToChat(Instance.Config.Prefix + Instance.Localizer["chat<equip>", equipment.Name]);
    }
}