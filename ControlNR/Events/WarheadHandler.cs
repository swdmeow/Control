namespace Control.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using MEC;
    using Mirror;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Map;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using Exiled.CustomItems.API.Features;
    using UnityEngine;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Server;
    using System.Threading;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles;
    using MapGeneration.Distributors;
    using InventorySystem.Items.Firearms.Attachments;
    using Control;
    using Exiled.API.Features.Items;
    using Сontrol;
    using XPSystem.API.Serialization;
    using Respawning;
    using Control.Commands;
    using Respawning.NamingRules;
    using Control.Extensions;
    using InventorySystem.Items.Pickups;
    using static Mono.Security.X509.X520;
    using HarmonyLib;
    using CommandSystem.Commands.RemoteAdmin.ServerEvent;
    using CustomPlayerEffects;
    using Org.BouncyCastle.Crypto.Generators;
    using WarheadEvent = Exiled.Events.Handlers.Warhead;
    using Exiled.Events.EventArgs.Warhead;
    using System.Runtime.Remoting.Messaging;
    using Control.CustomRoles;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class WarheadHandler
    {
        public static CoroutineHandle ChangeColorsCoroutineHandle;
        public static CoroutineHandle DecontamitionSequnse;

        public void OnEnabled()
        {
            WarheadEvent.Stopping += OnStopping;
            WarheadEvent.Starting += OnStaring;
            WarheadEvent.Detonating += OnDetonating;

        }
        public void OnDisabled()
        {
            WarheadEvent.Stopping -= OnStopping;
            WarheadEvent.Starting -= OnStaring;
            WarheadEvent.Detonating -= OnDetonating;

        }
        private async void OnDetonating(DetonatingEventArgs ev)
        {
            Control.API.Extensions.StopAudio();

            foreach (Room room in Room.List)
            {
                room.FlickerableLightController.WarheadLightOverride = false;

                room.ResetColor();
            }
            Timing.KillCoroutines(ChangeColorsCoroutineHandle);

            Cassie.Message("Attention . DECONTAMINATION process of surface zone will be started in TMINUS . 1 minute", true, false, false); ;
            foreach (Player pl in Player.List)
            {
                pl.Broadcast(new Broadcast("<size=75%><color=green>Обеззараживание</color> уличной зоны начнется через <color=red>1 минуту..</color></size>", 15), true);
            }

            await Task.Delay(60000);

            if (!Warhead.IsDetonated) return;

            Cassie.Message("Attention . DECONTAMINATION process of surface zone has been started", true, false, false);

            foreach (Player pl in Player.List)
            {
                pl.Broadcast(new Broadcast("<size=75%><color=green>Обеззараживание</color> уличной зоны <color=red>начато..</color></size>", 15), true);
            }

            DecontamitionSequnse = Timing.RunCoroutine(DecontaminationProcess());
        }
        private void OnStopping(StoppingEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            Control.API.Extensions.StopAudio();

            foreach (Room room in Room.List)
            {
                room.FlickerableLightController.WarheadLightOverride = false;

                room.ResetColor();
            }
            Timing.KillCoroutines(ChangeColorsCoroutineHandle);
        }
        private void OnStaring(StartingEventArgs ev)
        {
            Control.API.Extensions.PlayAudio($"{new System.Random().Next(1, Directory.GetFiles(Path.Combine(Paths.Configs, "ControlNR/Music/")).Length)}.ogg", 75, true, "da.mp9");

            ChangeColorsCoroutineHandle = Timing.RunCoroutine(ChangeColors());
        }
        public static IEnumerator<float> ChangeColors()
        {
            for (; ; )
            {
                foreach (Room room in Room.List)
                {
                    room.Color = Colors.ElementAt(new System.Random().Next(0, Colors.Count()));
                }

                yield return Timing.WaitForSeconds(1.1f);
            }
        }
        public static IEnumerator<float> DecontaminationProcess()
        {
            for (; ; )
            {
                foreach(Player pl in Player.List.Where(x => x.IsAlive))
                {
                    pl.EnableEffect(EffectType.Decontaminating, 1f);
                }

                yield return Timing.WaitForSeconds(5.1f);
            }
        }
        private static List<UnityEngine.Color> Colors = new List<UnityEngine.Color>()
        {

                    new UnityEngine.Color(255f / 255f, 255f / 255f, 255f / 255f, 1f / 255f),
                    new UnityEngine.Color(255f / 255f, 0f / 255f, 0f / 255f, 1f / 255f),
                    new UnityEngine.Color(0f / 255f, 255f / 255f, 0f / 255f, 1f / 255f),
                    new UnityEngine.Color(0f / 255f, 0f / 255f, 255f / 255f / 255f, 1f / 255f),
                    new UnityEngine.Color(255f / 255f, 255f / 255f, 0f / 255f, 1f / 255f),
                    new UnityEngine.Color(0f / 255f, 255f / 255f, 255f / 255f, 1f / 255f),
                    new UnityEngine.Color(255f / 255f, 0f / 255f, 255f / 255f, 1f / 255f),
                    new UnityEngine.Color(192f / 255f, 192f / 255f, 192f / 255f, 1f / 255f),
                    new UnityEngine.Color(128f / 255f, 128f / 255f, 128f / 255f, 1f / 255f),
                    new UnityEngine.Color(128f / 255f, 0f / 255f, 0f / 255f, 1f / 255f),
                    new UnityEngine.Color(128f / 255f, 128f / 255f, 0f / 255f, 1f) / 255f,
                    new UnityEngine.Color(0f / 255f, 128f / 255f, 0f / 255f, 1f / 255f),
                    new UnityEngine.Color(128f / 255f, 0f / 255f, 128f / 255f, 1f / 255f),
                    new UnityEngine.Color(0f / 255f, 128f / 255f, 128f / 255f, 1f / 255f),
                    new UnityEngine.Color(0f / 255f, 0f / 255f, 128f / 255f, 1f / 255f),
        };
    }
}