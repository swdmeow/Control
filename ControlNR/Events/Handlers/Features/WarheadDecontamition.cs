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
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Extensions;

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

            Cassie.Message("jam_007_2 Attention . . pitch_0.3 jam_080_8 .g3 . pitch_1.1 jam_090_9 RADIATION of jam_010_1 Alpha warhead pitch_0.4 .g1 pitch_1 will be pitch_2.6 .g2 pitch_0.9 to pitch_1 surface jam_090_1 zone in . . pitch_7 1 1 1 1 1 1 1 1 1 pitch_1 TMINUS . . . jam_191_5 1 pitch_0.7 minute pitch_7 .g1 .g1 .g1 .g1 .g1 .g1", true, false, false); ;

            Map.Broadcast(15, "<size=75%><color=red>Радиация</color> вызванная взрывом альфа-боеголовки прибудет на уличную часть комплекса через <color=red>1 минуту..</color></size>");

            await Task.Delay(60000);

            if (!Warhead.IsDetonated) return;

            Cassie.Message(" pitch_3 .G6 .G6 .G6 .G6 .g6 .g6 .g6 .g6 .g6", true, false, false);

            DecontamitionSequnse = Timing.RunCoroutine(DecontaminationProcess());
        }
        public static IEnumerator<float> DecontaminationProcess()
        {

            for (; ; )
            {
                yield return Timing.WaitForSeconds(2.5f);

                if (!Warhead.IsDetonated) Timing.KillCoroutines(DecontamitionSequnse);

                foreach (Player pl in Player.List.Where(x => x.IsAlive))
                {
                    float HPToHurt = pl.MaxHealth / (pl.IsScp ? 15 : 10);

                    pl.EnableEffect(EffectType.Burned);

                    pl.Hurt(HPToHurt, DamageType.Warhead);
                }
            }
        }
    }
}