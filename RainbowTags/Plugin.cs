namespace RainbowTags
{
    using System;
    using Exiled.API.Features;
    using PlayerHandlers = Exiled.Events.Handlers.Player;

    public class Plugin : Plugin<Config>
    {
        private EventHandlers eventHandlers;

        public override string Name { get; } = "RainbowTags";
        public override Version RequiredExiledVersion { get; } = new Version(5, 0, 0);
        public override string Author { get; } = "swd";

        public override void OnEnabled()
        {
            eventHandlers = new EventHandlers(this);
            PlayerHandlers.ChangingGroup += eventHandlers.OnChangingGroup;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            PlayerHandlers.ChangingGroup -= eventHandlers.OnChangingGroup;
            eventHandlers = null;
            base.OnDisabled();
        }
    }
}