using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Globalization;

using static Equipments.Equipments;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public static class Item_Trail
{
    private static bool trailExists = false;
    public static void OnPluginStart()
    {
        Item.RegisterType("trail", OnServerPrecacheResources, OnEquip, OnUnequip, true, null);
        if (Item.GetItemsByType("trail").Count > 0) trailExists = true;
    }
    public static void OnServerPrecacheResources(ResourceManifest manifest)
    {
        List<KeyValuePair<string, Dictionary<string, string>>> items = Item.GetItemsByType("trail");

        foreach (KeyValuePair<string, Dictionary<string, string>> item in items)
        {
            manifest.AddResource(item.Value["uniqueid"]);
        }
    }
    public static bool OnEquip(CCSPlayerController player, Dictionary<string, string> item)
    {
        return true;
    }
    public static bool OnUnequip(CCSPlayerController player, Dictionary<string, string> item)
    {
        return true;
    }

    public static void OnTick(CCSPlayerController player)
    {
        if (!trailExists)
            return;

        Equipments_Items? playertrail = Instance.GlobalEquipmentsItems.FirstOrDefault(p => p.SteamID == player.SteamID && p.Type == "trail");
        if (playertrail == null) return;

        Dictionary<string, string>? itemdata = Item.GetItem(playertrail.Type, playertrail.UniqueId);
        if (itemdata == null) return;

        Vector? absorigin = player.PlayerPawn.Value?.AbsOrigin;
        if (absorigin == null) return;
 
        float lifetime = 1.3f;

        if (itemdata.TryGetValue("lifetime", out string? ltvalue) && float.TryParse(ltvalue, CultureInfo.InvariantCulture, out float lt))
            lifetime = lt;

        CreateParticle(absorigin, playertrail.UniqueId, lifetime, itemdata, player);
    }

    public static void CreateParticle(Vector absOrigin, string effectName, float lifetime, Dictionary<string, string> itemdata, CCSPlayerController player)
    {
        CParticleSystem? entity = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");

        if (entity == null || !entity.IsValid) return;

        if (!itemdata.TryGetValue("acceptInputValue", out string? acceptinputvalue) || string.IsNullOrEmpty(acceptinputvalue))
            acceptinputvalue = "Start";

        QAngle angle = new();

        if (!itemdata.TryGetValue("angleValue", out string? angleValue) || string.IsNullOrEmpty(angleValue))
            angle.X = 90;
        else
        {
            string[] angleValues = angleValue.Split(' ');

            angle.X = int.Parse(angleValues[0]);
            angle.Y = int.Parse(angleValues[0]);
            angle.Z = int.Parse(angleValues[0]);
        }

        entity.EffectName = effectName;
        entity.DispatchSpawn();
        entity.Teleport(absOrigin, angle, new Vector());
        entity.AcceptInput(acceptinputvalue!);
        entity.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");

        Instance.AddTimer(lifetime, () =>
        {
            if (entity != null && entity.IsValid)
                entity.Remove();
        });
    }
}