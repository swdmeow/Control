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

    internal sealed class MapHandler
    {
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Map.PlacingBlood += OnPlacingBlood;
            Exiled.Events.Handlers.Map.PlacingBulletHole += OnPlacingBulletHole;

            Exiled.Events.Handlers.Map.AnnouncingScpTermination += OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
        }
        public void OnDisabled()
        {
            Exiled.Events.Handlers.Map.PlacingBlood -= OnPlacingBlood;
            Exiled.Events.Handlers.Map.PlacingBulletHole -= OnPlacingBulletHole;

            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;

        }
        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ServerHandler.CassieDestroyedLVL == 0) return;

            if(ServerHandler.CassieDestroyedLVL >= 1)
            {
                Cassie.Message("SCP-███<b></b> успе█нjjjj у̸н̸и̴ч̸т████. При█ин█ с̸͓̍м̶̟͛е̵̰̰̽̈р̵͍͑ти - н̸е̸известна.. <color=#ffffff00>h pitch_0.8 SCP . pitch_0.6 .G6 .g1 . .g1 . .g1 . has . been .g3 . SUCCESSFULLY . .g4 terminated . .g4 . termination cause is pitch_0.5 unspecified . .g5 . pitch_0.3 .g5 pitch_0.1 .g5", false, false, true);

                ev.IsAllowed = false;
            }
        }
        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            if (ServerHandler.CassieDestroyedLVL == 0) return;

            if (ServerHandler.CassieDestroyedLVL >= 1)
            {
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