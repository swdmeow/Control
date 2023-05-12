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
using System.Linq;

namespace AdminTools.Commands.dRename
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class dRename : ParentCommand
    {
        public dRename() => LoadGeneratedCommands();

        public override string Command { get; } = "dRename";

        public override string[] Aliases { get; } = new string[] { };
        public override string Description { get; } = "rename";

        public override void LoadGeneratedCommands() { }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "dRename", PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Использование: dRename";
                return false;
            }
            var rand = new Random();
            foreach (Player pl in Player.List.Where(x => x.Role.Type == PlayerRoles.RoleTypeId.ClassD))
            {
                pl.DisplayNickname = $"D-{rand.Next(0001, 9999)}";
            }


            response = "ky";
            return true;
        }
    }
}
