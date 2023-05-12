namespace Control.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using System;
    using Сontrol;
    using UnityEngine;
    using Exiled.CustomRoles.API.Features;
    using Exiled.API.Features.Pickups;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using MEC;
    using Control;
    using Control.Extensions;
    using Exiled.API.Features.Items;
    using Interactables.Interobjects.DoorUtils;
    using XPSystem.API.Serialization;
    using Control.Commands;
    using Exiled.API.Features.Roles;
    using System.Threading.Tasks;
    using Mono.Security.X509.Extensions;
    using Player = Exiled.Events.Handlers.Player;
    using PlayerStatsSystem;
    using Exiled.API.Features.Pickups.Projectiles;
    using InventorySystem.Items.ThrowableProjectiles;
    using Steamworks.ServerList;

    internal sealed class PlayerHandler
    {
        public void OnEnabled()
        {
            Player.ChangingRole += OnChangingRole;
            Player.DroppingAmmo += OnDroppingAmmo;
            Player.Dying += OnDying;
            Player.ReloadingWeapon += OnReloadWeapon;
            Player.Left += OnLeft;
            Player.Verified += OnVerified;
            Player.UsingRadioBattery += OnUsingRadioBatteryEventArgs;
            Player.InteractingDoor += OnInteractingDoor;
            Player.UnlockingGenerator += OnUnlockingGenerator;
            Player.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            Player.InteractingLocker += OnInteractingLocker;
            Player.TriggeringTesla += OnTriggeringTesla;
            Player.Handcuffing += OnHandcuffing;
            Player.ThrownProjectile += OnThrownProjectile;
        }
        public void OnDisabled()
        {
            Player.ChangingRole -= OnChangingRole;
            Player.DroppingAmmo -= OnDroppingAmmo;
            Player.Dying -= OnDying;
            Player.ReloadingWeapon -= OnReloadWeapon;
            Player.Left -= OnLeft;
            Player.Verified -= OnVerified;
            Player.UsingRadioBattery -= OnUsingRadioBatteryEventArgs;
            Player.Handcuffing -= OnHandcuffing;

            Player.InteractingDoor -= OnInteractingDoor;
            Player.UnlockingGenerator -= OnUnlockingGenerator;
            Player.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            Player.InteractingLocker -= OnInteractingLocker;
            Player.TriggeringTesla -= OnTriggeringTesla;
        }
        public void OnHandcuffing(HandcuffingEventArgs ev)
        {
            ev.Target.SetAmmo(AmmoType.Nato9, 0);
            ev.Target.SetAmmo(AmmoType.Ammo44Cal, 0);
            ev.Target.SetAmmo(AmmoType.Nato762, 0);
            ev.Target.SetAmmo(AmmoType.Ammo12Gauge, 0);
            ev.Target.SetAmmo(AmmoType.Nato556, 0);
        }
        private void OnVerified(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                if (ev.Player.GroupName == "d1" || ev.Player.GroupName == "d2")
                {
                    // Add player to DB;
                    ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Insert(new PlayerLog()
                    {
                        ID = ev.Player.UserId,
                        cooldownRole = false,
                        cooldownItem = false,
                        cooldownCall = false,
                        cooldownVote = false,
                        ForcedToSCP = false,
                        GivedTimes = 0,
                        ForcedTimes = 0,
                        CallTimes = 0,
                    });
                }
            });
        }
        private void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
        } 
        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.IsInIdleRange = false;
        }
        private void OnUsingRadioBatteryEventArgs(UsingRadioBatteryEventArgs ev) => ev.Drain = 0f;
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Door.RequiredPermissions.RequiredPermissions))
                ev.IsAllowed = true;
        }
        private void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Generator.Base._requiredPermission))
                ev.IsAllowed = true;
        }
        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Chamber != null && ev.Player.HasKeycardPermission(ev.Chamber.RequiredPermissions))
            {
                ev.IsAllowed = true;
            }
        }
        private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead))
                ev.IsAllowed = true;
        }
        private void OnChangingRole(ChangingRoleEventArgs ev)
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

            if (ev.NewRole == RoleTypeId.NtfPrivate && ev.Items.Contains(ItemType.KeycardNTFOfficer))
            {
                ev.Items.Remove(ItemType.KeycardNTFOfficer);
                ev.Items.Add(ItemType.KeycardNTFLieutenant);

            }
            if (ev.NewRole == RoleTypeId.FacilityGuard && ev.Items.Contains(ItemType.KeycardGuard))
            {
                ev.Items.Remove(ItemType.KeycardGuard);
                ev.Items.Add(ItemType.KeycardNTFOfficer);

            }
            if (ev.NewRole == RoleTypeId.ClassD)
            {
                ev.Items.Add(ItemType.KeycardJanitor);
            }
        }
        private void OnLeft(LeftEventArgs ev)
        {
            if (Res.DiedWithSCP500R.Contains(ev.Player))
            {
                Res.DiedWithSCP500R.Remove(ev.Player);
                Res.RoleDiedWithSCP500R.Clear();
            }

            if (Scp173Role.TurnedPlayers.Contains(ev.Player))
            {
                Scp173Role.TurnedPlayers.Remove(ev.Player);
            }

            if (Scp096Role.TurnedPlayers.Contains(ev.Player))
            {
                Scp096Role.TurnedPlayers.Remove(ev.Player);
            }

            ev.Player.IsOverwatchEnabled = false;
        }
        private void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Player == null) return;


            ev.Player.SetAmmo(AmmoType.Nato9, 0);
            ev.Player.SetAmmo(AmmoType.Ammo44Cal, 0);
            ev.Player.SetAmmo(AmmoType.Nato762, 0);
            ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 0);
            ev.Player.SetAmmo(AmmoType.Nato556, 0);
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
    }
}