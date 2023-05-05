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

    internal sealed class ServerHandler
    {
        //public static SchematicObject doorAirlock = null;
        //public static SchematicObject door035 = null;
        //public static SchematicObject room035 = null;
        //public static Vector3 positionToTeleport;
        //public static Vector3 pos;
        public static bool RoomRotated = false;

        public static int CassieDestroyedLVL = 0;
        public static HashSet<Pickup> InteractingItemsElevator { get; } = new HashSet<Pickup>();
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
            var it = CustomItem.Get(3).Spawn(Room.List.ElementAt(new System.Random().Next(0, Room.List.Count())).transform.position + Vector3.up);
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
            if(ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
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
            Log.Info("Dropping collections and lists..");
            try
            {
                Res.DiedWithSCP500R.Clear();
                Res.RoleDiedWithSCP500R.Clear();

                ControlNR.Singleton.db.Execute("DROP COLLECTION VIPPlayers");
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
            bool human = false;
            bool scps = false;

            foreach (Player player in Player.List)
            {
                if (player == null)
                {
                    Log.Debug($"{nameof(OnEndingRound)}: Skipping a null player.");
                    continue;
                }

                if(CustomRole.Get(2).Check(player))
                {
                    continue;
                }

                if (CustomRole.Get(1).Check(player) || player.Role.Side == Side.Scp)
                {
                    Log.Debug($"{nameof(OnEndingRound)}: Found an SCP player.");
                    scps = true;
                }
                else if (player.Role.Side == Side.Mtf || player.Role == RoleTypeId.ClassD || player.Role.Side == Side.ChaosInsurgency)
                {
                    Log.Debug($"{nameof(OnEndingRound)}: Found a Human player.");
                    human = true;
                }

                if (scps && human)
                {
                    Log.Debug($"{nameof(OnEndingRound)}: Both humans and scps detected.");
                    break;
                }
            }

            if (human && scps)
            {
                ev.IsRoundEnded = false;
            } else
            {
                ev.IsRoundEnded = true;
            }

            if (ev.IsRoundEnded == true)
            {
                PlayerExtensions._hintQueue.Clear();

                Server.FriendlyFire = true;

                Cassie.Message("Огонь по своим включён.. <color=#ffffff00>h F F enabled .g1", false, false, true);
            }
        }
    }
}