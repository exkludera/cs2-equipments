using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public Dictionary<CCSPlayerController, Dictionary<string, List<CBaseModelEntity>>> playerModels = new();

    public HookResult EventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null || !player.IsValid) return HookResult.Continue;

        foreach (var item in GetEquippedItems(player))
        {
            var equipment = item.Value;
            if (!string.IsNullOrEmpty(equipment.Model))
                EquipModel(player, equipment.Model, item.Key.Split('-')[1]);
        }

        return HookResult.Continue;
    }

    public void EquipModel(CCSPlayerController player, string model, string category)
    {
        if (!playerModels.ContainsKey(player))
            playerModels[player] = new Dictionary<string, List<CBaseModelEntity>>();

        UnequipModel(player, category, model);

        AddTimer(0.1f, () => {

            if (player == null || !player.IsValid || !player.PawnIsAlive || player.IsBot)
                return;

            player.PlayerPawn.Value!.Effects = 1;
            Utilities.SetStateChanged(player.PlayerPawn.Value!, "CBaseEntity", "m_fEffects");

            CreateModel(player, model, category);
        });
    }

    public void UnequipModel(CCSPlayerController player, string category, string modelFile)
    {
        if (playerModels.TryGetValue(player, out var equipmentByCategory))
        {
            if (equipmentByCategory.TryGetValue(category, out var models))
            {
                if (modelFile == null)
                {
                    foreach (var model in models)
                        if (model.IsValid)
                            model.Remove();

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
        }
    }

    public void CreateModel(CCSPlayerController player, string modelfile, string category)
    {
        if (!playerModels[player].ContainsKey(category))
            playerModels[player][category] = new List<CBaseModelEntity>();

        var model = Utilities.CreateEntityByName<CDynamicProp>("prop_dynamic_override")!;

        AddTimer(0.1f, () => {
            model.SetModel(modelfile);
            model.DispatchSpawn();
            model.AcceptInput("FollowEntity", player.PlayerPawn.Value!, player.PlayerPawn.Value!, "!activator");

            playerModels[player][category].Add(model);
        });
    }
}