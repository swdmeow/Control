namespace Control.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;
    using Exiled.CustomRoles.API.Features;
    using System.Collections.Generic;
    using System.Linq;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomRoleController : ICommand
    {
        public string Command { get; } = "CustomRole";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Команда для выдачи кастомных ролей.. [ID роли, ID игрока]";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Exiled.Permissions.Extensions.Permissions.CheckPermission(sender, "ControlNR.CustomRole"))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.At(0) == null || arguments.At(1) == null)
            {
                response = $"Ну где же... аргументы, ну где же аргументы?";
                return false;
            }

            if (!uint.TryParse(arguments.At(0), out uint x))
            {
                response = $"Первый аргумент не цифра..";
                return false;
            }

            CustomRole.TryGet(x, out CustomRole customRole);

            if (customRole == null)
            { 
                response = $"Айди роли не найден...";
                return false;
            }

            Player pl = Player.Get(arguments.At(1));

            if (pl == null)
            {
                response = $"Игрок не найден..";
                return false;
            }

            customRole.AddRole(pl);

            response = "Успешно!";
            return true;
        }
    }
}