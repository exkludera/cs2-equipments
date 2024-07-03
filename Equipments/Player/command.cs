using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;

using static Equipments.Equipments;

namespace Equipments;

public static class Command
{
    public static void Load()
    {
        EquipmentsConfig config = Instance.Config;

        Dictionary<IEnumerable<string>, (string description, CommandInfo.CommandCallback handler)> commands = new()
        {
            {config.Commands.OpenMenu, ("Equipments menu", Command_Menu)},
            {config.Commands.ResetPlayer, ("Reset player's equipments", Command_ResetPlayer)},
            {config.Commands.ResetDatabase, ("Reset database", Command_ResetDatabase)}
        };

        foreach (KeyValuePair<IEnumerable<string>, (string description, CommandInfo.CommandCallback handler)> commandPair in commands)
        {
            foreach (string command in commandPair.Key)
            {
                Instance.AddCommand($"css_{command}", commandPair.Value.description, commandPair.Value.handler);
            }
        }
    }

    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Command_Menu(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
            return;

        if (Instance.Config.Settings.Flag != "" && !AdminManager.PlayerHasPermissions(player, Instance.Config.Settings.Flag))
        {
            player.PrintToChatMessage("chat<nopermission>");
            return;
        }

        if (Instance.Config.Settings.HudMenu)
            MenuHud.DisplayMenu(player, false);
        else
            MenuChat.DisplayMenu(player, false);
    }

    [CommandHelper(minArgs: 1, whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    [RequiresPermissions("@css/root")]
    public static void Command_ResetPlayer(CCSPlayerController? player, CommandInfo command)
    {
        (List<CCSPlayerController> players, string targetname) = Functions.Find(command, 2, false);

        if (players == null)
        {
            if (!SteamID.TryParse(command.GetArg(1), out SteamID? steamId) || steamId == null)
            {
                command.ReplyToCommand(Instance.Localizer["prefix"] + Instance.Localizer["Must be a steamid"]);
                return;
            }

            var playerdata = Instance.GlobalEquipmentsItems.SingleOrDefault(player => player.SteamID == steamId.SteamId64);

            if (playerdata == null)
            {
                command.ReplyToCommand(Instance.Localizer["prefix"] + Instance.Localizer["No matching client"]);
                return;
            }

            Instance.GlobalEquipmentsItems.RemoveAll(p => p.SteamID == steamId.SteamId64);

            Database.ExecuteAsync($"DELETE FROM {Instance.Config.Database.DBTable} WHERE SteamId = @SteamId;",
                new { @SteamID = steamId.SteamId64 });


            Server.PrintToChatAll(Instance.Localizer["prefix"] + Instance.Localizer["css_reset", player?.PlayerName ?? "Console", steamId.SteamId64]);
            return;
        }

        if (players.Count > 1)
        {
            command.ReplyToCommand(Instance.Localizer["prefix"] + Instance.Localizer["More than one client matched"]);
            return;
        }

        CCSPlayerController target = players.Single();

        Instance.GlobalEquipmentsItems.RemoveAll(p => p.SteamID == target.SteamID);

        Database.ResetPlayer(target);

        Server.PrintToChatAll(Instance.Localizer["prefix"] + Instance.Localizer["css_reset", player?.PlayerName ?? "Console", target.PlayerName]);
    }

    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.SERVER_ONLY)]
    public static void Command_ResetDatabase(CCSPlayerController? player, CommandInfo command)
    {
        if (player != null)
            return;

        Instance.GlobalEquipmentsItems.Clear();

        Database.ResetDatabase();

        foreach (CCSPlayerController target in Utilities.GetPlayers())
        {
            Database.LoadPlayer(target);
        }
    }

}