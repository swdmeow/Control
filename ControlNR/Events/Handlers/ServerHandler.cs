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
            Timing.CallDelayed(0.1f, () =>
            {
                foreach (Player pl in Player.List.Where(x => x.IsScp))
                {
                    pl.ShowHint("<size=75%>Вы можете поменять свою игровую роль на другой SCP-объект<br>Используя команду .force [номер SCP]<br>Эта команда действует до 2-х минут раунда</size>", 30);
                }
            });

            if (HintExtensions.WriteHintCoroutineHandle == null || !HintExtensions.WriteHintCoroutineHandle.Value.IsValid || !HintExtensions.WriteHintCoroutineHandle.Value.IsRunning)
                HintExtensions.WriteHintCoroutineHandle = Timing.RunCoroutine(HintExtensions.WriteHint());

        }
        private void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency && ev.IsAllowed)
            {
                Cassie.Message("О̸̲̮͆паc█ос█ь! Внешний ██риме̸т̵р нару█ен неи███стным отря██м. Всему персоналу р███енд█ется укрыться в безо̸̙̌п̸̖͐̀асном м████.. <color=#ffffff00>h pitch_0.15 .g4 . .g4 . pitch_0.6 danger .g2 . pitch_0.7 external pitch_0.5 .g4 jam_1_1 board r was pitch_8 breached by  . pitch_0.6 .g4 . pitch_0.7 an unknown unit . all remaining personnel . pitch_0.6 .g6 . are advised to take shelter in a safe location </color>", false, false, true);
                CassieDestroyedLVL += 1;
            }
        }
        private void OnWaitingForPlayers()
        {
            Log.Info("Dropping collections and lists, kill coroutines, disable LobbyLock");
            try
            {
                Timing.KillCoroutines(WarheadHandler.ChangeColorsCoroutineHandle);
                Timing.KillCoroutines(WarheadHandler.DecontamitionSequnse);

                Round.IsLobbyLocked = false;
                Res.DiedWithSCP500R.Clear();
                Res.RoleDiedWithSCP500R.Clear();
                Res.StatusEffectBase.Clear();
                CassieDestroyedLVL = 0;
                GrenadeLauncher.CooldownIsEnable = false;
                isWarheadStart = false;
                isWarheadCassie1Minute = false;

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

                Warhead.IsLocked = true;

                //Cassie.Message("Детонация альфа-боеголовки будет запущена через 1 минуту.. <color=#ffffff00>h Alpha warhead detonation will be started in t minute 1 minute ");
            }

            bool mtf = Player.List.Where(p => p.Role.Team == Team.FoundationForces || p.Role == RoleTypeId.Scientist && !CustomRole.Get((uint)1).Check(p)).Count() > 0;
            bool chaos = Player.List.Where(p => p.Role.Team == Team.ChaosInsurgency || p.Role == RoleTypeId.ClassD && !CustomRole.Get((uint)1).Check(p) && !CustomRole.Get((uint)2).Check(p)).Count() > 0;
            bool scps = Player.List.Where(p => p.Role.Team == Team.SCPs || CustomRole.Get((uint)1).Check(p)).Count() > 0;

            // Если кто-то один остался в живых..
            if (!mtf && chaos && !scps) { ev.IsRoundEnded = true; Log.Info("1.2: Any team left only"); }
            else if (mtf && !chaos && !scps) { ev.IsRoundEnded = true; Log.Info("1.3: Any team left only"); }
            else if (!mtf && !chaos && scps) { ev.IsRoundEnded = true; Log.Info("1.4: Any team left only"); }

            // Only one team and SCPs
            else if ((chaos || mtf) && scps) { ev.IsRoundEnded = false; Log.Debug("2: SCPs and MTF || CHAOS || CLASSD"); }

            // 
            else if (mtf && chaos) { ev.IsRoundEnded = false; Log.Debug("3: Chaos & MTF"); }

            else
            {
                Log.Info("=========НЕИЗВЕСТНЫЙ СЦЕНАРИЙ КОНЦА РАУНДА================");
                Log.Info($"МТФ ЖИВЫ: {mtf} ");
                Log.Info($"ХАОС ЖИВЫ: {chaos} ");
                Log.Info($"СЦП ЖИВЫ: {scps} ");
                Log.Info($"ДОЛЖЕН РАУНД ЗАКОНЧИТСЯ?: ${ev.IsRoundEnded}");
                Log.Info("=========НЕИЗВЕСТНЫЙ СЦЕНАРИЙ КОНЦА РАУНДА================");
            }

            if (ev.IsRoundEnded == true)
            {
                PlayerExtensions._hintQueue.Clear();

                Server.FriendlyFire = true;

                Cassie.Message("Огонь по своим включён.. <color=#ffffff00>h F F enabled .g1", true, false, true);

                if(API.Extensions.Dummies.Count() > 0) API.Extensions.StopAudio();
            }
        }
    }
}