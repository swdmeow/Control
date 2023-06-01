namespace Control.Handlers.Events
{
    using Exiled.API.Features;
    using MEC;
    using Exiled.CustomRoles.API.Features;
    using System.Linq;
    using Exiled.Events.EventArgs.Server;
    using PlayerRoles;
    using Сontrol;
    using Respawning;
    using Control.Commands;
    using Control.Extensions;
    using Control.CustomItems;
    using UnityEngine;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Pickups;
    using Mirror;
    using System.Data;
    using System.Collections.Generic;
    using MapEditorReborn.API.Features;
    using MapEditorReborn.API.Features.Objects;
    using Exiled.CustomItems.API.Features;
    //using Exiled.API.Enums;
    using Utils.NonAllocLINQ;

    internal sealed class ServerHandler
    {
        public static bool RoomRotated = false;

        private bool isWarheadCassie1Minute = false;
        private bool isWarheadStart = false;
        public static int CassieDestroyedLVL = 0;
        public static bool TickRoundEndDisable = false;

        public static SchematicObject Room035;


        public static List<RoleTypeId> RandomRoles = new List<RoleTypeId>()
        {
            RoleTypeId.ClassD,
            RoleTypeId.Scientist,
            RoleTypeId.FacilityGuard,
        };
        public static List<SpawnableTeamType> RandomSpawnableTeamType = new List<SpawnableTeamType>()
        {
            SpawnableTeamType.NineTailedFox,
            SpawnableTeamType.ChaosInsurgency,
        };
        public ServerHandler()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;
            ServerEvent.EndingRound += OnEndingRound;
            ServerEvent.RespawningTeam += OnRespawningTeam;
            ServerEvent.RoundStarted += OnStartingRound;
            ServerEvent.RoundEnded += OnRoundEnded;
        }
        public void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;
            ServerEvent.EndingRound -= OnEndingRound;
            ServerEvent.RespawningTeam -= OnRespawningTeam;
            ServerEvent.RoundStarted -= OnStartingRound;
            ServerEvent.RoundEnded -= OnRoundEnded;
        }
        private void OnStartingRound()
        {
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (Player pl in Player.List.Where(x => x.IsScp && x.Role.Type != RoleTypeId.Scp079))
                {
                    pl.ShowHint("<size=75%>Вы можете поменять свою игровую роль на другой SCP-объект<br>Используя команду .force [номер SCP]<br>Эта команда действует до 2-х минут раунда</size>", 30f);
                }

                // 079 remove 1

                Player player = Player.List.Where(x => x.Role == RoleTypeId.Scp079).FirstOrDefault();

                if (player != null)
                {
                    player.Role.Is(out Scp079Role scp079Role);

                    scp079Role.LoseSignal(30f);

                    player.ShowHint("<br>У вас есть 30 секунд на перевод на другой SCP-объект (через команду .force).\nВ ином случае вы будуете переведены в другой SCP/человека автоматически", 30f);

                    Timing.CallDelayed(30f, () =>
                    {
                        if (player.Role.Type == RoleTypeId.Scp079)
                        {
                            if (Player.List.Where(x => x.IsScp).Count() == 5)
                            {
                                player.Role.Set(RandomRoles.RandomItem(), Exiled.API.Enums.SpawnReason.LateJoin);
                            } else
                            {
                                if (Player.List.Where(x => x.Role == RoleTypeId.Scp173).FirstOrDefault() == null) player.Role.Set(RoleTypeId.Scp173, Exiled.API.Enums.SpawnReason.LateJoin);
                                else if (Player.List.Where(x => x.Role == RoleTypeId.Scp049).FirstOrDefault() == null) player.Role.Set(RoleTypeId.Scp049, Exiled.API.Enums.SpawnReason.LateJoin);
                                else if (Player.List.Where(x => x.Role == RoleTypeId.Scp939).FirstOrDefault() == null) player.Role.Set(RoleTypeId.Scp939, Exiled.API.Enums.SpawnReason.LateJoin);
                                else if (Player.List.Where(x => x.Role == RoleTypeId.Scp106).FirstOrDefault() == null) player.Role.Set(RoleTypeId.Scp106, Exiled.API.Enums.SpawnReason.LateJoin);
                                else if (Player.List.Where(x => x.Role == RoleTypeId.Scp096).FirstOrDefault() == null) player.Role.Set(RoleTypeId.Scp096, Exiled.API.Enums.SpawnReason.LateJoin);
                                else
                                {
                                    player.Role.Set(RandomRoles.RandomItem(), Exiled.API.Enums.SpawnReason.LateJoin);
                                }
                            }
                        }

                        foreach (Door door in Door.List.Where(x => x.Type == Exiled.API.Enums.DoorType.Scp079First || x.Type == Exiled.API.Enums.DoorType.Scp079Second))
                        {
                            door.IsOpen = true;
                        }

                        TickRoundEndDisable = true;
                    });
                } else
                {
                    foreach(Door door in Door.List.Where(x => x.Type == Exiled.API.Enums.DoorType.Scp079First || x.Type == Exiled.API.Enums.DoorType.Scp079Second))
                    {
                        door.IsOpen = true;
                    }
                }
            });
        }
        private void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency && ev.IsAllowed)
            {
                if (CassieDestroyedLVL >= 3)
                {
                    Cassie.Message("jam_040_9 pitch_0.43 .G1 . jam_020_9 .G3 . .G5 . pitch_0.3 .g3 . . . pitch_0.2 .g1", false, false, false);

                    return;
                }
                if (CassieDestroyedLVL == 2)
                {
                    Cassie.Message("pitch_0.09 jam_006_1 .G4 .G6 .G3", false, false, false);

                    CassieDestroyedLVL += 1;

                    return;
                }
                Cassie.Message("О̸̲̮͆паc█ос█ь! Внешний ██риме̸т̵р нару█ен неи███стным отря██м. Всему персоналу р███енд█ется укрыться в безо̸̙̌п̸̖͐̀асном м████.. <color=#ffffff00>h pitch_0.15 .g4 . .g4 . pitch_0.6 danger .g2 . pitch_0.7 external pitch_0.5 .g4 jam_1_1 board r was pitch_8 breached by  . pitch_0.6 .g4 . pitch_0.7 an unknown unit . all remaining personnel . pitch_0.6 .g6 . are advised to take shelter in a safe location </color>", false, false, true);
                CassieDestroyedLVL += 1;
            }
        }
        private void OnWaitingForPlayers()
        {
            Log.Info($"\nEnabling ControlNR.\nVersion: {ControlNR.Singleton.Version}\nAuthor: {ControlNR.Singleton.Author}");
            try
            {
                Timing.KillCoroutines(WarheadMusic.ChangeColorsCoroutineHandle);
                Timing.KillCoroutines(WarheadDecontamition.DecontamitionSequnse);

                if (ControlNR.Singleton.Config.FullRoundRestart)
                {
                    Log.Info("Setting NextRoundAction to full restart.");
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                }

                Round.IsLobbyLocked = false;
                Res.DiedWithSCP500R.Clear();
                Res.RoleDiedWithSCP500R.Clear();
                Res.StatusEffectBase.Clear();
                CassieDestroyedLVL = 0;
                GrenadeLauncher.CooldownIsEnable = false;
                isWarheadStart = false;
                isWarheadCassie1Minute = false;
                PlayerExtensions._hintQueue.Clear();

                ControlNR.Singleton.db.DropCollection("VIPPlayers");

                Room Lcz330 = Room.Get(Exiled.API.Enums.RoomType.Lcz330);

                Log.Info("Spawning schematic..");
                Room035 = MapEditorReborn.API.Features.ObjectSpawner.SpawnSchematic("maska", Lcz330.Position, Lcz330.transform.rotation);
                
                // YeahDoor
                if (Lcz330)
                {
                    NetworkBehaviour.Destroy(Lcz330.gameObject);
                }
            }
            catch (System.Exception ex)
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
            if (!isWarheadCassie1Minute && Round.ElapsedTime.Minutes >= 34 && !Warhead.IsDetonated)
            {
                isWarheadCassie1Minute = true;
                Cassie.Message("Неизбежная детонация альфа-боеголовки будет запущена через 1<b></b> минуту.. <color=#ffffff00>h Alpha warhead detonation SEQUENCE .G1 will be started . in TMINUS . 1 minute", true, false, true);
            }

            if (!isWarheadStart && Round.ElapsedTime.Minutes >= 35 && !Warhead.IsDetonated)
            {
                isWarheadStart = true;

                if (Warhead.IsInProgress == false)
                {
                    Warhead.Controller.InstantPrepare();
                    Warhead.Controller.StartDetonation(true, false, (Player.List.Count() == 0 ? Server.Host.ReferenceHub : Player.List.ToList().RandomItem().ReferenceHub));
                }
                Warhead.IsLocked = true; 

                //Cassie.Message("Детонация альфа-боеголовки будт запущена через 1 минуту.. <color=#ffffff00>h Alpha warhead detonation will be started in t minute 1 minute ");
            }

            if (TickRoundEndDisable != true)
            {
                bool mtf = Player.List.Where(p => (p.Role.Team == Team.FoundationForces || p.Role == RoleTypeId.Scientist) && !CustomRole.Get((uint)1).Check(p)).Count() > 0;
                bool chaos = Player.List.Where(p => (p.Role.Team == Team.ChaosInsurgency || p.Role == RoleTypeId.ClassD) && !CustomRole.Get((uint)1).Check(p) && !CustomRole.Get((uint)2).Check(p)).Count() > 0;
                bool scps = Player.List.Where(p => p.Role.Team == Team.SCPs || CustomRole.Get((uint)1).Check(p)).Count() > 0;

                if (!mtf && chaos && !scps) { ev.IsRoundEnded = true; Log.Info("1.2 (Chaos): Any team left only"); }
                else if (mtf && !chaos && !scps) { ev.IsRoundEnded = true; Log.Info("1.3 (MTF): Any team left only"); }
                else if (!mtf && !chaos && scps) { ev.IsRoundEnded = true; Log.Info("1.4 (SCPs): Any team left only"); }

                else if ((chaos || mtf) && scps) { ev.IsRoundEnded = false; Log.Debug("2: SCPs and MTF || CHAOS || CLASSD"); }
                else if (mtf && chaos) { ev.IsRoundEnded = false; Log.Debug("3: Chaos & MTF"); }
            }

            if(TickRoundEndDisable == true) TickRoundEndDisable = false;
        }
        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Server.FriendlyFire = true;

            Cassie.Clear();
            Cassie.Message("Огонь по своим включён.. <color=#ffffff00>h F F enabled .g1", true, false, true);

            foreach (Pickup pickup in Pickup.List)
            {
                pickup.Destroy();
            }
            foreach (Ragdoll ragdoll in Ragdoll.List)
            {
                ragdoll.Destroy();
            }

            foreach(Player pl in Player.List)
            {
                pl.IsGodModeEnabled = false;
            }
            
            Timing.CallDelayed(0.1f, () =>
            {
                Respawn.ForceWave(RandomSpawnableTeamType.RandomItem(), false);
            });

            try
            {
                Control.API.Extensions.StopAudio();
            }
            catch (System.Exception) { }
        }
    }
}