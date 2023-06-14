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
using Control.Handlers.Events.API.Serialization;
using Microsoft.SqlServer.Server;
using Control;
using GameCore;
using PlayerRoles.PlayableScps.HUDs;

namespace Control.Patches
{
    internal static class TagPatches
    {
        [HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRequestHideTag")]
        internal static class HidingRank
        {
            private static bool Prefix(CharacterClassManager __instance)
            {
                if (__instance is null || !__instance._commandRateLimit.CanExecute(true))
                    return false;

                Player pl = Player.Get(__instance.gameObject);

                if (!string.IsNullOrEmpty(__instance.SrvRoles.HiddenBadge))
                {
                    pl.SendConsoleMessage("Your rank is already hidden.", "yellow");
                    return false;
                }

                if(pl.IsNorthwoodStaff || pl.Group != null)
                {
                    pl.SendConsoleMessage($"\nDetected you as {(pl.IsNorthwoodStaff ? "Northwood Staff" : "player with group")}\nHiding rank..\n<b>You rank wiil be hided, but you level <color=red>(WITHOUT RANK)</color> will be shown</b>", "yellow");

                    __instance.SrvRoles.GlobalHidden = !__instance.SrvRoles.GlobalSet;
                    //__instance.SrvRoles.HiddenBadge = __instance.SrvRoles.MyText;
                    __instance.SrvRoles.NetworkGlobalBadge = null;
                    __instance.SrvRoles.SetText("");
                    __instance.SrvRoles.SetColor("");
                    __instance.SrvRoles.RefreshHiddenTag(); // base

                    pl.SendConsoleMessage("Rank hidden.", "green");

                    return false; // 
                } else
                {
                    pl.SendConsoleMessage("You can't hide you tag, because you don't have anything to hide without you level.", "red");
                }

                return false;
            }
        }
        [HarmonyPatch(typeof(CharacterClassManager), "UserCode_CmdRequestShowTag__Boolean")]
        internal static class ShowingTag
        {
            private static bool Prefix(CharacterClassManager __instance, bool global)
            {
                if (global)
                {
                    if (string.IsNullOrEmpty(__instance.SrvRoles.PrevBadge))
                    {
                        __instance.ConsolePrint("You don't have a global tag.", "magenta");
                    }

                    if ((string.IsNullOrEmpty(__instance.SrvRoles.MyText) || !__instance.SrvRoles.RemoteAdmin) && (((__instance.SrvRoles.GlobalBadgeType == 3 || __instance.SrvRoles.GlobalBadgeType == 4) && ConfigFile.ServerConfig.GetBool("block_gtag_banteam_badges") && !ServerStatic.PermissionsHandler.IsVerified) || (__instance.SrvRoles.GlobalBadgeType == 1 && ConfigFile.ServerConfig.GetBool("block_gtag_staff_badges")) || (__instance.SrvRoles.GlobalBadgeType == 2 && ConfigFile.ServerConfig.GetBool("block_gtag_management_badges") && !ServerStatic.PermissionsHandler.IsVerified) || (__instance.SrvRoles.GlobalBadgeType == 0 && ConfigFile.ServerConfig.GetBool("block_gtag_patreon_badges") && !ServerStatic.PermissionsHandler.IsVerified)))
                    {
                        __instance.ConsolePrint("You can't show this type of global badge on this server. Try joining server with global badges allowed.", "red");
                    }

                    __instance.SrvRoles.NetworkGlobalBadge = __instance.SrvRoles.PrevBadge;
                    __instance.SrvRoles.GlobalHidden = false;
                    __instance.SrvRoles.HiddenBadge = null;
                    __instance.SrvRoles.RpcResetFixed();
                    __instance.ConsolePrint("Global tag refreshed.", "green");
                }
                else
                {
                    __instance.SrvRoles.NetworkGlobalBadge = null;
                    __instance.SrvRoles.HiddenBadge = null;
                    __instance.SrvRoles.GlobalHidden = false;
                    __instance.SrvRoles.RpcResetFixed();
                    __instance.SrvRoles.RefreshPermissions(disp: true);
                    __instance.ConsolePrint("Local tag refreshed.", "green");
                }

                return false;
            }
        } 
    }
}