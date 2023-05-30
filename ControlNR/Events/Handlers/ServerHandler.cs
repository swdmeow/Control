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

    internal sealed class ServerHandler
    {
        public static bool RoomRotated = false;

        private bool isWarheadCassie1Minute = false;
        private bool isWarheadStart = false;
        public static int CassieDestroyedLVL = 0;
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
                foreach (Player pl in Player.List.Where(x => x.IsScp))
                {
                    pl.ShowHint("<size=75%>Вы можете поменять свою игровую роль на другой SCP-объект<br>Используя команду .force [номер SCP]<br>Эта команда действует до 2-х минут раунда</size>", 30);
                }
            });
        }
        private void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency && ev.IsAllowed)
            {
                if(CassieDestroyedLVL >= 3)
                {
                    Cassie.Message("jam_040_9 pitch_0.43 .G1 . jam_020_9 .G3 . .G5 . pitch_0.3 .g3 . . . pitch_0.2 .g1", false, false, false);

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

                // Disable it when exiled add it lmao
                Scp049Role.TurnedPlayers.Clear();

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

                if (Warhead.IsInProgress == false) Warhead.Start();

                Timing.CallDelayed(0.1f, () => { Warhead.IsLocked = true; });

                //Cassie.Message("Детонация альфа-боеголовки будет запущена через 1 минуту.. <color=#ffffff00>h Alpha warhead detonation will be started in t minute 1 minute ");
            }

            bool mtf = Player.List.Where(p => (p.Role.Team == Team.FoundationForces || p.Role == RoleTypeId.Scientist) && !CustomRole.Get((uint)1).Check(p)).Count() > 0;
            bool chaos = Player.List.Where(p => (p.Role.Team == Team.ChaosInsurgency || p.Role == RoleTypeId.ClassD) && !CustomRole.Get((uint)1).Check(p) && !CustomRole.Get((uint)2).Check(p)).Count() > 0;
            bool scps = Player.List.Where(p => p.Role.Team == Team.SCPs || CustomRole.Get((uint)1).Check(p)).Count() > 0;

            if (!mtf && chaos && !scps) { ev.IsRoundEnded = true; Log.Info("1.2 (Chaos): Any team left only"); }
            else if (mtf && !chaos && !scps) { ev.IsRoundEnded = true; Log.Info("1.3 (MTF): Any team left only"); }
            else if (!mtf && !chaos && scps) { ev.IsRoundEnded = true; Log.Info("1.4 (SCPs): Any team left only"); }

            else if ((chaos || mtf) && scps) { ev.IsRoundEnded = false; Log.Debug("2: SCPs and MTF || CHAOS || CLASSD"); }
            else if (mtf && chaos) { ev.IsRoundEnded = false; Log.Debug("3: Chaos & MTF"); }
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

            Timing.CallDelayed(0.1f, () =>
            {
                Respawn.ForceWave(SpawnableTeamType.NineTailedFox, false);
            });

            try
            {
                API.Extensions.StopAudio();
            }
            catch (System.Exception) { }
        }
    }
}