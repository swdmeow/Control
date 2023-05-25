namespace Control.Handlers.Events
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
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using PlayerStatsSystem;
    using Exiled.API.Features.Pickups.Projectiles;
    using InventorySystem.Items.ThrowableProjectiles;
    using Steamworks.ServerList;
    using Exiled.Loader;
    using PluginAPI.Commands;
    using CommandSystem.Commands.RemoteAdmin;
    using GameCore;

    internal sealed class PlayerHandler
    {
        public PlayerHandler()
        {
            PlayerEvent.ChangingRole += OnChangingRole;
            PlayerEvent.DroppingAmmo += OnDroppingAmmo;
            PlayerEvent.Dying += OnDying;
            PlayerEvent.ReloadingWeapon += OnReloadWeapon;
            PlayerEvent.Left += OnLeft;
            PlayerEvent.Verified += OnVerified;
            PlayerEvent.UsingRadioBattery += OnUsingRadioBatteryEventArgs;
            PlayerEvent.InteractingDoor += OnInteractingDoor;
            PlayerEvent.UnlockingGenerator += OnUnlockingGenerator;
            PlayerEvent.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            PlayerEvent.InteractingLocker += OnInteractingLocker;
            PlayerEvent.TriggeringTesla += OnTriggeringTesla;
            PlayerEvent.Handcuffing += OnHandcuffing;
            PlayerEvent.Escaping += OnEscaping;
        }
        public void OnDisabled()
        {
            PlayerEvent.ChangingRole -= OnChangingRole;
            PlayerEvent.DroppingAmmo -= OnDroppingAmmo;
            PlayerEvent.Dying -= OnDying;
            PlayerEvent.ReloadingWeapon -= OnReloadWeapon;
            PlayerEvent.Left -= OnLeft;
            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.UsingRadioBattery -= OnUsingRadioBatteryEventArgs;
            PlayerEvent.Handcuffing -= OnHandcuffing;

            PlayerEvent.InteractingDoor -= OnInteractingDoor;
            PlayerEvent.UnlockingGenerator -= OnUnlockingGenerator;
            PlayerEvent.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            PlayerEvent.InteractingLocker -= OnInteractingLocker;
            PlayerEvent.TriggeringTesla -= OnTriggeringTesla;
            PlayerEvent.Escaping += OnEscaping;
        }
        private void OnEscaping(EscapingEventArgs ev)
        {
            if (ev.IsAllowed) return;

            if (!ev.Player.IsCuffed) return;

            if (ev.Player.Role.Team == Team.ChaosInsurgency) { ev.IsAllowed = true; ev.EscapeScenario = EscapeScenario.CuffedClassD; } // МОГ
            if (ev.Player.Role.Team == Team.FoundationForces) { ev.IsAllowed = true; ev.EscapeScenario = EscapeScenario.CuffedScientist; } // ПХ
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

            // Disabled due exiled bug


            /*
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead))
                ev.IsAllowed = true;*/
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
            if(ev.Player.GroupName == "d2")
            {
                Timing.CallDelayed(0.1f, () => CustomItem.Get((uint)7).Give(ev.Player));
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