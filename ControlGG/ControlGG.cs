namespace Ñontrol
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomRoles.API.Features;

    public class ControlGG : Plugin<Config>
    {
        public static ControlGG Singleton;
        public override string Name => "ControlGG";
        public override string Author => "swd";
        public override System.Version Version => new System.Version(1, 2, 0);

        private Control.Handlers.Handler Handler;
        public override void OnEnabled()
        {
            Singleton = this;
            Handler = new Control.Handlers.Handler();

            Log.Info($"Enabling {Name},\nAuthor: {Author}\nVersion: {Version}");

            CustomItem.RegisterItems();
            CustomRole.RegisterRoles(false, null, true, Assembly);

            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Handler.Dispose();

            CustomItem.UnregisterItems();
            CustomRole.UnregisterRoles();

            Singleton = null;
            Handler = null;

            base.OnDisabled();
        }
    }
}