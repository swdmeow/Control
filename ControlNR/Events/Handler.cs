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
        private Control.Handlers.Events.WarheadHandler WarheadHandler;
        private Control.Handlers.Events.Scp914Handler Scp914Handler;
        public Handler()
        {
            PlayerHandler = new Control.Handlers.Events.PlayerHandler();
            ServerHandler = new Control.Handlers.Events.ServerHandler();
            MapHandler = new Control.Handlers.Events.MapHandler();
            Scp330Handler = new Control.Handlers.Events.Scp330Handler();
            WarheadHandler = new Control.Handlers.Events.WarheadHandler();
            Scp914Handler = new Control.Handlers.Events.Scp914Handler();

        }
        public void Dispose()
        {
            ServerHandler.OnDisabled();
            Scp330Handler.OnDisabled();
            MapHandler.OnDisabled();
            PlayerHandler.OnDisabled();
            WarheadHandler.OnDisabled();
            Scp914Handler.OnDisabled();

            PlayerHandler = null;
            ServerHandler = null;
            MapHandler = null;
            Scp330Handler = null;
            WarheadHandler = null;
        }
    }
}