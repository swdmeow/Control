namespace Control.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Сontrol;
    using MEC;
    using Mirror;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;

    internal sealed class Scp330Handler
    {
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Scp330.InteractingScp330 += InteractingScp330;
        }
        public void OnDisabled()
        {
            Exiled.Events.Handlers.Scp330.InteractingScp330 -= InteractingScp330;
        }
        private void InteractingScp330(InteractingScp330EventArgs ev)
        {
            var rand = new System.Random();

            if(rand.Next(0, 10) >= 9)
            {
                ev.Candy = CandyKindID.Pink;
            }
        }
    }
}