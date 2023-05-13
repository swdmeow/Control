namespace Control.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Сontrol;
    using MEC;
    using Mirror;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Map;
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System;
    using Control.Events;
    using Steamworks.ServerList;

    internal sealed class MapHandler
    {
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Map.PlacingBlood += OnPlacingBlood;
            Exiled.Events.Handlers.Map.PlacingBulletHole += OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.SpawningItem += OnSpawningItem;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination += OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
        }
        public void OnDisabled()
        {
            Exiled.Events.Handlers.Map.PlacingBlood -= OnPlacingBlood;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.SpawningItem -= OnSpawningItem;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;

        }
        private void OnSpawningItem(SpawningItemEventArgs ev)
        {
            if(ev.Pickup.Type == ItemType.Ammo12gauge ||
               ev.Pickup.Type == ItemType.Ammo44cal ||
               ev.Pickup.Type == ItemType.Ammo556x45 ||
               ev.Pickup.Type == ItemType.Ammo762x39 ||
               ev.Pickup.Type == ItemType.Ammo9x19)
            {
                ev.IsAllowed = false;
            }
        }
        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ServerHandler.CassieDestroyedLVL == 0) return;

            if(ServerHandler.CassieDestroyedLVL >= 1)
            {
                //if(ev.TerminationCause == Term)
                Cassie.Message("SCP-███<b></b> успе█нjjjj у̸н̸и̴ч̸т████. При█ин█ с̸͓̍м̶̟͛е̵̰̰̽̈р̵͍͑ти - н̸е̸известна.. <color=#ffffff00>h pitch_0.8 SCP . pitch_0.6 .G6 .g1 . .g1 . .g1 . has . been .g3 . SUCCESSFULLY . .g4 terminated . .g4 . termination cause is pitch_0.5 unspecified . .g5 . pitch_0.3 .g5 pitch_0.1 .g5", false, false, true);

                ev.IsAllowed = false;
            }

        }
        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            if (ServerHandler.CassieDestroyedLVL == 0) return;

            if (ServerHandler.CassieDestroyedLVL >= 1)
            {
                Cassie.Message("pitch_1 .g6 pitch_0.4 . .g5 . . .g4 . . .g4 . . pitch_0.96  pitch_0.5 .g1 .g2 pitch_0.3 .g2 .g3 pitch_1. . pitch_0.9 pitch_0.2 .g5 .g4 .g7 pitch_1", true, false, false); ;
                ev.IsAllowed = false;
            }
        }
        private void OnPlacingBulletHole(PlacingBulletHole ev)
        {
            ev.IsAllowed = false;
        }
        private void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}