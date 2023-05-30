namespace Control.Handlers.Events
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

    internal sealed class WarheadDecontamition
    {
        public static CoroutineHandle DecontamitionSequnse;

        public WarheadDecontamition()
        {
            WarheadEvent.Detonating += OnDetonating;
        }
        public void OnDisabled()
        {
            WarheadEvent.Detonating -= OnDetonating;
        }
        private async void OnDetonating(DetonatingEventArgs ev)
        {
            foreach (Pickup pickup in Pickup.List)
            {
                if (pickup.Room.Type != RoomType.Surface)
                {
                    pickup.Destroy();
                }
            }
            foreach (Ragdoll ragdoll in Ragdoll.List)
            {
                if (ragdoll.Room.Type != RoomType.Surface)
                {
                    ragdoll.Destroy();
                }
            }

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
        public static IEnumerator<float> DecontaminationProcess()
        {
            for (; ; )
            {
                if (!Warhead.IsDetonated) break;
                foreach (Player pl in Player.List.Where(x => x.IsAlive))
                {
                    pl.EnableEffect(EffectType.Decontaminating, 1f);
                }

                yield return Timing.WaitForSeconds(5.1f);
            }
        }
    }
}