﻿namespace Control.Events
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

    internal sealed class ServerHandler
    {
        //public static SchematicObject doorAirlock = null;
        //public static SchematicObject door035 = null;
        //public static SchematicObject room035 = null;
        //public static Vector3 positionToTeleport;
        //public static Vector3 pos;
        public static bool RoomRotated = false;

        private bool isWarheadCassie1Minute = false;
        private bool isWarheadStart = false;
        public static int CassieDestroyedLVL = 0;
        //public static HashSet<Pickup> InteractingItemsElevator { get; } = new HashSet<Pickup>();
        public void OnEnabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted += OnStartingRound;
        }
        public void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;
            Exiled.Events.Handlers.Server.RoundStarted -= OnStartingRound;
        }
        private void OnStartingRound()
        {
            //var SCPs = Player.List.Where(x => x.IsScp).ToArray();

            //string subtitles = $"Сканирование систем.. Сканирование систем заверешено.. обнаружено нарушение условий содержаний SCP-объектов {SCPs.Join(x => x.Role.Name, ", ")}";
            //string msg = $"Scanning System . . . pitch_0.9 .g1 pitch_0.85 .g1 pitch_0.8 .g1 pitch_0.775 .g1 pitch_0.74 .g1 . pitch_2 .g6 . . . pitch_0.9 .g2 pitch_1 .g2 pitch_1.05 .g2 pitch_1.1 .g2 pitch_1.135 .g2 pitch_1.16 .g2 . . . pitch_1.05 . system scan completed . .g1 . detected CONTAINMENT breach of {SCPs.Join(x => x.Role.Name.Replace("-", " "), " . ")}";

            //Cassie.Message($"{subtitles} <color=#ffffff00>h {msg}", false, false, true);

            //CustomItem.Get((uint)3).Spawn(Room.List.ElementAt(new System.Random().Next(0, Room.List.Count())).transform.position + Vector3.up);
            /*
            Room room = Room.Get(RoomType.LczAirlock);

            pos = room.transform.position;
            Quaternion rot = room.Rotation;
            Quaternion FuturePos = new Quaternion(0, 0, 0, 0);

            positionToTeleport = pos;

            if (rot == Quaternion.identity)
            {
                // rot = 0, 0, 0, 1
                pos.z -= 3.5f;
                positionToTeleport.z -= 6.5f;

                Vector3 ItemPos = pos;
                ItemPos.x += 1.0f;

                Pickup pickup = Pickup.CreateAndSpawn(ItemType.Adrenaline, pos + Vector3.up, new Quaternion(0, 0, 0, 0));
                Pickup scp035pickup = Pickup.CreateAndSpawn(ItemType.Adrenaline, pos + new Vector3(-11f, 101f, -0.5f), new Quaternion(0, 0, 0, 0));

                pickup.Base.PhysicsModule.DestroyModule();
                scp035pickup.Base.PhysicsModule.DestroyModule();

                InteractingItemsElevator.Add(pickup);
                InteractingItemsElevator.Add(scp035pickup);
            }
            else
            {
                RoomRotated = true;
                Log.Info("Room is rotated..");

                FuturePos = new Quaternion(0, 90, 0, 90);
                pos.x -= 3.5f;
                positionToTeleport.x -= 6.5f;



                Vector3 ItemPos = pos;
                ItemPos.x += 1.0f;

                Pickup pickup = Pickup.CreateAndSpawn(ItemType.Adrenaline, pos + Vector3.up, new Quaternion(0, 0, 0, 0));
                Pickup scp035pickup = Pickup.CreateAndSpawn(ItemType.Adrenaline, pos + new Vector3(-11f, 101f, -0.5f), new Quaternion(0, 0, 0, 0));

                pickup.Base.PhysicsModule.DestroyModule();
                scp035pickup.Base.PhysicsModule.DestroyModule();

                InteractingItemsElevator.Add(pickup);
                InteractingItemsElevator.Add(scp035pickup);
            }


            // Spawn first door
            doorAirlock = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic("Door", pos, FuturePos);
            // Spawn first door
            if (RoomRotated) door035 = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic("Door", pos + new Vector3(-8.6f, 101f, -0.5f), FuturePos);
            if (!RoomRotated) door035 = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic("Door", pos + new Vector3(-9.7f, 101f, 0.1f), new Quaternion(0, 30, 0, 30));

            // Open door;
            Timing.CallDelayed(0.5f, () =>
            {
                doorAirlock.AnimationController.Play("Open", "FIFIF");
            });

            Vector3 MaskaPos = pos;
            MaskaPos.y += 100f;

            // Spawn 035 chamber
            MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic("maska", MaskaPos, FuturePos);
            */
        }
        private void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
            {
                Cassie.Message("О̸̲̮͆паc█ос█ь! Внешний ██риме̸т̵р нару█ен неи███стным отря██м. Всему персоналу р███енд█ется укрыться в безо̸̙̌п̸̖͐̀асном м████.. <color=#ffffff00>h pitch_0.15 .g4 . .g4 . pitch_0.6 danger .g2 . pitch_0.7 external pitch_0.5 .g4 jam_1_1 board r was pitch_8 breached by  . pitch_0.6 .g4 . pitch_0.7 an unknown unit . all remaining personnel . pitch_0.6 .g6 . are advised to take shelter in a safe location </color>", false, false, true);
                CassieDestroyedLVL += 1;
            }
        }
        private void OnWaitingForPlayers()
        {
            //doorAirlock = null;
            //door035 = null;
            //room035 = null;
            Log.Info("Dropping collections and lists, kill coroutines..");
            try
            {
                Timing.KillCoroutines(WarheadHandler.ChangeColorsCoroutineHandle);

                Res.DiedWithSCP500R.Clear();
                Res.RoleDiedWithSCP500R.Clear();
                Res.StatusEffectBase.Clear();
                CassieDestroyedLVL = 0;

                ControlNR.Singleton.db.DropCollection("VIPPlayers");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }


            if (Server.FriendlyFire == true)
            {
                Server.FriendlyFire = false;
            }
        }
        private void OnEndingRound(EndingRoundEventArgs ev)
        {

            bool mtf = Player.List.Where(p => p.Role.Team == Team.FoundationForces && !CustomRole.Get((uint)1).Check(p)).Count() > 0;
            bool classd = Player.List.Where(p => p.Role == RoleTypeId.ClassD && !CustomRole.Get((uint)2).Check(p) && !CustomRole.Get((uint)1).Check(p)).Count() > 0;
            bool chaos = Player.List.Where(p => p.Role.Team == Team.ChaosInsurgency && p.Role != RoleTypeId.ClassD && !CustomRole.Get((uint)1).Check(p)).Count() > 0;
            bool scps = Player.List.Where(p => p.Role.Team == Team.SCPs || CustomRole.Get((uint)1).Check(p)).Count() > 0;

            // Никто не в живых..
            if (!mtf && !classd && !scps && !chaos) { ev.IsRoundEnded = true; Log.Debug("1"); }
            // ТОЛЬКО Класс д или ПХ в живых
            else if (!mtf && (chaos || classd) && !scps) { ev.IsRoundEnded = true; Log.Debug("2"); }
            // Если класс д или пх и мог живыф
            else if ((chaos || classd) && mtf && !scps) { ev.IsRoundEnded = false; Log.Debug("3"); }
            // МОГ И СЦП
            else if (mtf && scps) { ev.IsRoundEnded = false; Log.Debug("4"); }
            // Класс д или хаос и сцп
            else if ((classd || chaos) && scps) { ev.IsRoundEnded = false; Log.Debug("5"); }
            else
            {
                Log.Debug("=========НЕИЗВЕСТНЫЙ СЦЕНАРИЙ КОНЦА РАУНДА================");
                Log.Debug($"МТФ ЖИВЫ: {mtf} ");
                Log.Debug($"ХАОС ЖИВЫ: {chaos} ");
                Log.Debug($"КЛАССД ЖИВЫ: {classd} ");
                Log.Debug($"СЦП ЖИВЫ: {scps} ");
                Log.Debug($"ДОЛЖЕН РАУНД ЗАКОНЧИТСЯ?: ${ev.IsRoundEnded}");
                Log.Debug("=========НЕИЗВЕСТНЫЙ СЦЕНАРИЙ КОНЦА РАУНДА================");

            }

            if (ev.IsRoundEnded == true)
            {
                PlayerExtensions._hintQueue.Clear();

                Server.FriendlyFire = true;

                Cassie.Message("Огонь по своим включён.. <color=#ffffff00>h F F enabled .g1", true, false, true);
            }

            if (!isWarheadCassie1Minute && Round.ElapsedTime.Minutes >= 24)
            {
                isWarheadCassie1Minute = true;
                Cassie.Message("Детонация альфа-боеголовки будет запущена через 1<b></b> минуту.. <color=#ffffff00>h Alpha warhead detonation SEQUENCE .G1 will be started . in TMINUS . 1 minute");
            }

            if (!isWarheadStart && Round.ElapsedTime.Minutes >= 25)
            {
                isWarheadStart = true;

                Warhead.Start();
                Warhead.IsLocked = true;

                //Cassie.Message("Детонация альфа-боеголовки будет запущена через 1 минуту.. <color=#ffffff00>h Alpha warhead detonation will be started in t minute 1 minute ");
            }
        }
    }
}