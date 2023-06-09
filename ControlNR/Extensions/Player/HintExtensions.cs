using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Policy;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Interactables.Interobjects.DoorUtils;
using MapEditorReborn.Commands.UtilityCommands;
using MEC;
using PlayerRoles.Spectating;

namespace Control.Extensions
{
    public static class HintExtensions
    {
        public static CoroutineHandle? WriteHintCoroutineHandle = null;
        public static List<(Player, string, float)> XPHintQueue = new List<(Player, string, float)>();
        public static IEnumerator<float> WriteHint()
        {
            for (; ; )
            {

                foreach (Player pl in Player.List)
                {
                    if (pl.CurrentHint != null)
                    {
                        if (!pl.CurrentHint.Content.StartsWith("<b></b>"))
                        {
                            continue;
                        }
                    }

                    string Hint = "<b></b>";

                    if (!pl.IsAlive)
                    {
                        Hint += "<size=66%>";

                        string SpawningTeam = "";
                        if(Respawn.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
                        {
                            SpawningTeam = "<color=green>ПХ</color>";
                        } else
                        {
                            SpawningTeam = "<color=blue>МОГ</color>";
                        }
                        if (!Round.IsLobby) Hint += $"<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>{(Respawn.IsSpawning ? $"Вы заспавнитесь за {SpawningTeam} через" : "Вы заспавнитесь через")}:<br>{(Respawn.TimeUntilSpawnWave.Minutes.ToString().Length == 1 ? "0" + Respawn.TimeUntilSpawnWave.Minutes : Respawn.TimeUntilSpawnWave.Minutes)}:{(Respawn.TimeUntilSpawnWave.Seconds.ToString().Length == 1 ? "0" + Respawn.TimeUntilSpawnWave.Seconds : Respawn.TimeUntilSpawnWave.Seconds)}<br><b><color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b><br>";
                        else
                        {
                            Hint += $"<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><b><color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b><br>";
                        }

                        Hint += "</size>";
                        pl.ShowHint(Hint, 0.7f);
                    }

                    if (!pl.IsScp && !CustomRole.Get((uint)1).Check(pl) && pl.IsAlive)
                    {
                        try
                        {
                            bool HasXP = false;
                            if (XPHintQueue.Where(x => x.Item1 == pl) != null) HasXP = true;

                            Hint += "<size=66%>";
                            Hint += $"<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>{(HasXP ? XPHintQueue.Where(x => x.Item1 == pl).FirstOrDefault().Item2 : "")}<br>{((Round.IsLobby || !pl.IsAlive) ? "" : $"👥 {pl.CurrentSpectatingPlayers.Count()}")}<br>{(Round.IsLobby ? "" : $"{(Round.ElapsedTime.Hours.ToString().Length == 1 ? "0" + Round.ElapsedTime.Hours : Round.ElapsedTime.Hours)}:{(Round.ElapsedTime.Minutes.ToString().Length == 1 ? "0" + Round.ElapsedTime.Minutes : Round.ElapsedTime.Minutes)}:{(Round.ElapsedTime.Seconds.ToString().Length == 1 ? "0" + Round.ElapsedTime.Seconds : Round.ElapsedTime.Seconds)}")}<br><b><color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b><br>";

                            if (HasXP)
                            {
                                var x = XPHintQueue.Where(x => x.Item1 == pl).FirstOrDefault();

                                if (x.Item3 <= 0)
                                {
                                    XPHintQueue.Remove(x);
                                }
                            }

                            Hint += "</size>";
                            pl.ShowHint(Hint, 0.7f);
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }

                    if (pl.IsScp || CustomRole.Get((uint)1).Check(pl))
                    {
                        bool HasXP = false;
                        if (XPHintQueue.Where(x => x.Item1 == pl) != null) HasXP = true;

                        Hint += "<size=59%><pos=75%>";

                        Hint += $"{(HasXP ? XPHintQueue.Where(x => x.Item1 == pl).FirstOrDefault().Item2 : "")}<br><pos=75%>👥 {pl.CurrentSpectatingPlayers.Count()}<br><pos=75%><b><color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b></size><br>";

                        foreach (Player AddTarget in Player.List.Where(x => x.IsScp || CustomRole.Get((uint)1).Check(x)))
                        {
                            Hint += $"<size=75%><pos=75%><color=red>{(AddTarget.IsScp ? AddTarget.Role.Name : "SCP-035")} - {(AddTarget.HumeShieldStat.CurValue > 0 ? $"</color><color=#3A3679>{Math.Ceiling(AddTarget.Health + AddTarget.HumeShieldStat.CurValue)}</color>" : $"</color><color=red>{(AddTarget.Health == 0 ? Generator.List.Where(x => x.IsEngaged).Count() : Math.Ceiling(AddTarget.Health))}</color>")}/{(AddTarget.HumeShieldStat.CurValue > 0 ? $"</color><color=#3A3679>{Math.Ceiling(AddTarget.MaxHealth + AddTarget.HumeShieldStat.MaxValue)}</color>" : $"</color><color=red>{(AddTarget.Health == 0 ? Generator.List.Count() : Math.Ceiling(AddTarget.MaxHealth))}</color>")}</color></size><br>";
                        }

                        if (HasXP)
                        {
                            var x = XPHintQueue.Where(x => x.Item1 == pl).FirstOrDefault();

                            if (x.Item3 <= 0)
                            {
                                XPHintQueue.Remove(x);
                            }
                        }

                        Hint += "</size></color>";
                        pl.ShowHint(Hint, 0.7f);
                    }

                    if(XPHintQueue.Count != 0) XPHintQueue = XPHintQueue.Select(s => { s.Item3 -= 0.5f; return s; }).ToList();
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}