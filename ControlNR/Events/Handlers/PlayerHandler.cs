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
        }
        public void OnDisabled()
        {
            PlayerEvent.ChangingRole -= OnChangingRole;
            PlayerEvent.Left -= OnLeft;
            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.UsingRadioBattery -= OnUsingRadioBatteryEventArgs;
            PlayerEvent.TriggeringTesla -= OnTriggeringTesla;
            PlayerEvent.Escaping -= OnEscaping;
        }
        private void OnEscaping(EscapingEventArgs ev)
        {
            Log.Info(ev.Player.Role);

            if (ev.IsAllowed) return;

            if (!ev.Player.IsCuffed) return;

            if (ev.Player.Role.Team == Team.ChaosInsurgency) { ev.IsAllowed = true; ev.EscapeScenario = EscapeScenario.CuffedClassD; } // МОГ
            if (ev.Player.Role.Team == Team.FoundationForces) { ev.IsAllowed = true; ev.EscapeScenario = EscapeScenario.CuffedScientist; } // ПХ
        }
        private void OnVerified(VerifiedEventArgs ev)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                if (ev.Player.GroupName.StartsWith("d1") || ev.Player.GroupName.StartsWith("d2"))
                {
                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Insert(new VIPLog());
                }
            });
        }
        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
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
                ev.Items.Add(ItemType.KeycardJanitor);
            } 
        }
        private void OnLeft(LeftEventArgs ev)
        {
            ev.Player.IsOverwatchEnabled = false;
        }
    }
}