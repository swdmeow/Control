using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using Exiled.Permissions.Extensions;
using System.Text;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using Exiled.CustomRoles.API.Features;
using System.Linq;
using PlayerRoles;
using CommandSystem.Commands.RemoteAdmin;

namespace Control.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Cuff : ICommand
    {
        public string Command { get; } = "cuff";
        public static List<Player> IgnoredCuffPlayers = new List<Player>();
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Включить или выключить связывание для себя или же связать себя";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Exiled.Permissions.Extensions.Permissions.CheckPermission(sender, "ControlGG.Cuff"))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Использование: cuff (enable / disable / u)";
                return false;
            }

            Player pl = Player.Get(sender);

            switch (arguments.At(0))
            {
                case "enable":
                    if (IgnoredCuffPlayers.Contains(pl)) IgnoredCuffPlayers.Remove(pl);
                    response = "Связывание включено.";
                    Log.Info("Catch cuff command. Cuffing for player enable.");
                    break;
                case "disable":
                    if (!IgnoredCuffPlayers.Contains(pl)) IgnoredCuffPlayers.Add(pl);
                    response = "Связывание выключено.";
                    Log.Info("Catch cuff command. Cuffing for player disable.");
                    break;
                case "u":
                    if(!pl.IsCuffed)
                    {
                        pl.Cuffer = Player.List.ToList().RandomItem();
                    }
                    response = "Вы связаны.";
                    Log.Info("Catch cuff command. Cuffing player.");
                    break;
                default:
                    response = "Использование: cuff (enable / disable / u)";
                    break;
            }

            return true;
        }
    }
}
