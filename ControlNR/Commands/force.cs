namespace Control.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;
    using Exiled.CustomRoles.API.Features;
    using System.Collections.Generic;
    using Exiled.CustomItems.API.Features;
    using Control.CustomItems;
    using System.Linq;
    using PlayerRoles;
    using Exiled.API.Features.Roles;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Force : ICommand
    {
        public string Command { get; } = "force";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Команда для перевода в другой класс SCP..";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.IsScp)
            {
                response = "Вы не SCP объект..";
                return false;
            }

            if(Round.ElapsedTime.TotalSeconds >= 120)
            {
                response = "Прошло более 120 секунд после начала раунда..";
                return false;
            }

            if(arguments.At(0) == null || !int.TryParse(arguments.At(0), out int _x))
            {
                response = "Использование .force [номер SCP]";
                return false;
            }

            RoleTypeId role;

            switch(arguments.At(0))
            {
                case "173":
                    role = RoleTypeId.Scp173;
                    break;
                case "049":
                    role = RoleTypeId.Scp049;
                    break;
                case "096":
                    role = RoleTypeId.Scp096;
                    break;
                case "939":
                    role = RoleTypeId.Scp939;
                    break;
                case "106":
                    role = RoleTypeId.Scp106;
                    break;
                case "079":
                    role = RoleTypeId.Scp079;
                    break;
                default:
                    response = "Неверный номер SCP объекта...";
                    return true;
            }

            if (Player.List.Where(x => x.Role == role).FirstOrDefault() != null)
            {
                response = "Данный SCP уже есть в раунде..";
                return false;
            }

            player.Role.Set(role, Exiled.API.Enums.SpawnReason.ForceClass, RoleSpawnFlags.All);


            response = "Успешно!";
            return true;
        }
    }
}