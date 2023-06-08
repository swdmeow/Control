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
                // 76561199014366139@steam - topar
             
                if (ev.Player.GroupName.StartsWith("d1") || ev.Player.GroupName.StartsWith("d2"))
                {
                    // Add player to DB;
                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Insert(new VIPLog()
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
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player.IsScp) return;

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

            if(ev.Items.Count >= 8)
            {
                if (ev.Player.GroupName.StartsWith("d2"))
                {
                    Timing.CallDelayed(0.1f, () => CustomItem.Get((uint)7).Give(ev.Player));
                } else
                {
                    if (Loader.Random.Next(100) > 77) Timing.CallDelayed(0.1f, () => CustomItem.Get((uint)7).Give(ev.Player));
                }
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
    }
}