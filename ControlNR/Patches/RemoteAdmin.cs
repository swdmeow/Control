using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Сontrol;
using XPSystem.API.Serialization;
using MEC;
using PlayerRoles;
using CommandSystem;
using Org.BouncyCastle.Utilities;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Text.RegularExpressions;
using CommandSystem.Commands.RemoteAdmin;
using UnityStandardAssets.Effects;
using Exiled.Events.EventArgs.Player;
using Utf8Json.Formatters;
using Respawning;

namespace Control.Patches
{
    internal class RemoteAdminPatch
    {

        [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
        public static class RemoteAdmin
        {
            static bool Prefix(string q, CommandSender sender)
            {
                if (q.Contains("$"))
                {
                    return true;
                }
                if (sender.SenderId == "SERVER CONSOLE" && !q.Contains("REQUEST_DATA"))
                {
                    return true;
                }
                if (sender == null || q == null)
                {
                    return true;
                }
                if (sender == Server.Host.Sender)
                {
                    return true;
                }

                bool Success = true;
                bool Allowed = true;

                Player player = Player.Get(sender);

                string[] args = q.Trim().Split(QueryProcessor.SpaceArray, 512);

                if (player != null)
                {
                    if (player?.GroupName == "d1" || player?.GroupName == "d2")
                    {
                        Allowed = false;
                        var log = ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers")?.FindById(player.UserId);

                        if (log == null)
                        {
                            ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Insert(new PlayerLog()
                            {
                                ID = player.UserId,
                                cooldownRole = false,
                                cooldownItem = false,
                                cooldownCall = false,
                                cooldownVote = false,
                                ForcedToSCP = false,
                                GivedTimes = 0,
                                ForcedTimes = 0,
                                CallTimes = 0,
                            });

                            Success = false;
                            sender.RaReply($"ControlNR#Вы занесены в базу данных, повторите попытку..", Success, true, string.Empty);
                            return Allowed;
                        }

                        if (Round.IsStarted == false)
                        {
                            Success = false;
                            sender.RaReply($"ControlNR#Раунд не начался..", Success, true, string.Empty);
                            return Allowed;
                        }

                        Success = true;

                        if (args[0] == "size")
                        {
                            Allowed = false;
                            Success = false;

                            if (!float.TryParse(args[1], out float x) || !float.TryParse(args[2], out float y) || !float.TryParse(args[3], out float z))
                            {
                                sender.RaReply($"ControlNR#X, Y или Z - не цифра..", Success, true, string.Empty);
                                return Allowed;
                            }

                            if (x < 0.9f || y < 0.9f || z < 0.9f)
                            {
                                sender.RaReply($"ControlNR#Вы не можете изменить себе размер меньше 0.9..", Success, true, string.Empty);
                                return Allowed;
                            }
                            if (x > 1.1f || y > 1.1f || z > 1.1f)
                            {
                                sender.RaReply($"ControlNR#Вы не можете изменить себе размер больше 1.1..", Success, true, string.Empty);
                                return Allowed;
                            }
                            Success = true;

                            player.Scale = new UnityEngine.Vector3(x, y, z);
                            sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);

                            return Allowed;
                        }

                        if (player?.GroupName == "d1")
                        {
                            if (args[0] == "forceclass")
                            {
                                try
                                {
                                    if (log.ForcedTimes >= 1)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Вы уже использовали все свои попытки..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (args[2].ToLower().StartsWith("scp"))
                                    {
                                        if (Player.List.Where(x => x.IsScp)?.Count() == 0)
                                        {
                                            if (Player.List.Where(x => x.IsHuman).Count() >= 6)
                                            {
                                                Success = true;

                                                log.cooldownRole = true;
                                                log.ForcedTimes += 1;
                                                log.ForcedToSCP = true;

                                                ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                                sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                                return Allowed;
                                            }

                                            Success = false;
                                            sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                            return Allowed;
                                        }

                                        if (Player.List.Where(x => x.IsHuman).Count() / Player.List.Where(x => x.IsScp)?.Count() <= 10)
                                        {
                                            Success = true;

                                            log.cooldownRole = true;
                                            log.ForcedTimes += 1;
                                            log.ForcedToSCP = true;

                                            ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                            sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                        else
                                        {
                                            Success = false;
                                            sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                    }

                                    if (args[2].ToLower().StartsWith("ntf") || args[2].ToLower().StartsWith("chaos"))
                                    {
                                        if (Round.ElapsedTime.TotalMinutes <= 3)
                                        {
                                            Success = false;
                                            sender.RaReply($"ControlNR#За МОГ и ХАОС можно форснутся после 3-х минут раунда..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                    }

                                    if (args[2].ToLower().StartsWith("tutorial") || args[2].ToLower().StartsWith("overwatch"))
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Не-не..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    Enum.TryParse(args[2], true, out RoleTypeId role);

                                    log.cooldownRole = true;
                                    log.ForcedTimes += 1;

                                    ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                    player.Role.Set(role);

                                    return Allowed;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }
                            if (args[0] == "give")
                            {
                                try
                                {
                                    if (player.Role.Team == Team.SCPs)
                                    {
                                        Allowed = false;
                                        sender.RaReply($"ControlNR#Зачем..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (player.Items.Count >= 8)
                                    {
                                        Allowed = false;
                                        sender.RaReply($"ControlNR#У вас больше 8 предметов.. отмена..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    Success = true;

                                    if (Round.ElapsedTime.Minutes < 3)
                                    {
                                        // 0, 1, 2 , 3
                                        switch (args[2].Substring(0, 2))
                                        {
                                            case "0.":
                                            case "1.":
                                            case "2.":
                                            case "3.":
                                            case "15":
                                            case "14":
                                            case "17":
                                            case "34":
                                            case "33":
                                            case "35":
                                                {
                                                    break;
                                                }
                                            default:
                                                {
                                                    Success = false;
                                                    sender.RaReply($"ControlNR#Этот предмет нельзя выдать до 3-ех минут раунда..", Success, true, string.Empty);
                                                    return Allowed;
                                                }
                                        }
                                    }

                                    if (log.GivedTimes >= 2)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Вы использовали все свои попытки..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (log.cooldownItem == true)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Подождите 2 минуты перед тем, как выдавать себе вещи..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    log.cooldownItem = true;
                                    log.GivedTimes += 1;

                                    ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                    Timing.CallDelayed(120f, () =>
                                    {
                                        var ValueChange = ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers")?.FindById(player.UserId);
                                        ValueChange.cooldownItem = false;
                                        ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);
                                    });

                                    Enum.TryParse(args[2].Substring(0, 2).Replace(".", ""), true, out ItemType item);

                                    player.AddItem(item, 1);

                                    sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                    return Allowed;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }
                        }

                        if (player?.GroupName == "d2")
                        {
                            if (args[0] == "forceclass")
                            {
                                try
                                {
                                    if (log.ForcedTimes >= 3)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Вы уже использовали все свои попытки..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                    if(log.cooldownRole == true)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Ваша задержка на выдачу роли ещё не прошла..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (args[2].ToLower().StartsWith("scp"))
                                    {
                                        if (Player.List.Where(x => x.IsScp)?.Count() == 0)
                                        {
                                            if (Player.List.Where(x => x.IsHuman).Count() >= 6)
                                            {
                                                Success = true;

                                                log.cooldownRole = true;
                                                log.ForcedTimes += 1;
                                                log.ForcedToSCP = true;

                                                ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                                sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                                return Allowed;
                                            }

                                            Success = false;
                                            sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                            return Allowed;
                                        }

                                        if (Player.List.Where(x => x.IsHuman).Count() / Player.List.Where(x => x.IsScp)?.Count() <= 10)
                                        {
                                            Success = true;

                                            log.cooldownRole = true;
                                            log.ForcedTimes += 1;
                                            log.ForcedToSCP = true;

                                            ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                            sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                        else
                                        {
                                            Success = false;
                                            sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                    }

                                    if (args[2].ToLower().StartsWith("ntf") || args[2].ToLower().StartsWith("chaos"))
                                    {
                                        if (Round.ElapsedTime.TotalMinutes <= 3)
                                        {
                                            Success = false;
                                            sender.RaReply($"ControlNR#За МОГ и ХАОС можно форснутся после 3-х минут раунда..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                    }

                                    if (args[2].ToLower().StartsWith("tutorial") || args[2].ToLower().StartsWith("overwatch"))
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Не-не..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    Enum.TryParse(args[2], true, out RoleTypeId role);

                                    log.cooldownRole = true;
                                    log.ForcedTimes += 1;
                                    ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                    player.Role.Set(role);

                                    Timing.CallDelayed(120f, () =>
                                    {
                                        var ValueChange = ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers")?.FindById(player.UserId);
                                        ValueChange.cooldownRole = false;
                                        ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);
                                    });

                                    Allowed = false;
                                    return Allowed;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }
                            if (args[0] == "give")
                            {
                                try
                                {
                                    if (player.Role.Team == Team.SCPs)
                                    {
                                        Allowed = false;
                                        sender.RaReply($"ControlNR#Зачем..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (player.Items.Count >= 8)
                                    {
                                        Allowed = false;
                                        sender.RaReply($"ControlNR#У вас больше 8 предметов.. отмена..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    Success = true;

                                    if (Round.ElapsedTime.Minutes < 3)
                                    {
                                        // 0, 1, 2 , 3
                                        switch (args[2].Substring(0, 2))
                                        {
                                            case "0.":
                                            case "1.":
                                            case "2.":
                                            case "3.":
                                            case "15":
                                            case "14":
                                            case "17":
                                            case "34":
                                            case "33":
                                            case "35":
                                                {
                                                    break;
                                                }
                                            default:
                                                {
                                                    Success = false;
                                                    sender.RaReply($"ControlNR#Этот предмет нельзя выдать до 3-ех минут раунда..", Success, true, string.Empty);
                                                    return Allowed;
                                                }
                                        }
                                    }

                                    if (log.GivedTimes >= 5)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Вы использовали все свои попытки..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (log.cooldownItem == true)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Подождите 2 минуты перед тем, как выдавать себе вещи..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    log.cooldownItem = true;
                                    log.GivedTimes += 1;

                                    ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                    Timing.CallDelayed(120f, () =>
                                    {
                                        var ValueChange = ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers")?.FindById(player.UserId);
                                        ValueChange.cooldownItem = false;
                                        ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);
                                    });

                                    Enum.TryParse(args[2].Substring(0, 2).Replace(".", ""), true, out ItemType item);

                                    player.AddItem(item, 1);

                                    sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                    return Allowed;
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }
                            if (args[0] == "SERVER_EVENT")
                            {
                                try
                                {
                                    if (args[1] == "RESPAWN_MTF" || args[1] == "RESPAWN_CI")
                                    {
                                        if (log.CallTimes >= 2)
                                        {
                                            Success = false;
                                            sender.RaReply($"ControlNR#Вы уже использовали все свои попытки..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                        if (log.cooldownCall == true)
                                        {
                                            Success = false;
                                            sender.RaReply($"ControlNR#Ваша задержка на вызов ещё не прошла..", Success, true, string.Empty);
                                            return Allowed;
                                        }

                                        log.cooldownCall = true;
                                        log.CallTimes += 1;
                                        ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);

                                        Enum.TryParse(args[1], out SpawnableTeamType spawn);
                                        Respawn.ForceWave(spawn, true);

                                        Timing.CallDelayed(120f, () =>
                                        {
                                            var ValueChange = ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers")?.FindById(player.UserId);
                                            ValueChange.cooldownCall = false;
                                            ControlNR.Singleton.db.GetCollection<PlayerLog>("VIPPlayers").Update(log);
                                        });

                                        Allowed = false;
                                        return Allowed;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }
                            // VOTE TO EVENTS
                        }

                        sender.RaReply($"ControlNR#Вы не можете использовать эту команду..", Success, true, string.Empty);
                        return Allowed;
                    }
                }
                return Allowed;
            }
        }
    }
}