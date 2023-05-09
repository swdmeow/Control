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
    using MapEditorReborn.API.Enums;
    using Respawning;
    using Control.Commands;
    using Respawning.NamingRules;
    using Control.Extensions;
    using MapEditorReborn.API.Features;
    using MapEditorReborn.API.Features.Objects;
    using InventorySystem.Items.Pickups;
    using static Mono.Security.X509.X520;
    using MapEditorReborn.API.Features.Serializable;
    using HarmonyLib;
    using CommandSystem.Commands.RemoteAdmin.ServerEvent;
    using CustomPlayerEffects;
    using Org.BouncyCastle.Crypto.Generators;
    using Warhead = Exiled.Events.Handlers.Warhead;
    using Exiled.Events.EventArgs.Warhead;
    using System.Runtime.Remoting.Messaging;
    using Control.CustomRoles;
    using System.IO;

    internal sealed class WarheadHandler
    {
        public static CoroutineHandle ChangeColorsCoroutineHandle;
        public void OnEnabled()
        {
            Warhead.Stopping += OnStopping;
            Warhead.Starting += OnStaring;
            Warhead.Detonating += OnDetonating;
        }
        public void OnDisabled()
        {
            Warhead.Stopping -= OnStopping;
            Warhead.Starting -= OnStaring;
            Warhead.Detonating -= OnDetonating;
        }
        private void OnStopping(StoppingEventArgs ev)
        {
            Control.API.Extensions.StopAudio();

            foreach (Room room in Room.List)
            {
                room.Color = new UnityEngine.Color(255f / 255f, 255f / 255f, 255f / 255f);
            }
            Timing.KillCoroutines(ChangeColorsCoroutineHandle);
        }
        private void OnStaring(StartingEventArgs ev)
        {
            Control.API.Extensions.PlayAudio($"{new System.Random().Next(1, Directory.GetFiles(Path.Combine(Paths.Configs, "ControlNR/Music/")).Length)}.ogg", 75, true, "блять.mp8");

            ChangeColorsCoroutineHandle = Timing.RunCoroutine(ChangeColors());
        }
        private void OnDetonating(DetonatingEventArgs ev)
        {
            Control.API.Extensions.StopAudio();

            foreach (Room room in Room.List)
            {
                room.Color = new UnityEngine.Color(255f / 255f, 255f / 255f, 255f / 255f);
            }
            Timing.KillCoroutines(ChangeColorsCoroutineHandle);
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