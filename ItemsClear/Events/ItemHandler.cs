using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using UnityEngine;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.Handlers;
using MEC;
using PlayerRoles;
using System.Linq;

namespace Alpha.Events
{
    internal sealed class ItemHandler
    {
        public void RoundStart()
        {
            Exiled.API.Features.Warhead.IsLocked = true;

            Round.IsLocked = true;
        }
        public void WaitingPlayers()
        {
            Timing.CallDelayed(10f, () =>
            {
                if (Exiled.API.Features.Player.List.Count() < 2) CharacterClassManager.ForceRoundStart();
            });
        }
        public void OnBanned(BannedEventArgs ev)
        {
            if (ev.Player.Nickname != "Dedicated Server")
            {
                ev.Details.Reason = ev.Details.Reason + $" | {ev.Player.Nickname}";
            }
            else
            {
               ev.Details.Reason = ev.Details.Reason + $" | {ev.Details.Issuer}";
            }
        }
        public void IssuingMute(IssuingMuteEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}