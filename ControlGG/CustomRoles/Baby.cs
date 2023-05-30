using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using Exiled.Events.EventArgs.Scp096;
using Exiled.API.Features;
using MEC;
using System.Threading.Tasks;
using System.Threading;
using Exiled.API.Enums;
using PlayerRoles.FirstPersonControl;
using static Respawning.RespawnEffectsController;
using Exiled.Events.EventArgs.Scp173;
using Utils.NonAllocLINQ;
using System;
using System.Reflection;
using Exiled.API.Extensions;
using Exiled.API.Features.Pools;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Loader;
using Mirror;
using CustomPlayerEffects;
using System.Text;

namespace Control.CustomRoles
{
    [CustomRole(RoleTypeId.None)]
    public class Baby : CustomRole
    {
        public override uint Id { get; set; } = 8;
        public override string Name { get; set; } = "ребёнок.";
        public override string Description { get; set; } = "...";
        public override RoleTypeId Role { get; set; } = RoleTypeId.None;
        public override string CustomInfo { get; set; }
        public override int MaxHealth { get; set; } = 100;
        public override bool KeepInventoryOnSpawn { get; set; } = true;
        public override SpawnProperties SpawnProperties { get; set; } = null;
        protected override void SubscribeEvents()
        {
            Log.Debug(Name + ": Loading events.");
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;

            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.Dying += OnDying;

        }
        protected override void RoleAdded(Player player)
        {
            player.Scale = new Vector3(0.6f, 0.6f, 0.6f);

            foreach (Item item in player.Items)
            {
                if (item is Firearm)
                {
                    if (item.Type == ItemType.GunCOM15 || item.Type == ItemType.GunCOM18)
                    {
                        continue;
                    }
                    player.RemoveItem(item);
                    player.AddItem(ItemType.GunCOM18);

                    return;
                }
            }

            base.RoleAdded(player);
        }
        private void OnDying(DyingEventArgs ev)
        {
            if (CustomRole.Get((uint)8).Check(ev.Player))
            {
                ev.Player.Scale = new Vector3(1f, 1f, 1f);
            }
        }
        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (CustomRole.Get((uint)8).Check(ev.Player))
            {
                if (ev.Pickup is FirearmPickup)
                {
                    if (ev.Pickup.Type == ItemType.GunCOM15 || ev.Pickup.Type == ItemType.GunCOM18)
                    {
                        return;
                    }
                    ev.IsAllowed = false;
                }
            }
        }
        private void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (CustomRole.Get((uint)8).Check(ev.Player))
            {
                if (ev.Firearm.Type == ItemType.GunCOM15 || ev.Firearm.Type == ItemType.GunCOM18) return;

                ev.IsAllowed = false;
            }
        }
        private void OnShooting(ShootingEventArgs ev)
        {
            if (CustomRole.Get((uint)8).Check(ev.Player))
            {
                if (ev.Player.CurrentItem.Type == ItemType.GunCOM15 || ev.Player.CurrentItem.Type == ItemType.GunCOM18) return;

                ev.IsAllowed = false;
                ev.Player.ShowHint("Вы не можете использовать тяжёлое оружие играя за ребёнка..");
                ev.Player.DropItem(ev.Player.CurrentItem);
            }
        }
        protected override void UnsubscribeEvents()
        {
            foreach (Exiled.API.Features.Player trackedPlayer in TrackedPlayers)
            {
                RemoveRole(trackedPlayer);
            }

            Log.Debug(Name + ": Unloading events.");
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.Dying -= OnDying;

        }
        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if (Check(ev.Player) && ((ev.NewRole == RoleTypeId.Spectator && !KeepRoleOnDeath) || (ev.NewRole != RoleTypeId.Spectator && ev.NewRole != Role && !KeepRoleOnChangingRole)))
            {
                RemoveRole(ev.Player);
            }
        }
        public override void AddRole(Exiled.API.Features.Player player)
        {
            Log.Debug(Name + ": Adding role to " + player.Nickname + ".");
            TrackedPlayers.Add(player);
            if (Role != RoleTypeId.None)
            {
                if (KeepPositionOnSpawn && KeepInventoryOnSpawn)
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                }
                else if (KeepPositionOnSpawn)
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);
                }
                else if (KeepInventoryOnSpawn)
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                }
                else
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.All);
                }
            }

            if (!KeepInventoryOnSpawn)
            {
                Log.Debug(Name + ": Clearing " + player.Nickname + "'s inventory.");
                player.ClearInventory();
            }

            foreach (string item in Inventory)
            {
                Log.Debug(Name + ": Adding " + item + " to inventory.");
                TryAddItem(player, item);
            }

            Log.Debug(Name + ": Setting health values.");
            player.Health = MaxHealth;
            player.MaxHealth = MaxHealth;
            player.Scale = Scale;
            Vector3 spawnPosition = GetSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                player.Position = spawnPosition;
            }
            player.InfoArea &= ~(PlayerInfoArea.Nickname | PlayerInfoArea.Role);
            if (CustomAbilities != null)
            {
                foreach (CustomAbility item2 in CustomAbilities!)
                {
                    item2.AddAbility(player);
                }
            }

            ShowMessage(player);
            ShowBroadcast(player);
            RoleAdded(player);
            player.UniqueRole = Name;
            player.TryAddCustomRoleFriendlyFire(Name, CustomRoleFFMultiplier);
            // Delete stringBuilder to not cause console message
        }
    }
}