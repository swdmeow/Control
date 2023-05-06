using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha.Patches
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

                bool Success = true;
                bool Allowed = true;
                Player player = Player.Get(sender);

                string[] args = q.Trim().Split(QueryProcessor.SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);

                if (player != null)
                {
                    if (player?.GroupName == "f")
                    {
                        if (args[0] == "give" || args[0] == "doortp" || args[0] == "forceclass" || args[0] == "god" || args[0] == "noclip" || args[0] == "bring" || args[0] == "bypass" || args[0] == "mute" || args[0] == "hp")
                        {
                            Success = false;
                            Allowed = false;

                            if (args[1] == $"{player.Id}.")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            if (args[1] == $"{player.Id}")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            sender.RaReply($"ControlAll#Не-не-не...", Success, true, string.Empty);
                            return Allowed;
                        }

                        if (args[0] == "pfx")
                        {
                            Success = false;
                            Allowed = false;

                            if (args[4] == $"{player.Id}.")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            if (args[4] == $"{player.Id}")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            sender.RaReply($"ControlAll#Не-не-не...", Success, true, string.Empty);
                            return Allowed;
                        }

                        if (args[0] == "size" || args[0] == "randomtp")
                        {
                            Success = false;
                            Allowed = false;

                            if (args[1] == $"{player.Id}.")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            if (args[1] == $"{player.Id}")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            sender.RaReply($"ControlAll#Не-не-не...", Success, true, string.Empty);
                            return Allowed;
                        }
                        if (args[0] == "ccolor")
                        {
                            Success = false;
                            Allowed = false;

                            if (float.TryParse(args[1], out float x) && float.TryParse(args[2], out float y) && float.TryParse(args[3], out float z))
                            {
                                if(x < 500 || y < 500 || z < 500)
                                {
                                    Success = true;
                                    Allowed = true;
                                    return Allowed;
                                }
                                if (x > -500 || y < -500 || z < -500)
                                {
                                    Success = true;
                                    Allowed = true;
                                    return Allowed;
                                }
                            }

                            sender.RaReply($"ControlAll#Не-не-не...", Success, true, string.Empty);
                            return Allowed;
                        }
                        if (args[0] == "randomtp")
                        {
                            Success = false;
                            Allowed = false;

                            if (args[1] == $"{player.Id}.")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            if (args[1] == $"{player.Id}")
                            {
                                Success = true;
                                Allowed = true;
                                return Allowed;
                            }
                            sender.RaReply($"ControlAll#Не-не-не...", Success, true, string.Empty);
                            return Allowed;
                        }
                    }
                }
                return Allowed;
            }
        }
    }
}