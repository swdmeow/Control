namespace Ñontrol
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Control.Events;
    using Exiled.API.Enums;
    using LiteDB;
    using System.IO;
    using HarmonyLib;
    using System;
    using Exiled.Events.EventArgs.Scp079;
    using XPSystem.API.Serialization;
    using MEC;
    using Control.Extensions;
    using Control.CustomRoles;

    public class ControlNR : Plugin<Config>
    {
        public static ControlNR Singleton;
        public override string Name => "ControlNR";
        public override string Author => "swd";
        public override System.Version Version => new System.Version(1, 5, 0);

        private Control.Events.PlayerHandler PlayerHandler;
        private Control.Events.ServerHandler ServerHandler;
        private Control.Events.Scp330Handler Scp330Handler;
        private Control.Events.MapHandler MapHandler;
        private Harmony harmony;

        public LiteDatabase db;
        public override void OnEnabled()
        {
            if (!Directory.Exists(Path.Combine(Paths.Configs, "ControlNR"))) Directory.CreateDirectory(Path.Combine(Paths.Configs, "ControlNR"));

            db = new LiteDatabase(Path.Combine(Paths.Configs, @".\ControlNR\LimitDonator.db"));
            Singleton = this;

            harmony = new Harmony($"ControlNR - {DateTime.Now.Ticks}");

            PlayerHandler = new Control.Events.PlayerHandler();
            ServerHandler = new Control.Events.ServerHandler();
            MapHandler = new Control.Events.MapHandler();
            Scp330Handler = new Control.Events.Scp330Handler();

            ServerHandler.OnEnabled();
            Scp330Handler.OnEnabled();
            MapHandler.OnEnabled();
            PlayerHandler.OnEnabled();

            CustomItem.RegisterItems();
            CustomRole.RegisterRoles();

            if (PlayerExtensions.HintCoroutineHandle == null || !PlayerExtensions.HintCoroutineHandle.Value.IsValid || !PlayerExtensions.HintCoroutineHandle.Value.IsRunning)
                PlayerExtensions.HintCoroutineHandle = Timing.RunCoroutine(PlayerExtensions.HintCoroutine());

            if (SCP343.HintCooldownCoroutineHandle == null || !SCP343.HintCooldownCoroutineHandle.Value.IsValid || !SCP343.HintCooldownCoroutineHandle.Value.IsRunning)
                SCP343.HintCooldownCoroutineHandle = Timing.RunCoroutine(SCP343.HintCoroutine());

            harmony.PatchAll();
            
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            
            Singleton = null;

            PlayerHandler = null;
            ServerHandler = null;
            MapHandler = null;
            Scp330Handler = null;

            harmony.UnpatchAll();
            harmony = null;

            db.Dispose();
            db = null;

            ServerHandler.OnDisabled();
            Scp330Handler.OnDisabled();
            MapHandler.OnDisabled();
            PlayerHandler.OnDisabled();

            CustomItem.UnregisterItems();
            CustomRole.UnregisterRoles();

            base.OnDisabled();
        }
    }
}