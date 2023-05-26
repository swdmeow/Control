using Control.Handlers.Events;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;

namespace Control.Handlers
{
    internal sealed class Handler
    {
        private Control.Handlers.Events.PlayerHandler PlayerHandler;
        private Control.Handlers.Events.ServerHandler ServerHandler;
        private Control.Handlers.Events.Scp330Handler Scp330Handler;
        private Control.Handlers.Events.MapHandler MapHandler;
        private Control.Handlers.Events.WarheadMusic WarheadMusic;
        private Control.Handlers.Events.Scp914Handler Scp914Handler;
        private Control.Handlers.Events.UnlimitedAmmoHandler UnlimitedAmmoHandler;
        private Control.Handlers.Events.RemoteKeycardHandler RemoteKeycardHandler;

        public Handler()
        {
            PlayerHandler = new Control.Handlers.Events.PlayerHandler();
            ServerHandler = new Control.Handlers.Events.ServerHandler();
            MapHandler = new Control.Handlers.Events.MapHandler();
            Scp330Handler = new Control.Handlers.Events.Scp330Handler();
            WarheadMusic = new Control.Handlers.Events.WarheadMusic();
            Scp914Handler = new Control.Handlers.Events.Scp914Handler();
            UnlimitedAmmoHandler = new Control.Handlers.Events.UnlimitedAmmoHandler();
            RemoteKeycardHandler = new Control.Handlers.Events.RemoteKeycardHandler();
        }
        public void Dispose()
        {
            ServerHandler.OnDisabled();
            Scp330Handler.OnDisabled();
            MapHandler.OnDisabled();
            PlayerHandler.OnDisabled();
            WarheadMusic.OnDisabled();
            Scp914Handler.OnDisabled();
            RemoteKeycardHandler.OnDisabled();
            UnlimitedAmmoHandler.OnDisabled();

            RemoteKeycardHandler = null;
            UnlimitedAmmoHandler = null;
            PlayerHandler = null;
            ServerHandler = null;
            MapHandler = null;
            Scp330Handler = null;
            WarheadMusic = null;
        }
    }
}