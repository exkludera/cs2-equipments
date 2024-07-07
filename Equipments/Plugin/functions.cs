using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using static Equipments.Equipments;
namespace Equipments;

public static class Functions
{
    static public void PrintToChatMessage(this CCSPlayerController player, string message, params object[] args)
    {
        player.PrintToChat($" {Instance.Localizer["prefix"]} {Instance.Localizer[message, args]}");
    }

    public static bool PlayerAlive(CCSPlayerController player)
    {
        if (player == null || !player.IsValid || player.Pawn == null || !player.PlayerPawn.IsValid || !player.PawnIsAlive)
            return false;

        return true;
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];
        for (int i = 0; i < length; i++)
            result[i] = chars[random.Next(chars.Length)];

        return new string(result);
    }

    public static (List<CCSPlayerController> players, string targetname) Find(CommandInfo command,int minArgCount,bool singletarget,bool ignoreMessage = false)
    {
        if (command.ArgCount < minArgCount)
            return (new List<CCSPlayerController>(), string.Empty);

        TargetResult targetresult = command.GetArgTargetResult(1);

        if (targetresult.Players.Count == 0)
        {
            if (!ignoreMessage)
                command.ReplyToCommand(Instance.Localizer["prefix"] + Instance.Localizer["No matching client"]);

            return (new List<CCSPlayerController>(), string.Empty);
        }
        else if (singletarget && targetresult.Players.Count > 1)
        {
            command.ReplyToCommand(Instance.Localizer["prefix"] + Instance.Localizer["More than one client matched"]);
            return (new List<CCSPlayerController>(), string.Empty);
        }

        string targetname;

        if (targetresult.Players.Count == 1)
            targetname = targetresult.Players.Single().PlayerName;
        else
        {
            Target.TargetTypeMap.TryGetValue(command.GetArg(1), out TargetType type);

            targetname = type switch
            {
                TargetType.GroupAll => Instance.Localizer["all"],
                TargetType.GroupBots => Instance.Localizer["bots"],
                TargetType.GroupHumans => Instance.Localizer["humans"],
                TargetType.GroupAlive => Instance.Localizer["alive"],
                TargetType.GroupDead => Instance.Localizer["dead"],
                TargetType.GroupNotMe => Instance.Localizer["notme"],
                TargetType.PlayerMe => targetresult.Players.First().PlayerName,
                TargetType.TeamCt => Instance.Localizer["ct"],
                TargetType.TeamT => Instance.Localizer["t"],
                TargetType.TeamSpec => Instance.Localizer["spec"],
                _ => targetresult.Players.First().PlayerName
            };
        }

        return (targetresult.Players, targetname);
    }
}