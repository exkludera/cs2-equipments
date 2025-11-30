using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

public static class Models
{
    public static readonly Dictionary<CCSPlayerController, Dictionary<MenuCategory, List<CBaseModelEntity>>> equippedModels = new();

    public static void Equip(CCSPlayerController player, MenuCategory category, string model)
    {
        if (!equippedModels.ContainsKey(player))
            equippedModels[player] = new();

        Unequip(player, category, model);

        Server.NextFrame(() => CreateModel(player, category, model));
    }

    public static void Unequip(CCSPlayerController player, MenuCategory category, string? modelFile)
    {
        if (!equippedModels.TryGetValue(player, out var equipmentByCategory))
            return;

        if (!equipmentByCategory.TryGetValue(category, out var models))
            return;

        if (string.IsNullOrEmpty(modelFile))
        {
            foreach (var ent in models)
                if (ent.IsValid)
                    ent.Remove();

            models.Clear();
            equipmentByCategory.Remove(category);
        }
        else
        {
            var modelToRemove = models.FirstOrDefault(m => m.IsValid && m.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName == modelFile);

            if (modelToRemove != null)
            {
                modelToRemove.Remove();
                models.Remove(modelToRemove);

                if (models.Count == 0)
                    equipmentByCategory.Remove(category);
            }
        }
    }

    private static void CreateModel(CCSPlayerController player, MenuCategory category, string model)
    {
        if (!equippedModels[player].ContainsKey(category))
            equippedModels[player][category] = new();

        var entity = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic_override")!;

        entity.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags &= ~(uint)(1 <<2);
        entity.SetModel(model);
        entity.Teleport(player.AbsOrigin);
        entity.DispatchSpawn();
        entity.AcceptInput("FollowEntity", player.PlayerPawn.Value!, entity, "!activator");

        equippedModels[player][category].Add(entity);
    }
}