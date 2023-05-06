namespace Alpha
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Alpha.Events;
    using Exiled.Events.EventArgs.Warhead;
    using System.Threading;
    using Exiled.Events.Patches.Events.Player;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Radio;
    using HarmonyLib;
    using MEC;
    using Alpha.Patches;
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Spawn;

    public class alpha : Plugin<Config>
    {
        private static readonly alpha Singleton = new();

        private Alpha.Events.ItemHandler ItemHandler;

        private Harmony harmony;

        public override string Name => "Control-ALL";
        public override string Author => "swd";
        public override Version Version => new Version(1, 6, 0);

        private alpha()
        {

        }

        public static alpha Instance => Singleton;
        public override void OnEnabled()
        {
            try
            {
                harmony = new Harmony($"swd.ALL-{DateTime.Now.Ticks}");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            ItemHandler = new Alpha.Events.ItemHandler();

            SomeMethod();

            Exiled.Events.Handlers.Player.Banned += ItemHandler.OnBanned;
            Exiled.Events.Handlers.Player.IssuingMute += ItemHandler.IssuingMute;


            Exiled.Events.Handlers.Server.RoundStarted += ItemHandler.RoundStart;
            Exiled.Events.Handlers.Server.WaitingForPlayers += ItemHandler.WaitingPlayers;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            harmony?.UnpatchAll();

            Exiled.Events.Handlers.Player.Banned -= ItemHandler.OnBanned;
            Exiled.Events.Handlers.Player.IssuingMute -= ItemHandler.IssuingMute;

            Exiled.Events.Handlers.Server.RoundStarted -= ItemHandler.RoundStart;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= ItemHandler.WaitingPlayers;

            ItemHandler = null;

            base.OnDisabled();
        }

        public void SomeMethod()
        {
            Timing.RunCoroutine(MyCoroutine());
        }

        public IEnumerator<float> MyCoroutine()
        {
            for (; ; )
            {
                foreach (Pickup item in Pickup.List)
                {
                    foreach (var Type in alpha.Instance.Config.ListedItemType)
                    {
                        if (item.Type == Type)
                        {
                            UnityEngine.MonoBehaviour.Destroy(item.GameObject);
                        }
                    }
                }
                yield return Timing.WaitForSeconds(alpha.Instance.Config.ItemCountDoDestroy);
            }
        }
    }
}