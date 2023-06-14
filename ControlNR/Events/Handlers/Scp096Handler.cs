namespace Control.Handlers.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Сontrol;
    using MEC;
    using Mirror;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;
    using Exiled.Events.EventArgs.Scp096;

    internal sealed class Scp096Handler
    {
        public Scp096Handler()
        {
            Exiled.Events.Handlers.Scp096.Enraging += OnEnraging;
        }
        public void OnDisabled()
        {
            Exiled.Events.Handlers.Scp096.Enraging -= OnEnraging;
        }
        private void OnEnraging(EnragingEventArgs ev)
        {
            Player pl = ev.Player;
            pl.CurrentRoom.TurnOffLights(1f);

            Timing.CallDelayed(0.1f, () => pl.CurrentRoom.TurnOffLights(1f));
            Timing.CallDelayed(0.2f, () => pl.CurrentRoom.TurnOffLights(1f));
            Timing.CallDelayed(0.3f, () => pl.CurrentRoom.TurnOffLights(1f));
            Timing.CallDelayed(0.4f, () => pl.CurrentRoom.TurnOffLights(1f));

        }
    }
}