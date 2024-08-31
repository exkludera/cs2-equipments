using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public HookResult EventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null || !player.IsValid) return HookResult.Continue;

        if (playerCookies.TryGetValue(player, out var cookies))
        {
            foreach (var category in Config.Menu.Values)
            {
                string cookieName = $"Equipment-{category.Title}";
                if (cookies.TryGetValue(cookieName, out var Category))
                {
                    var model = category.Models.FirstOrDefault(m => m.Name.Equals(Category, StringComparison.OrdinalIgnoreCase))!;
                    Equip(player, model.File, category.Title);
                }
            }
        }

        return HookResult.Continue;
    }

    public void Equip(CCSPlayerController player, string model, string category)
    {
        if (!playerEquipment.ContainsKey(player))
            playerEquipment[player] = new();

        Unequip(player, category);

        AddTimer(0.1f, () => {

            if (player == null || !player.IsValid || !player.PawnIsAlive || player.IsBot)
                return;

            player.PlayerPawn.Value!.Effects = 1;
            Utilities.SetStateChanged(player.PlayerPawn.Value!, "CBaseEntity", "m_fEffects");

            CreateItem(player, model, category);
        });
    }

    public void Unequip(CCSPlayerController player, string category)
    {
        if (playerEquipment.TryGetValue(player, out var equipmentByCategory))
        {
            if (equipmentByCategory.TryGetValue(category, out var entity) && entity.IsValid)
                entity.Remove();
            
            equipmentByCategory.Remove(category);
        }
    }

    public void CreateItem(CCSPlayerController player, string model, string category)
    {
        var entity = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic_override");

        AddTimer(0.1f, () => {
            entity!.Globalname = $"{player.SteamID}({model})#{RandomString(6)}";
            entity.SetModel(model);
            entity.DispatchSpawn();
            entity.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");

            playerEquipment[player][category] = entity;
        });
    }
}