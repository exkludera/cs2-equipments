using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Menu;
using System.Text;

using static Equipments.Equipments;

namespace Equipments;

public static class MenuHud
{
    public static void AddMenuOption(CCSPlayerController player, CenterHtmlMenu menu, Action<CCSPlayerController, ChatMenuOption> onSelect, string display, params object[] args)
    {
        using (new WithTemporaryCulture(player.GetLanguage()))
        {
            StringBuilder builder = new();
            builder.AppendFormat(Instance.Localizer[display, args]);

            menu.AddMenuOption(builder.ToString(), onSelect);
        }
    }

    public static void DisplayMenu(CCSPlayerController player, bool inventory)
    {
        using (new WithTemporaryCulture(player.GetLanguage()))
        {
            StringBuilder builder = new();
            builder.AppendFormat(Instance.Localizer["menu<title>"]);

            CenterHtmlMenu menu = new(builder.ToString(), Instance);
            menu.ExitButton = true;

            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> category in Instance.Config.Items)
            {
                StringBuilder builderkey = new();
                builderkey.AppendFormat(Instance.Localizer[$"menu_item<{category.Key}>"]);

                menu.AddMenuOption(builderkey.ToString(), (CCSPlayerController player, ChatMenuOption option) =>
                {
                    DisplayItems(player, builderkey.ToString(), category.Value, inventory);
                });
            }

            MenuManager.OpenCenterHtmlMenu(Instance, player, menu);
        }
    }

    public static void DisplayItems(CCSPlayerController player, string key, Dictionary<string, Dictionary<string, string>> items, bool inventory)
    {
        DisplayItem(player, inventory, key, items);
    }

    public static void DisplayItem(CCSPlayerController player, bool inventory, string key, Dictionary<string, Dictionary<string, string>> items)
    {
        CenterHtmlMenu menu = new(key, Instance);
        menu.ExitButton = true;

        foreach (KeyValuePair<string, Dictionary<string, string>> kvp in items)
        {
            Dictionary<string, string> item = kvp.Value;

            if (item["enable"] != "true")
                continue;


            if (Item.PlayerUsing(player, item["type"], item["uniqueid"]))
            {
                AddMenuOption(player, menu, (player, option) =>
                {
                    Item.Unequip(player, item);

                    player.PrintToChatMessage("chat<unequip>", item["name"]);

                    DisplayItem(player, inventory, key, items);
                }, item["name"]);
            }
            else
            {
                AddMenuOption(player, menu, (player, option) =>
                {
                    Item.Equip(player, item);
                    player.PrintToChatMessage("chat<equip>", item["name"]);
                    DisplayItem(player, inventory, key, items);
                }, item["name"]);
            }
        }

        AddMenuOption(player, menu, (player, option) =>
        {
            DisplayMenu(player, inventory);
        }, Instance.Localizer["menu<back>"]);

        MenuManager.OpenCenterHtmlMenu(Instance, player, menu);
    }
}