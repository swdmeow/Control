namespace Control.Handlers.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Ammo;
    using MEC;
    using PlayerEvent = Exiled.Events.Handlers.Player;

    internal sealed class UnlimitedAmmoHandler
    {
        public UnlimitedAmmoHandler()
        {
            PlayerEvent.Handcuffing += OnHandcuffing;
            PlayerEvent.RemovingHandcuffs += OnRemovingHandcuffs;
            PlayerEvent.DroppingAmmo += OnDroppingAmmo;
            PlayerEvent.Dying += OnDying;
            PlayerEvent.ReloadingWeapon += OnReloadWeapon;
            PlayerEvent.ChangingRole += OnChangingRole;
        }
        public void OnDisabled()
        {
            PlayerEvent.Handcuffing -= OnHandcuffing;
            PlayerEvent.RemovingHandcuffs -= OnRemovingHandcuffs;
            PlayerEvent.DroppingAmmo -= OnDroppingAmmo;
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.ReloadingWeapon -= OnReloadWeapon;
            PlayerEvent.ChangingRole -= OnChangingRole;
        }
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player)) return;
            if (ev.Player.IsScp) return;

            if (ev.Reason != SpawnReason.RoundStart && ev.Reason != SpawnReason.LateJoin && ev.Reason != SpawnReason.Respawn && ev.Reason == SpawnReason.ForceClass)
            {
                ev.Player.SetAmmo(AmmoType.Nato9, 0);
                ev.Player.SetAmmo(AmmoType.Ammo44Cal, 0);
                ev.Player.SetAmmo(AmmoType.Nato762, 0);
                ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 0);
                ev.Player.SetAmmo(AmmoType.Nato556, 0);
            }

            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.SetAmmo(AmmoType.Nato9, 101);
                ev.Player.SetAmmo(AmmoType.Ammo44Cal, 101);
                ev.Player.SetAmmo(AmmoType.Nato762, 101);
                ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 14);
                ev.Player.SetAmmo(AmmoType.Nato556, 101);
            });
        }
        private void OnReloadWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player)) return;

            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.SetAmmo(AmmoType.Nato9, 101);
                ev.Player.SetAmmo(AmmoType.Ammo44Cal, 101);
                ev.Player.SetAmmo(AmmoType.Nato762, 101);
                ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 14);
                ev.Player.SetAmmo(AmmoType.Nato556, 101);
            });
        }
        private void OnDroppingAmmo(DroppingAmmoEventArgs ev) => ev.IsAllowed = false;
        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Player == null) return;


            ev.Player.SetAmmo(AmmoType.Nato9, 0);
            ev.Player.SetAmmo(AmmoType.Ammo44Cal, 0);
            ev.Player.SetAmmo(AmmoType.Nato762, 0);
            ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 0);
            ev.Player.SetAmmo(AmmoType.Nato556, 0);
        }
        public void OnHandcuffing(HandcuffingEventArgs ev)
        {
            ev.Target.SetAmmo(AmmoType.Nato9, 0);
            ev.Target.SetAmmo(AmmoType.Ammo44Cal, 0);
            ev.Target.SetAmmo(AmmoType.Nato762, 0);
            ev.Target.SetAmmo(AmmoType.Ammo12Gauge, 0);
            ev.Target.SetAmmo(AmmoType.Nato556, 0);
        }
        private void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.SetAmmo(AmmoType.Nato9, 101);
                ev.Player.SetAmmo(AmmoType.Ammo44Cal, 101);
                ev.Player.SetAmmo(AmmoType.Nato762, 101);
                ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 14);
                ev.Player.SetAmmo(AmmoType.Nato556, 101);
            });
        }
    }
}