namespace Control.Handlers.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using System;
    using Сontrol;
    using PlayerRoles;
    using MEC;
    using Control;
    using Control.Extensions;
    using Control.Commands;
    using Exiled.API.Features.Roles;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using Exiled.Loader;
    using Control.API.Serialization;
    using RemoteAdmin.Communication;
    using PlayerRoles.PlayableScps.HumeShield;
    using Utils.Networking;
    using Exiled.API.Extensions;
    using InventorySystem.Items.Firearms;
    using RelativePositioning;
    using UnityEngine;
    using Control.CustomItems;
    using System.Linq;
    using static Mono.Security.X509.X520;
    using MapEditorReborn.Commands.UtilityCommands;
    using Exiled.CustomRoles.API.Features;

    internal sealed class PlayerHandler
    {
        public PlayerHandler()
        {
            PlayerEvent.ChangingRole += OnChangingRole;
            PlayerEvent.Left += OnLeft;
            PlayerEvent.Verified += OnVerified;
            PlayerEvent.UsingRadioBattery += OnUsingRadioBatteryEventArgs;
            PlayerEvent.TriggeringTesla += OnTriggeringTesla;
            PlayerEvent.Escaping += OnEscaping;
            PlayerEvent.Dying += OnDying;
        }
        public void OnDisabled()
        {
            PlayerEvent.ChangingRole -= OnChangingRole;
            PlayerEvent.Left -= OnLeft;
            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.UsingRadioBattery -= OnUsingRadioBatteryEventArgs;
            PlayerEvent.TriggeringTesla -= OnTriggeringTesla;
            PlayerEvent.Escaping -= OnEscaping;
            PlayerEvent.Dying -= OnDying;
        }
        private void OnEscaping(EscapingEventArgs ev)
        {
            if (!ev.Player.IsCuffed) return;

            if (ev.Player.Role.Team == Team.ChaosInsurgency) { ev.IsAllowed = true; ev.EscapeScenario = EscapeScenario.CuffedClassD; } // МОГ
            if (ev.Player.Role.Team == Team.FoundationForces) { ev.IsAllowed = true; ev.EscapeScenario = EscapeScenario.CuffedScientist; } // ПХ
        }
        private void OnDying(DyingEventArgs ev)
        {
            if(ev.DamageHandler.Type == DamageType.Scp0492)
            {
                ev.Player.Role.Set(RoleTypeId.Scp0492);
            }
        }
        private void OnVerified(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(0.2f, () =>
            {
                if (ev.Player.GroupName.StartsWith("d1") || ev.Player.GroupName.StartsWith("d2"))
                {
                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Insert(new VIPLog()
                    {
                        ID = ev.Player.UserId,
                    });
                }
            });
        }
        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if ((ev.Player.Role.Side == Side.ChaosInsurgency || ev.Player.Role.Side == Side.Scp) && !CustomRole.Get((uint)2).Check(ev.Player)) return;

            ev.IsAllowed = false;
            ev.IsInIdleRange = false;
        }
        private void OnUsingRadioBatteryEventArgs(UsingRadioBatteryEventArgs ev) => ev.Drain = 0f;
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
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
                ev.Items.Add(ServerHandler.ItemTypeToDclass.RandomItem());
            }
        }
        private void OnLeft(LeftEventArgs ev)
        {
            ev.Player.IsOverwatchEnabled = false;

            if (Res.DiedWithSCP500R.Count == 0) return;

            if(ev.Player == Res.DiedWithSCP500R.First())
            {
                Res.DiedWithSCP500R.Clear();
                Res.StatusEffectBase.Clear();
                Res.RoleDiedWithSCP500R.Clear();
            }
        }
    }
}