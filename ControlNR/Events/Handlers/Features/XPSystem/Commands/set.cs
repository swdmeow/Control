using System;
using CommandSystem;
using Control.Handlers.Events.API.Serialization;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Сontrol;
using Control.Handlers.Events.API;

namespace XPSystem.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class set : ICommand
    {
        public string Command { get; } = "set";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = $"Установить уровень человеку.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("xps.set"))
            {
                response = "You don't have permission (xps.set) to use this command.";
                return false;
            }
            if (arguments.Count != 2)
            {
                response = "Usage : XPSystem set (UserId | in-game id) (int amount)";
                return false;
            }

            if (!API.TryGetLog(arguments.At(0), out var log))
            {
                response = "incorrect userid";
                return false;
            }

            if (int.TryParse(arguments.At(1), out int lvl))
            {
                log.LVL = lvl;
                log.XP = 0;
                log.UpdateLog();
                response = $"{arguments.At(0)}'s LVL is now {log.LVL}";
                return true;
            }

            response = $"Invalid amount of LVLs : {lvl}";
            return false;
        }
    }
}
