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
    using Control.Extensions;
    using CustomPlayerEffects;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class size : ICommand
    {
        public string Command { get; } = "size";
        public string[] Aliases { get; } = new string[] {};
        public string Description { get; } = "Команда для изменения своего размера..";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Exiled.Permissions.Extensions.Permissions.CheckPermission(sender, "ControlNR.size"))
            {
                response = "У вас нет прав..";
                return false;
            }

            Player player = Player.Get(sender);

            if (player.GroupName.StartsWith("d1") || player.GroupName.StartsWith("d2"))
            {
                if (arguments.Count < 3)
                {
                    response = $"ControlNR#Использование:\nДля донатеров: [размер по оси X] [размер по оси Y] [размер по оси Z]";
                    return false;
                }

                if (!float.TryParse(arguments.At(0), out float x1) || !float.TryParse(arguments.At(1), out float y1) || !float.TryParse(arguments.At(2), out float z1))
                {
                    response = $"ControlNR#X, Y или Z - не цифра..";
                    return false;
                }

                if (x1 < 0.9f || y1 < 0.9f || z1 < 0.9f)
                {
                    response = $"ControlNR#Вы не можете изменить себе размер меньше 0.9..";
                    return false;
                }
                if (x1 > 1.1f || y1 > 1.1f || z1 > 1.1f)
                {
                    response = $"ControlNR#Вы не можете изменить себе размер больше 1.1..";
                    return false;
                }
                player.Scale = new UnityEngine.Vector3(x1, y1, z1);

                response = "Успешно..";
                return true;
            }

            if (arguments.Count < 4)
            {
                response = $"ControlNR#Использование:\nДля администрации: [ID игрока или *] [размер по оси X] [размер по оси Y] [размер по оси Z]";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float x) || !float.TryParse(arguments.At(2), out float y) || !float.TryParse(arguments.At(3), out float z))
            {
                response = $"ControlNR#X, Y или Z - не цифра..";
                return false;
            }

            switch (arguments.At(0))
            {
                case "all":
                case "*":
                    {
                        foreach (Player pl in Player.List)
                        {
                            pl.Scale = new UnityEngine.Vector3(x, y, z);
                        }

                        response = "Успешно..";
                        return true;
                    }
                default:
                    {
                        Player pl = Player.Get(arguments.At(0));

                        if (pl == null)
                        {
                            response = "ControlNR#Ошибка при получении игрока.";
                            return true;
                        }

                        pl.Scale = new UnityEngine.Vector3(x, y, z);

                        response = "Успешно..";
                        return true;
                    }
            }
        }
    }
}