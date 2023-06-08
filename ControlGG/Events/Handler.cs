namespace Control.Handlers
{
    internal sealed class Handler
    {
        private Control.Handlers.Events.PlayerHandler PlayerHandler;
        private Control.Handlers.Events.ServerHandler ServerHandler;

        public Handler()
        {
            PlayerHandler = new Control.Handlers.Events.PlayerHandler();
            ServerHandler = new Control.Handlers.Events.ServerHandler();
        }
        public void Dispose()
        {
            ServerHandler.OnDisabled();
            PlayerHandler.OnDisabled();

            PlayerHandler = null;
            ServerHandler = null;
        }
    }
}