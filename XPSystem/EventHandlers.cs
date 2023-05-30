using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using System.Linq;
using MEC;
using XPSystem.API;

namespace XPSystem
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

    public class EventHandlers
    {
        public void OnJoined(VerifiedEventArgs ev)
        {
            ev.Player.GetLog();
            Timing.CallDelayed(0.5f, () =>
            {
                API.API.UpdateBadge(ev.Player, ev.Player.Group?.BadgeText);
            });
        }

        public void OnKill(DyingEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (ev.Player == null) return;

            Player killer = ev.DamageHandler.Type == DamageType.PocketDimension ? Player.Get(RoleTypeId.Scp106).FirstOrDefault() : ev.Attacker;
            if (killer == null) return;

            if (Main.Instance.Config.KillXP.TryGetValue(ev.Player.Role.Type, out var PlayerKillXP))
            {
                var log = killer.GetLog();

                log.AddXP(PlayerKillXP, Main.GetTranslation($"kill{ev.Player.Role.Type.ToString()}"));
                log.UpdateLog();
            } else
            {
                Log.Warn("PlayerKillXP == null");
            }
        }

        public void OnEscape(EscapingEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (CustomRole.Get((uint)1).Check(ev.Player) || CustomRole.Get((uint)2).Check(ev.Player)) return;

            if (!Main.Instance.Config.EscapeXP.TryGetValue(ev.Player.Role, out int xp))
            {
                Log.Warn($"No escape XP for {ev.Player.Role}");
                return;
            }

            var log = ev.Player.GetLog();
            log.AddXP(xp, Main.GetTranslation("escape"));
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
                log.AddXP(Main.Instance.Config.TeamWinXP, Main.GetTranslation("teamwin"));
                log.UpdateLog();
            }
        }
    }
}
