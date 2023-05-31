namespace Ñontrol
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomRoles.API.Features;
    using LiteDB;
    using System.IO;
    using HarmonyLib;
    using System;
    using MEC;
    using Control.Extensions;

    public class ControlNR : Plugin<Config>
    {
        public static ControlNR Singleton;
        public override string Name => "ControlNR";
        public override string Author => "swd";
        public override System.Version Version => new System.Version(2, 1, 0);

        private Harmony harmony;

        private Control.Handlers.Handler Handler;

        public LiteDatabase db;
        public LiteDatabase XPdb;
        public override void OnEnabled()
        {
            if (!Directory.Exists(Path.Combine(Paths.Configs, "ControlNR"))) Directory.CreateDirectory(Path.Combine(Paths.Configs, "ControlNR"));
            if (!Directory.Exists(Path.Combine(Paths.Configs, "ControlNR/Music"))) Directory.CreateDirectory(Path.Combine(Paths.Configs, "ControlNR/Music"));

            db = new LiteDatabase(Path.Combine(Paths.Configs, @"ControlNR/LimitDonator.db"));
            XPdb = new LiteDatabase(Path.Combine(Paths.Configs, @"ControlNR/XPUser.db"));
            Singleton = this;

            harmony = new Harmony($"ControlNR - {DateTime.Now.Ticks}");

            Handler = new Control.Handlers.Handler();

            CustomItem.RegisterItems();
            CustomRole.RegisterRoles(false, null, true);

            if (Control.Extensions.PlayerExtensions.HintCoroutineHandle == null || !Control.Extensions.PlayerExtensions.HintCoroutineHandle.Value.IsValid || !Control.Extensions.PlayerExtensions.HintCoroutineHandle.Value.IsRunning)
                Control.Extensions.PlayerExtensions.HintCoroutineHandle = Timing.RunCoroutine(Control.Extensions.PlayerExtensions.HintCoroutine());

            if (Control.CustomRoles.SCP343.HintCooldownCoroutineHandle == null || !Control.CustomRoles.SCP343.HintCooldownCoroutineHandle.Value.IsValid || !Control.CustomRoles.SCP343.HintCooldownCoroutineHandle.Value.IsRunning)
                Control.CustomRoles.SCP343.HintCooldownCoroutineHandle = Timing.RunCoroutine(Control.CustomRoles.SCP343.HintCoroutine());

            if (HintExtensions.WriteHintCoroutineHandle == null || !HintExtensions.WriteHintCoroutineHandle.Value.IsValid || !HintExtensions.WriteHintCoroutineHandle.Value.IsRunning)
                HintExtensions.WriteHintCoroutineHandle = Timing.RunCoroutine(HintExtensions.WriteHint());

            harmony.PatchAll();
            
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            harmony.UnpatchAll();
            harmony = null;

            db.Dispose();
            db = null;

            XPdb.Dispose();
            XPdb = null;

            Handler.Dispose();
            Handler = null;

            CustomItem.UnregisterItems();
            CustomRole.UnregisterRoles();

            Singleton = null;

            base.OnDisabled();
        }
    }
}