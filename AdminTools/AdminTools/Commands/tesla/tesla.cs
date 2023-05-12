using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System.Text;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using Exiled.CustomRoles.API.Features;
using System.Linq;

namespace AdminTools.Commands.Tesla
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Tesla : ParentCommand
    {
        public Tesla() => LoadGeneratedCommands();

        public override string Command { get; } = "tesla";

        public override string[] Aliases { get; } = new string[] { };


        public static bool TeslaBoolean = true;
        public override string Description { get; } = "disable or enable tesla";

        public override void LoadGeneratedCommands() { }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.Command"))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Использование: tesla (enable / disable)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "enable":
                    TeslaBoolean = true;

                    response = "Тесла включена.";
                    break;
                case "disable":
                    TeslaBoolean = false;

                    response = "Тесла выключена.";
                    break;
                default:
                    response = "...";
                    break;
            }
            return true; 
        }
    }
}
