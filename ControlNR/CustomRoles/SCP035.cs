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
using System.Diagnostics.SymbolStore;
using Control.Extensions;

namespace Control.CustomRoles
{
    [CustomRole(RoleTypeId.None)]
    public class SCP035 : CustomRole
    {
        public override uint Id { get; set; } = 1;
        public override string Name { get; set; } = "SCP-035";
        public override string Description { get; set; } = "Вы - SCP-035. Вы сотрудничаете с SCP-объектами и убиваете людей..";
        public override RoleTypeId Role { get; set; } = RoleTypeId.None;
        public override string CustomInfo { get; set; } = "SCP-035 \"Маска\"";
        public override int MaxHealth { get; set; } = 100;
        public override bool KeepInventoryOnSpawn { get; set; } = true;
        public override bool KeepRoleOnChangingRole { get; set; } = true;
        public override SpawnProperties SpawnProperties { get; set; } = null;

        private void OnHurting(HurtingEventArgs ev)
        {

            if (ev.Attacker == null) return;
            if (ev.Player == null) return;
            if (ev.Attacker == ev.Player) return;

            if (CustomRole.Get((uint)2).Check(ev.Player) || CustomRole.Get((uint)2).Check(ev.Attacker)) return;


            if (CustomRole.Get((uint)1).Check(ev.Attacker))
            {
                if (ev.Player.Role.Team == Team.SCPs || CustomRole.Get((uint)1).Check(ev.Player))
                {
                    ev.Amount = 0f;
                    ev.IsAllowed = false;

                    PlayerExtensions.ShowCustomHint(ev.Attacker, "Вы не можете наносить урон SCP объектам..", 2);

                    return;
                }

                ev.DamageHandler.IsFriendlyFire = false;
                ev.DamageHandler.ForceFullFriendlyFire = true;
                ev.IsAllowed = true;
            }

            if (CustomRole.Get((uint)1).Check(ev.Player))
            {
                if (ev.Attacker.Role.Team == Team.SCPs || CustomRole.Get((uint)1).Check(ev.Attacker))
                {
                    ev.Amount = 0f;
                    ev.IsAllowed = false;

                    PlayerExtensions.ShowCustomHint(ev.Attacker, "Вы не можете наносить урон SCP-035..", 2);

                    return;
                }

                ev.DamageHandler.IsFriendlyFire = false;
                ev.DamageHandler.ForceFullFriendlyFire = true;

                ev.IsAllowed = true;
            }
        }

        protected override void RoleAdded(Exiled.API.Features.Player player)
        {
            Exiled.API.Features.Roles.Scp173Role.TurnedPlayers.Add(player);
            Exiled.API.Features.Roles.Scp096Role.TurnedPlayers.Add(player);
            Exiled.API.Features.Roles.Scp049Role.TurnedPlayers.Add(player);

            Cassie.Message("Внимание! Обнаружено возможное нарушение условий содержаний SCP-035.<b></b> <color=#ffffff00>h ATTENTION ALL PERSONNEL . Detected possible CONTAINMENT breach of scp 0 3 5 ", false, false, true);
        }
        protected override void RoleRemoved(Exiled.API.Features.Player player)
        {
            Exiled.API.Features.Roles.Scp173Role.TurnedPlayers.Remove(player);
            Exiled.API.Features.Roles.Scp096Role.TurnedPlayers.Remove(player);
            Exiled.API.Features.Roles.Scp049Role.TurnedPlayers.Remove(player);


            CustomItem.Get((uint)3).Spawn(player.Position);
            player.DisplayNickname = null;
            Cassie.Message("SCP-035<b></b> был устранён.. <color=#ffffff00>h scp 0 3 5 has been terminated", false, false, true);
        }
        private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (!CustomRole.Get((uint)1).Check(ev.Player)) return;

            ev.IsAllowed = false;
        }
        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Attacker != null && CustomRole.Get((uint)1).Check(ev.Attacker))
            {
                MirrorExtensions.ChangeAppearance(ev.Attacker, ev.Player.Role, false, 0);

                ev.Attacker.DisplayNickname = ev.Player.Nickname;

                ev.Attacker.DisableAllEffects();
                if (ev.Player.ActiveEffects.Count() != 0)
                {
                    foreach (StatusEffectBase effect in ev.Player.ActiveEffects)
                    {
                        ev.Attacker.EnableEffect(effect, effect.TimeLeft);
                    }
                }

                ev.Attacker.MaxHealth += 5;

                bool AllowedToTp = true;
                Vector3 pos = ev.Player.Position;

                if (ev.DamageHandler.Type == DamageType.Explosion || ev.Player.Lift != null) AllowedToTp = false;

                Timing.CallDelayed(0.1f, () =>
                {
                    if (AllowedToTp == true) ev.Attacker.Teleport(pos);
                });
            }
        }
        protected override void SubscribeEvents()
        {
            Log.Debug(Name + ": Loading events.");
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnteringPocketDimension;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;

        }
        private void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (!CustomRole.Get((uint)1).Check(ev.Player)) return;

            if (ev.Effect.name == "Cardiac Arrest")
            {
                ev.IsAllowed = false;
            }
        }
        private void OnEscaping(EscapingEventArgs ev)
        {
            if (!CustomRole.Get((uint)1).Check(ev.Player)) return;

            ev.IsAllowed = false;
        }
        protected override void UnsubscribeEvents()
        {
            foreach (Exiled.API.Features.Player trackedPlayer in TrackedPlayers)
            {
                RemoveRole(trackedPlayer);
            }

            Log.Debug(Name + ": Unloading events.");
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnteringPocketDimension;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
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

            Log.Debug(Name + ": Setting player info");
            player.CustomInfo = CustomInfo;
            //player.InfoArea &= ~(PlayerInfoArea.Nickname | PlayerInfoArea.Role);
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
            if (string.IsNullOrEmpty(ConsoleMessage))
            {
                return;
            }
            // Delete stringBuilder to not cause console message
        }
    }
}