namespace Ñontrol
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomRoles.API.Features;
    using System.IO;
    using System;
    using MEC;
    using Control.Events;

    public class ControlNR : Plugin<Config>
    {
        public static ControlNR Singleton;
        public override string Name => "ControlGG";
        public override string Author => "swd";
        public override System.Version Version => new System.Version(1, 0, 0);

        private Control.Events.PlayerHandler PlayerHandler;
        public override void OnEnabled()
        {
            Singleton = this;

            PlayerHandler = new Control.Events.PlayerHandler();

            PlayerHandler.OnEnabled();

            CustomItem.RegisterItems();
            CustomRole.RegisterRoles(false, null, true);

            base.OnEnabled();
        }
        public override void OnReloaded() { }
        public override void OnDisabled()
        {
            PlayerHandler.OnDisabled();

            CustomItem.UnregisterItems();
            CustomRole.UnregisterRoles();

            PlayerHandler = null;

            Singleton = null;

            base.OnDisabled();
        }
    }
}