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

    internal sealed class PlayerHandler
    {
        public static bool elevIsLock = false;
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadWeapon;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Verified += OnVerified;


            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnUnlockingGenerator;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            //Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;

        }
        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadWeapon;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;


            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnUnlockingGenerator;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractingLocker;
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player.GroupName == "d1")
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
        }

        /*private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if(ServerHandler.InteractingItemsElevator.Contains(ev.Pickup))
            {
                ev.IsAllowed = false;

                if (elevIsLock) return;

                runElevator();
            }
        }*/
        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.IsInIdleRange = false;
        }
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
                    ev.IsAllowed = true;
        }
        private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead))
                ev.IsAllowed = true;
        }
        private void OnSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player == null) return;

            if (CustomRole.Get(2).Check(ev.Player)) return;

            ev.Player.SetAmmo(AmmoType.Nato9, 100);
            ev.Player.SetAmmo(AmmoType.Ammo44Cal, 100);
            ev.Player.SetAmmo(AmmoType.Nato762, 100);
            ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 14);
            ev.Player.SetAmmo(AmmoType.Ammo44Cal, 100);
        }
        private void OnLeft(LeftEventArgs ev)
        {
            if(Res.DiedWithSCP500R.Contains(ev.Player))
            {
                Res.DiedWithSCP500R.Remove(ev.Player);
            }

            if(Scp173Role.TurnedPlayers.Contains(ev.Player))
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
            if (CustomRole.Get(2).Check(ev.Player)) return;

            ev.Player.SetAmmo(AmmoType.Nato9, 200);
            ev.Player.SetAmmo(AmmoType.Ammo44Cal, 200);
            ev.Player.SetAmmo(AmmoType.Nato762, 200);
            ev.Player.SetAmmo(AmmoType.Ammo12Gauge, 14);
            ev.Player.SetAmmo(AmmoType.Nato556, 200);
        }
        /*
        private async void runElevator()
        {
            elevIsLock = true;
            ServerHandler.doorAirlock.AnimationController.Play("Close", "FIFIF");

            await Task.Delay(1500);

            var Players = Player.List.Where(x => Vector3.Distance(ServerHandler.positionToTeleport, x.Position) <= 3f);

            foreach (Player pl in Players)
            {
                if (!ServerHandler.RoomRotated) pl.Position = ServerHandler.pos + new Vector3(-12f, 101f, 0.2f);
                if (ServerHandler.RoomRotated) pl.Position = ServerHandler.pos + new Vector3(0f, 101f, 0f);
            };

            await Task.Delay(500);

            ServerHandler.door035.AnimationController.Play("Open", "FIFIF");

            elevIsLock = false;
        }*/
    }
}