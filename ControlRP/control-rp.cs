namespace Ñontrol
{
    using Exiled.API.Features;
    using System;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.CustomItems;

    public class control : Plugin<Config>
    {
        private static readonly control Singleton = new();

        public override string Name => "Control-RP";
        public override string Author => "swd";
        public override Version Version => new Version(0, 1, 1);

        private Control.Events.PlayerHandler PlayerHandler;
        private Control.Events.ServerHandler ServerHandler;
        private Control.CustomItems.GrenadeLauncher GrenadeLauncher;


        private control()
        {

        }

        public static control Instance => Singleton;
        public override void OnEnabled()
        {
            CustomItem.RegisterItems();

            PlayerHandler = new Control.Events.PlayerHandler();
            ServerHandler = new Control.Events.ServerHandler();

            // CustomItems
            GrenadeLauncher = new Control.CustomItems.GrenadeLauncher();
            // CustomItems Events
            Exiled.Events.Handlers.Player.Shooting += GrenadeLauncher.OnShooting;
            Exiled.Events.Handlers.Player.ReloadingWeapon += GrenadeLauncher.OnReloading;
            Exiled.Events.Handlers.Player.UnloadingWeapon += GrenadeLauncher.OnUnloadingWeapon;
            Exiled.Events.Handlers.Player.PickingUpItem += GrenadeLauncher.OnPickUpItem;

            Exiled.Events.Handlers.Server.RoundStarted += ServerHandler.OnRoundStarted;
            Exiled.Events.Handlers.Player.Spawned += PlayerHandler.OnSpawned;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {

            Exiled.Events.Handlers.Player.Shooting -= GrenadeLauncher.OnShooting;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= GrenadeLauncher.OnReloading;
            Exiled.Events.Handlers.Player.UnloadingWeapon -= GrenadeLauncher.OnUnloadingWeapon;
            Exiled.Events.Handlers.Player.PickingUpItem -= GrenadeLauncher.OnPickUpItem;


            CustomItem.UnregisterItems();

            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandler.OnRoundStarted;
            Exiled.Events.Handlers.Player.Spawned -= PlayerHandler.OnSpawned;

            PlayerHandler = null;
            ServerHandler = null;
            GrenadeLauncher = null;

            base.OnDisabled();
        }
    }
}