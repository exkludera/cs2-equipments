using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using System.Runtime.InteropServices;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public bool EquipWeapon(CCSPlayerController player, string model)
    {
        var equippedItems = Instance.GetEquippedItems(player);

        foreach (var equippedItem in equippedItems)
        {
            if (equippedItem.Value.Weapon == model)
                return Weapon.HandleEquip(player, model, true);
        }

        return false;
    }

    public bool UnequipWeapon(CCSPlayerController player, string model, bool update)
    {
        if (!update)
            return true;

        var equippedItems = Instance.GetEquippedItems(player);

        foreach (var equippedItem in equippedItems)
        {
            if (equippedItem.Value.Weapon == model)
                return Weapon.HandleEquip(player, model, false);
        }

        return true;
    }

    public void OnEntityCreated(CEntityInstance entity)
    {
        if (!entity.DesignerName.StartsWith("weapon_"))
            return;

        CBasePlayerWeapon weapon = entity.As<CBasePlayerWeapon>();

        Server.NextWorldUpdate(() =>
        {
            if (!weapon.IsValid || weapon.OriginalOwnerXuidLow <= 0)
                return;

            CCSPlayerController? player = Utilities.GetPlayerFromSteamId(weapon.OriginalOwnerXuidLow);

            if (player == null)
                return;

            var equippedItems = GetEquippedItems(player);
            if (equippedItems == null)
                return;

            string weaponDesignerName = Weapon.GetDesignerName(weapon);

            CBasePlayerWeapon? activeWeapon = player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;

            foreach (var item in equippedItems)
            {
                if (CoreConfig.FollowCS2ServerGuidelines)
                {
                    throw new Exception($"Cannot set or get 'CEconEntity::m_OriginalOwnerXuidLow' with \"FollowCS2ServerGuidelines\" option enabled.");
                }

                var weaponpart = item.Value.Weapon.Split(':');
                if (weaponpart.Length != 2)
                    continue;

                var weaponName = weaponpart[0];
                var weaponModel = weaponpart[1];

                if (weaponDesignerName == weaponName)
                {
                    Weapon.UpdateModel(player, weapon, weaponModel, weapon == activeWeapon);
                    break;
                }
            }
        });
    }

    public static HookResult EventItemEquip(EventItemEquip @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;

        if (player == null)
            return HookResult.Continue;

        CBasePlayerWeapon? activeWeapon = player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;

        if (activeWeapon == null)
            return HookResult.Continue;

        string globalname = activeWeapon.Globalname;

        if (!string.IsNullOrEmpty(globalname))
            Weapon.SetViewModel(player, globalname.Split(',')[1]);

        return HookResult.Continue;
    }

    public static class Weapon
    {
        public static string GetDesignerName(CBasePlayerWeapon weapon)
        {
            string weaponDesignerName = weapon.DesignerName;
            ushort weaponIndex = weapon.AttributeManager.Item.ItemDefinitionIndex;

            weaponDesignerName = (weaponDesignerName, weaponIndex) switch
            {
                var (name, _) when name.Contains("bayonet") => "weapon_knife",
                ("weapon_m4a1", 60) => "weapon_m4a1_silencer",
                ("weapon_hkp2000", 61) => "weapon_usp_silencer",
                _ => weaponDesignerName
            };

            return weaponDesignerName;
        }

        public static unsafe string GetViewModel(CCSPlayerController player)
        {
            var viewModel = ViewModel(player)?.VMName ?? string.Empty;
            return viewModel;
        }

        public static unsafe void SetViewModel(CCSPlayerController player, string model)
        {
            ViewModel(player)?.SetModel(model);
        }

        public static void UpdateModel(CCSPlayerController player, CBasePlayerWeapon weapon, string model, bool update)
        {
            weapon.Globalname = $"{GetViewModel(player)},{model}";
            weapon.SetModel(model);

            if (update)
                SetViewModel(player, model);
        }

        public static void ResetWeapon(CCSPlayerController player, CBasePlayerWeapon weapon, bool update)
        {
            string globalname = weapon.Globalname;

            if (string.IsNullOrEmpty(globalname))
                return;

            string[] globalnamedata = globalname.Split(',');

            weapon.Globalname = string.Empty;
            weapon.SetModel(globalnamedata[0]);

            if (update)
                SetViewModel(player, globalnamedata[0]);
        }

        public static bool HandleEquip(CCSPlayerController player, string modelName, bool isEquip)
        {
            if (player.PawnIsAlive)
            {
                var weaponpart = modelName.Split(':');
                if (weaponpart.Length != 2)
                {
                    return false;
                }

                var weaponName = weaponpart[0];
                var weaponModel = weaponpart[1];

                CBasePlayerWeapon? weapon = Get(player, weaponName);

                if (weapon != null)
                {
                    bool equip = weapon == player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;

                    if (isEquip)
                    {
                        UpdateModel(player, weapon, weaponModel, equip);
                    }
                    else
                    {
                        ResetWeapon(player, weapon, equip);
                    }
                    return true;
                }
                else return false;
            }

            return true;
        }


        private static CBasePlayerWeapon? Get(CCSPlayerController player, string weaponName)
        {
            CPlayer_WeaponServices? weaponServices = player.PlayerPawn?.Value?.WeaponServices;

            if (weaponServices == null)
                return null;

            CBasePlayerWeapon? activeWeapon = weaponServices.ActiveWeapon?.Value;

            if (activeWeapon != null && GetDesignerName(activeWeapon) == weaponName)
                return activeWeapon;

            return weaponServices.MyWeapons.SingleOrDefault(p => p.Value != null && GetDesignerName(p.Value) == weaponName)?.Value;
        }

        private static unsafe CBaseViewModel? ViewModel(CCSPlayerController player)
        {
            nint? handle = player.PlayerPawn.Value?.ViewModelServices?.Handle;

            if (handle == null || !handle.HasValue)
                return null;

            CCSPlayer_ViewModelServices viewModelServices = new(handle.Value);

            nint ptr = viewModelServices.Handle + Schema.GetSchemaOffset("CCSPlayer_ViewModelServices", "m_hViewModel");
            Span<nint> viewModels = MemoryMarshal.CreateSpan(ref ptr, 3);

            CHandle<CBaseViewModel> viewModel = new(viewModels[0]);

            return viewModel.Value;
        }
    }
}