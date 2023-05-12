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

namespace AdminTools.Commands.Tesla
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SCP096Engaged : ParentCommand
    {
        public SCP096Engaged() => LoadGeneratedCommands();

        public override string Command { get; } = "SCP096Engaged";

        public override string[] Aliases { get; } = new string[] { };


        public static bool SCP096EngagedBool = false;
        public override string Description { get; } = "Выключить или выключить бесконечный агр 096";

        public override void LoadGeneratedCommands() { }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "scp096Engaged", PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Использование: SCP096Engaged (enable / disable)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "enable":
                    SCP096EngagedBool = true;

                    response = "Тесла включена.";
                    break;
                case "disable":
                    SCP096EngagedBool = false;

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
