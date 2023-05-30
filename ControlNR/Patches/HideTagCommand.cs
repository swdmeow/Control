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
    internal class HideTagCommandPatch
    {

        [HarmonyPatch(typeof(GameConsoleCommandHandler), nameof(GameConsoleCommandHandler.))]
        public static class RemoteAdmin
        {
            static bool Prefix(string q, CommandSender sender)
            {
            }
        }
    }
}