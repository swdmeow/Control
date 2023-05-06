namespace Control.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using System;
    using Сontrol;
    using UnityEngine;
    using Exiled.CustomRoles.API.Features;
    using Exiled.API.Features.Pickups;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.Events.EventArgs.Scp079;

    internal sealed class Scp079Handler
    {
        public void OnChangingCamera(ChangingCameraEventArgs ev)
        {
            Log.Info("Detected change camera.. debuging");
            Log.Info($"Camera: {ev.Camera}, Player: {ev.Player}, {ev.AuxiliaryPowerCost}");
            if (ev.Camera.Room == null)
            {
                ev.IsAllowed = false;
            }
        }
        public void OnEnabled()
        {

        }
    }
}