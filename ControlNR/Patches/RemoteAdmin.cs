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
using Control.API.Serialization;

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
                    if (args[0].ToLower() == "hidetag")
                    {
                        sender.Respond("Зачем..");
                        return false;
                    }

                    if (player.GroupName.StartsWith("d1") || player.GroupName.StartsWith("d2"))
                    {
                        string UserID = player.UserId;

                        Allowed = false;
                        var log = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(player.UserId);

                        if (log == null)
                        {
                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Insert(new VIPLog()
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

                        if (args[0].ToLower() == "size")
                        {
                            Allowed = true;
                            return Allowed;
                        }

                        if (player.GroupName.StartsWith("d1"))
                        {
                            if (args[0] == "forceclass")
                            {
                                if (log.ForcedTimes >= 1)
                                {
                                    Success = false;
                                    sender.RaReply($"ControlNR#Вы уже использовали все свои попытки..", Success, true, string.Empty);
                                    return Allowed;
                                }

                                Enum.TryParse(args[2], true, out RoleTypeId role);

                                if (role.GetTeam() == Team.SCPs)
                                {
                                    if (log.ForcedToSCP == true)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Вы уже форсались за SCP..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                    if (Player.List.Where(x => x.IsScp)?.Count() == 0)
                                    {
                                        if (Player.List.Where(x => x.IsHuman).Count() >= 6)
                                        {
                                            Success = true;

                                            log.cooldownRole = true;
                                            log.ForcedTimes += 1;
                                            log.ForcedToSCP = true;

                                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                            sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                            return Allowed;
                                        }

                                        Success = false;
                                        sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (Player.List.Where(x => x.IsHuman).Count() / Player.List.Where(x => x.IsScp)?.Count() <= 10)
                                    {
                                        if (Player.List.Where(x => x.Role == role).FirstOrDefault() != null)
                                        {
                                            sender.RaReply($"ControlNR#Данный SCP уже есть в раунде..", Success, true, string.Empty);
                                            return false;
                                        }

                                        Success = true;

                                        log.cooldownRole = true;
                                        log.ForcedTimes += 1;
                                        log.ForcedToSCP = true;

                                        ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                        player.Role.Set(role);

                                        sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);

                                        Timing.CallDelayed(120f, () =>
                                        {
                                            var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                            ValueChange.cooldownRole = false;
                                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                        });

                                        return Allowed;
                                    }
                                    else
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                }


                                if (role.GetTeam() == Team.ChaosInsurgency && role != RoleTypeId.ClassD || role.GetTeam() == Team.FoundationForces && role != RoleTypeId.Scientist)
                                {
                                    if (Round.ElapsedTime.TotalMinutes <= 3)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#За МОГ и ХАОС можно форснутся после 3-х минут раунда..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                }

                                if (role == RoleTypeId.Tutorial || role == RoleTypeId.Overwatch || role == RoleTypeId.Filmmaker || role == RoleTypeId.None || role == RoleTypeId.CustomRole)
                                {
                                    Success = false;
                                    sender.RaReply($"ControlNR#Не-не..", Success, true, string.Empty);
                                    return Allowed;
                                }

                                log.cooldownRole = true;
                                log.ForcedTimes += 1;
                                ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                player.Role.Set(role);

                                Timing.CallDelayed(120f, () =>
                                {
                                    var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                    ValueChange.cooldownRole = false;
                                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                });

                                Allowed = false;
                                return Allowed;
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

                                    //Success = true;

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

                                    Enum.TryParse(args[2].Substring(0, 2).Replace(".", ""), true, out ItemType item);

                                    if (item == ItemType.GunCom45 || item == ItemType.Jailbird)
                                    {
                                        Log.Info($"Catch give event (VIP+). Item {args[2].Substring(0, 2)}. Allowed = false");
                                        Success = false;
                                        Allowed = false;
                                        sender.RaReply($"ControlNR#Этот предмет нельзя выдать..", Success, true, string.Empty);
                                        return Allowed;
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
                                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                    Timing.CallDelayed(120f, () =>
                                    {
                                        try
                                        {
                                            var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                            ValueChange.cooldownItem = false;

                                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error(ex);
                                        }
                                    });

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

                        if (player.GroupName.StartsWith("d2"))
                        {
                            if (args[0] == "forceclass")
                            {
                                if (log.ForcedTimes >= 3)
                                {
                                    Success = false;
                                    sender.RaReply($"ControlNR#Вы уже использовали все свои попытки..", Success, true, string.Empty);
                                    return Allowed;
                                }
                                if (log.cooldownRole == true)
                                {
                                    Success = false;
                                    sender.RaReply($"ControlNR#Ваша задержка на выдачу роли ещё не прошла..", Success, true, string.Empty);
                                    return Allowed;
                                }

                                Enum.TryParse(args[2], true, out RoleTypeId role);

                                if (role.GetTeam() == Team.SCPs)
                                {
                                    if (log.ForcedToSCP == true)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Вы уже форсались за SCP..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                    if (Player.List.Where(x => x.IsScp)?.Count() == 0)
                                    {
                                        if (Player.List.Where(x => x.IsHuman).Count() >= 6)
                                        {
                                            Success = true;

                                            log.cooldownRole = true;
                                            log.ForcedTimes += 1;
                                            log.ForcedToSCP = true;



                                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                            sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);
                                            return Allowed;
                                        }

                                        Success = false;
                                        sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                        return Allowed;
                                    }

                                    if (Player.List.Where(x => x.IsHuman).Count() / Player.List.Where(x => x.IsScp)?.Count() <= 10)
                                    {
                                        if (Player.List.Where(x => x.Role == role).FirstOrDefault() != null)
                                        {
                                            sender.RaReply($"ControlNR#Данный SCP уже есть в раунде..", Success, true, string.Empty);
                                            return false;
                                        }

                                        Success = true;

                                        log.cooldownRole = true;
                                        log.ForcedTimes += 1;
                                        log.ForcedToSCP = true;

                                        ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                        player.Role.Set(role);

                                        sender.RaReply($"ControlNR#Успешно..", Success, true, string.Empty);

                                        Timing.CallDelayed(120f, () =>
                                        {
                                            var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                            ValueChange.cooldownRole = false;
                                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                        });

                                        return Allowed;
                                    }
                                    else
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#Кол-во игроков для спавна SCP меньше необходимого..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                }


                                if (role.GetTeam() == Team.ChaosInsurgency && role != RoleTypeId.ClassD || role.GetTeam() == Team.FoundationForces && role != RoleTypeId.Scientist)
                                {
                                    if (Round.ElapsedTime.TotalMinutes <= 3)
                                    {
                                        Success = false;
                                        sender.RaReply($"ControlNR#За МОГ и ХАОС можно форснутся после 3-х минут раунда..", Success, true, string.Empty);
                                        return Allowed;
                                    }
                                }

                                if (role == RoleTypeId.Tutorial || role == RoleTypeId.Overwatch || role == RoleTypeId.Filmmaker || role == RoleTypeId.None || role == RoleTypeId.CustomRole)
                                {
                                    Success = false;
                                    sender.RaReply($"ControlNR#Не-не..", Success, true, string.Empty);
                                    return Allowed;
                                }

                                log.cooldownRole = true;
                                log.ForcedTimes += 1;
                                ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                player.Role.Set(role);

                                Timing.CallDelayed(120f, () =>
                                {
                                    var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                    ValueChange.cooldownRole = false;
                                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                });

                                Allowed = false;
                                return Allowed;
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
                                        Log.Info(args[2].Substring(0, 2));
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
                                            case "35.":
                                                {
                                                    Log.Info($"Catch give event (VIP+). Item {args[2].Substring(0, 2)}. Allowed = true");
                                                    break;
                                                }
                                            default:
                                                {
                                                    Log.Info($"Catch give event (VIP+). Item {args[2].Substring(0, 2)}. Allowed = false");
                                                    Success = false;
                                                    sender.RaReply($"ControlNR#Этот предмет нельзя выдать до 3-ех минут раунда..", Success, true, string.Empty);
                                                    return Allowed;
                                                }
                                        }
                                    }

                                    Enum.TryParse(args[2].Substring(0, 2).Replace(".", ""), true, out ItemType item);

                                    if(item == ItemType.GunCom45 || item == ItemType.Jailbird)
                                    {
                                        Log.Info($"Catch give event (VIP+). Item {args[2].Substring(0, 2)}. Allowed = false");
                                        Success = false;
                                        Allowed = false;
                                        sender.RaReply($"ControlNR#Этот предмет нельзя выдать..", Success, true, string.Empty);
                                        return Allowed;
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

                                    ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                    Timing.CallDelayed(120f, () =>
                                    {
                                        var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                        ValueChange.cooldownItem = false;
                                        ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                    });

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
                                            Allowed = false;
                                            sender.RaReply($"ControlNR#Вы уже использовали все свои попытки..", Success, true, string.Empty);
                                            return Allowed;
                                        }
                                        if (log.cooldownCall == true)
                                        {
                                            Success = false;
                                            Allowed = false;
                                            sender.RaReply($"ControlNR#Ваша задержка на вызов ещё не прошла..", Success, true, string.Empty);
                                            return Allowed;
                                        }

                                        log.cooldownCall = true;
                                        log.CallTimes += 1;
                                        ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(log);

                                        Timing.CallDelayed(120f, () =>
                                        {
                                            var ValueChange = ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers")?.FindById(UserID);
                                            ValueChange.cooldownCall = false;
                                            ControlNR.Singleton.db.GetCollection<VIPLog>("VIPPlayers").Update(ValueChange);
                                        });

                                        // Only one allow in this text file lmao
                                        Allowed = true;
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

                        Success = false;
                        Allowed = false;
                        sender.RaReply($"ControlNR#Вы не можете использовать эту команду..", Success, true, string.Empty);
                        return Allowed;
                    }
                }
                return Allowed;
            }
        }
    }
}