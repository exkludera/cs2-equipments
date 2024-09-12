using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public int Tick;
    public void OnTick()
    {
        Tick++;

        if (Tick < 16)
            return;

        Tick = 0;

        foreach (CCSPlayerController player in Utilities.GetPlayers().Where(p => !p.IsBot))
        {
            if (!player.PawnIsAlive || !playerCookies.ContainsKey(player.Slot))
                continue;

            foreach (var item in GetEquippedItems(player))
            {
                var equipment = item.Value;
                if (!string.IsNullOrEmpty(equipment.Particle))
                    CreateParticle(player, player.PlayerPawn.Value!.AbsOrigin!, equipment.Particle);
            }
        }
    }

    public void CreateParticle(CCSPlayerController player, Vector absOrigin, string particleFile)
    {
        var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = particleFile;
        particle.DispatchSpawn();
        particle.AcceptInput("Start");
        particle.AcceptInput("FollowEntity", player.PlayerPawn.Value!, player.PlayerPawn.Value!, "!activator");

        particle.Teleport(absOrigin);

        AddTimer(1.0f, () =>
        {
            if (particle == null || !particle.IsValid)
                return;

            particle.Remove();
        });
    }
}