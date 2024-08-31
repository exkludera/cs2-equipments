using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public bool HasPermission(CCSPlayerController player, string Permission, string Team)
    {
        string permission = Permission.ToLower();
        string team = Team.ToLower();

        bool isTeamValid = (team == "t" || team == "terrorist") && player.Team == CsTeam.Terrorist ||
                    (team == "ct" || team == "counterterrorist") && player.Team == CsTeam.CounterTerrorist ||
                    (team == "" || team == "both" || team == "all");

        return (string.IsNullOrEmpty(permission) || AdminManager.PlayerHasPermissions(player, permission)) && isTeamValid;
    }

    public string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];
        for (int i = 0; i < length; i++)
            result[i] = chars[random.Next(chars.Length)];

        return new string(result);
    }
}