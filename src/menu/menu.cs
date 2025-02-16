using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

public static partial class Menu
{
    static Plugin Instance = Plugin.Instance;
    static Config Config = Instance.Config;
    static IStringLocalizer Localizer = Instance.Localizer;

    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Open(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null)
            return;

        if (!Instance.HasPermission(player, Config.Permission.ToLower(), Config.Team.ToLower()))
        {
            player.PrintToChat(Config.Prefix + Localizer["NoPermission"]);
            return;
        }

        switch (Instance.Config.MenuType.ToLower())
        {
            case "chat":
            case "text":
                Chat.MainMenu(player);
                break;
            case "html":
            case "center":
            case "centerhtml":
            case "hud":
                HTML.MainMenu(player);
                break;
            case "wasd":
            case "wasdmenu":
                WASD.MainMenu(player);
                break;
            case "screen":
            case "screenmenu":
                Screen.MainMenu(player);
                break;
            default:
                HTML.MainMenu(player);
                break;
        }
    }

    public static void ExecuteOption(CCSPlayerController player, Equipment equipment, string category)
    {
        bool allowMultiple = Config.Categories[category].AllowMultiple;

        string cookieName = allowMultiple ?
            $"Equipment-{category}-{equipment.Name}" :
            $"Equipment-{category}";

        string equippedType = Instance.DetermineEquipmentType(equipment);
        string filePath = Instance.GetEquipmentFilePath(equipment, equippedType);

        if (Instance.playerCookies.TryGetValue(player.Slot, out var playerCookieDict))
        {
            if (!allowMultiple)
            {
                if (playerCookieDict.TryGetValue(cookieName, out var currentEquippedName) && currentEquippedName == equipment.Name)
                {
                    if (Instance.equipmentCookies.TryGetValue(cookieName, out var EmptycookieId))
                        Instance.ClientprefsApi?.SetPlayerCookie(player, EmptycookieId, "empty");

                    Instance.UnequipBasedOnType(player, equippedType, category, filePath);
                    playerCookieDict.Remove(cookieName);
                    player.PrintToChat(Config.Prefix + Localizer[$"chat<unequip>", equipment.Name]);
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
                            //player.PrintToChat(Config.Prefix + Instance.Localizer[$"chat<unequip>", playerCookieDict[key]]);
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
                    if (Instance.equipmentCookies.TryGetValue(cookieName, out var EmptycookieId))
                        Instance.ClientprefsApi?.SetPlayerCookie(player, EmptycookieId, "empty");

                    Instance.UnequipBasedOnType(player, equippedType, category, filePath);
                    playerCookieDict.Remove(cookieName);
                    player.PrintToChat(Config.Prefix + Localizer[$"chat<unequip>", equipment.Name]);
                    return;
                }
            }
        }

        if (Instance.equipmentCookies.TryGetValue(cookieName, out var cookieId))
            Instance.ClientprefsApi?.SetPlayerCookie(player, cookieId, equipment.Name);

        else Instance.Logger.LogError($"Cookie ID not found for key {cookieName}. Ensure the cookies are correctly created and registered.");

        Instance.playerCookies[player.Slot][cookieName] = equipment.Name;
        Instance.EquipBasedOnType(player, equipment, category);

        player.PrintToChat(Config.Prefix + Localizer["chat<equip>", equipment.Name]);
    }
}