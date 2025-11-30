using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public static class Events
{
    private static readonly Plugin Instance = Plugin.Instance;

    public static void Register()
    {
        Instance.RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
        Instance.RegisterListener<Listeners.OnEntityCreated>(OnEntityCreated);
        Instance.RegisterListener<Listeners.OnMapStart>(OnMapStart);

        Instance.RegisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        Instance.RegisterEventHandler<EventPlayerDisconnect>(EventPlayerDisconnect);
    }

    public static void Unregister()
    {
        Instance.RemoveListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
        Instance.RemoveListener<Listeners.OnEntityCreated>(OnEntityCreated);
        Instance.RemoveListener<Listeners.OnMapStart>(OnMapStart);

        Instance.DeregisterEventHandler<EventPlayerSpawn>(EventPlayerSpawn);
        Instance.DeregisterEventHandler<EventPlayerDisconnect>(EventPlayerDisconnect);
    }

    private static void OnServerPrecacheResources(ResourceManifest manifest)
    {
        List<string> resources = new();

        void CollectAllResources(MenuCategory category)
        {
            foreach (var item in category.Equipment)
            {
                if (!string.IsNullOrEmpty(item.Model)) resources.Add(item.Model);
                if (!string.IsNullOrEmpty(item.Particle)) resources.Add(item.Particle);
            }

            if (category.SubCategories != null)
            {
                foreach (var subCat in category.SubCategories.Values)
                    CollectAllResources(subCat);
            }
        }

        foreach (var category in Instance.Config.Categories.Values)
            CollectAllResources(category);

        foreach (var resource in resources)
            if (!string.IsNullOrEmpty(resource))
                manifest.AddResource(resource);
    }

    private static void OnEntityCreated(CEntityInstance entity)
    {
        if (!entity.DesignerName.StartsWith("weapon_"))
            return;

        Server.NextWorldUpdate(() => Weapons.ProcessEntity(entity.As<CBasePlayerWeapon>()));
    }

    private static void OnMapStart(string mapname)
    {
        Models.equippedModels.Clear();
        Particles.equippedParticles.Clear();
        Weapons.oldDesignerName.Clear();
    }

    private static HookResult EventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || player.IsBot)
            return HookResult.Continue;

        foreach (var selection in Utils.GetEquippedSelections(player))
        {
            var equipment = selection.equipment;
            var category = selection.category;

            if (!string.IsNullOrEmpty(equipment.Model))
                Models.Equip(player, category, equipment.Model);

            if (!string.IsNullOrEmpty(equipment.Particle))
                Particles.Equip(player, category, player.PlayerPawn.Value?.AbsOrigin!, equipment.Particle);
        }

        return HookResult.Continue;
    }

    private static HookResult EventPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || player.IsBot)
            return HookResult.Continue;

        foreach (var selection in Utils.GetEquippedSelections(player))
        {
            var equipment = selection.equipment;
            var category = selection.category;

            Utils.RemoveEquipment(player, category, equipment);
        }

        return HookResult.Continue;
    }
}