using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using System.Linq;
using MEC;

namespace Control.Handlers.Events
{
    using System.Collections.Generic;
    using Exiled.API.Extensions;
    using Exiled.CustomItems.API.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp914;
    using Exiled.Events.EventArgs.Server;
    using PlayerRoles;
    using Exiled.CustomRoles;
    using Exiled.CustomRoles.API.Features;
   
    using ServerEvent = Exiled.Events.Handlers.Server;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using Control.Handlers.Events.API;
    using Сontrol;

    public class XPHandler
    {
        public XPHandler()
        {
            PlayerEvent.Verified += OnJoined;
            PlayerEvent.Dying += OnKill;
            ServerEvent.RoundEnded += OnRoundEnd;
            PlayerEvent.Escaping += OnEscape;
            ServerEvent.ReloadedRA += ReloadedRA;
        }
        public void OnDisabled()
        {
            PlayerEvent.Verified -= OnJoined;
            PlayerEvent.Dying -= OnKill;
            ServerEvent.RoundEnded -= OnRoundEnd;
            PlayerEvent.Escaping -= OnEscape;
            ServerEvent.ReloadedRA -= ReloadedRA;
        }
        public void OnJoined(VerifiedEventArgs ev)
        {
            ev.Player.GetLog();
            Timing.CallDelayed(0.2f, () =>
            {
                API.API.UpdateBadge(ev.Player, ev.Player.Group?.BadgeText);
            });
        }
        public void ReloadedRA()
        {
            foreach (Player pl in Player.List.Where(x => x.Group == null))
            {
                API.API.UpdateBadge(pl);
            }
        }
        public void OnKill(DyingEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (ev.Player == null) return;
            if (ev.Player == ev.Attacker) return;

            Player killer = ev.DamageHandler.Type == DamageType.PocketDimension ? Player.Get(RoleTypeId.Scp106).FirstOrDefault() : ev.Attacker;
            if (killer == null) return;

            if (ControlNR.Singleton.Config.XPSystem.KillXP.TryGetValue(ev.Player.Role.Type, out var PlayerKillXP))
            {
                var log = killer.GetLog();

                log.AddXP(PlayerKillXP);
                log.UpdateLog();
            } else
            {
                Log.Warn($"PlayerKillXP == null ({ev.Player.Role.Type})") ;
            }
        }

        public void OnEscape(EscapingEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (CustomRole.Get((uint)1).Check(ev.Player) || CustomRole.Get((uint)2).Check(ev.Player)) return;

            if (!ControlNR.Singleton.Config.XPSystem.EscapeXP.TryGetValue(ev.Player.Role, out int xp))
            {
                Log.Warn($"No escape XP for {ev.Player.Role}");
                return;
            }

            var log = ev.Player.GetLog();
            log.AddXP(xp);
            log.UpdateLog();
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {

            Side team;
            switch (ev.LeadingTeam)
            {
                case LeadingTeam.FacilityForces:
                    team = Side.Mtf;
                    break;
                case LeadingTeam.ChaosInsurgency:
                    team = Side.ChaosInsurgency;
                    break;
                case LeadingTeam.Anomalies:
                    team = Side.Scp;
                    break;
                case LeadingTeam.Draw:
                    team = Side.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            foreach (var player in Player.Get(team))
            {
                var log = player.GetLog();
                if (log is null)
                    return;
                log.AddXP(ControlNR.Singleton.Config.XPSystem.TeamWinXP);
                log.UpdateLog();
            }
        }
    }
}
