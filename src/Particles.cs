using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public static class Particles
{
    public static readonly Dictionary<CCSPlayerController, Dictionary<MenuCategory, List<CEnvParticleGlow>>> equippedParticles = new();

    public static void Equip(CCSPlayerController player, MenuCategory category, Vector absOrigin, string particleFile)
    {
        if (!equippedParticles.ContainsKey(player))
            equippedParticles[player] = new();

        Unequip(player, category, particleFile);

        Server.NextFrame(() => CreateParticle(player, category, absOrigin, particleFile));
    }

    public static void Unequip(CCSPlayerController player, MenuCategory category, string? particleFile)
    {
        if (!equippedParticles.TryGetValue(player, out var byCategory))
            return;

        if (!byCategory.TryGetValue(category, out var particles))
            return;

        if (string.IsNullOrEmpty(particleFile))
        {
            foreach (var ent in particles)
                if (ent.IsValid)
                    ent.Remove();

            particles.Clear();
            byCategory.Remove(category);
        }
        else
        {
            var toRemove = particles.FirstOrDefault(p => p.IsValid && p.EffectName == particleFile);
            if (toRemove != null)
            {
                toRemove.Remove();
                particles.Remove(toRemove);

                if (particles.Count == 0)
                    byCategory.Remove(category);
            }
        }
    }

    private static void CreateParticle(CCSPlayerController player, MenuCategory category, Vector absOrigin, string particleFile)
    {
        if (!equippedParticles[player].ContainsKey(category))
            equippedParticles[player][category] = new();

        var particle = Utilities.CreateEntityByName<CEnvParticleGlow>("env_particle_glow")!;

        particle.StartActive = true;
        particle.EffectName = particleFile;
        particle.Teleport(absOrigin);
        particle.DispatchSpawn();
        particle.AcceptInput("FollowEntity", player.PlayerPawn.Value, particle, "!activator");

        equippedParticles[player][category].Add(particle);
    }
}