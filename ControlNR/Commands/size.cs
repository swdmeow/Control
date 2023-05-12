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
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "size", PlayerPermissions.Broadcasting, "ControlNR", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            Player player = Player.Get(sender);

            if (!float.TryParse(arguments.At(0), out float x) || !float.TryParse(arguments.At(1), out float y) || !float.TryParse(arguments.At(2), out float z))
            {
                response = $"ControlNR#X, Y или Z - не цифра..";
                return false;
            }

            player.Scale = new UnityEngine.Vector3(x, y, z);

            response = "Успешно..";
            return true;
        }
    }
}