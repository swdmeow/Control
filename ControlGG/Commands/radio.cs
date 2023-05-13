namespace Control.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;
    using Exiled.CustomRoles.API.Features;
    using System.Collections.Generic;
    using System.Linq;
    using Steamworks.Data;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class radio : ICommand
    {
        public string Command { get; } = "radio";

        public string[] Aliases { get; } = new string[] { };
        public static bool BooleanRadio = false;
        public string Description { get; } = "Команда для включения бесконечной рации..";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "CustomRole", PlayerPermissions.ForceclassWithoutRestrictions, "ControlGG", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            switch (arguments.At(0))
            {
                case "enable":
                    BooleanRadio = true;
                    response = "Бесконечная батарея - включена.";
                    break;
                case "disable":
                    BooleanRadio = false;
                    response = "Бесконечная батарея - выключена.";
                    break;
                default:
                    response = "Использование: radio (enable / disable)";
                    break;
            }
            return true;
        }
    }
}