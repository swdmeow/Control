namespace Control.Handlers.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Map;
    using InventorySystem.Items.Firearms.Ammo;
    using MapEvent = Exiled.Events.Handlers.Map;

    internal sealed class MapHandler
    {
        public MapHandler()
        {
            MapEvent.PlacingBlood += OnPlacingBlood;
            MapEvent.PlacingBulletHole += OnPlacingBulletHole;
            MapEvent.SpawningItem += OnSpawningItem;
            MapEvent.AnnouncingScpTermination += OnAnnouncingScpTermination;
            MapEvent.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
        }
        public void OnDisabled()
        {
            MapEvent.PlacingBlood -= OnPlacingBlood;
            MapEvent.PlacingBulletHole -= OnPlacingBulletHole;
            MapEvent.SpawningItem -= OnSpawningItem;
            MapEvent.AnnouncingScpTermination -= OnAnnouncingScpTermination;
            MapEvent.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;
        }
        private void OnSpawningItem(SpawningItemEventArgs ev)
        {
            if(ev.Pickup.Base is AmmoPickup)
            {
                ev.IsAllowed = false;
            }
        }
        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ServerHandler.CassieDestroyedLVL == 0) return;

            if(ServerHandler.CassieDestroyedLVL >= 3)
            {
                //if(ev.TerminationCause == Term)
                Cassie.Message("SCP-███<b></b> успе█нjjjj у̸н̸и̴ч̸т████. При█ин█ с̸͓̍м̶̟͛е̵̰̰̽̈р̵͍͑ти - н̸е̸известна.. <color=#ffffff00>h pitch_0.8 SCP . pitch_0.6 .G6 .g1 . .g1 . .g1 . has . been .g3 . SUCCESSFULLY . .g4 terminated . .g4 . termination cause is pitch_0.5 unspecified . .g5 . pitch_0.3 .g5 pitch_0.1 .g5", false, false, true);

                ev.IsAllowed = false;
            }

        }
        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            if (ServerHandler.CassieDestroyedLVL == 0) return;

            if (ServerHandler.CassieDestroyedLVL >= 3)
            {
                ev.IsAllowed = false;
                Cassie.Message("pitch_1 .g6 pitch_0.4 . .g5", true, false, false);

                return;
            }

            if (ServerHandler.CassieDestroyedLVL >= 1)
            {
                Cassie.Message("pitch_1 .g6 pitch_0.4 . .g5 . . .g4 . . .g4 . . pitch_0.96  pitch_0.5 .g1 .g2 pitch_0.3 .g2 .g3 pitch_1. . pitch_0.9 pitch_0.2 .g5 .g4 pitch_1", true, false, false); ;
                ev.IsAllowed = false;

                return;
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