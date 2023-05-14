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
    using CustomPlayerEffects;
    using UnityEngine;
    using MEC;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MaxHealth : ICommand
    {
        public string Command { get; } = "MaxHealth";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Команда для установки максимального кол-ва хп..";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "MaxHealth", PlayerPermissions.PlayersManagement, "ControlNR", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (!int.TryParse(arguments.At(0), out int playerID))
            {
                response = $"Ты где цифру игрока потерял..";
                return false;
            }

            if (!int.TryParse(arguments.At(1), out int result))
            {
                response = $"Ты где цифру хп потерял..";
                return false;
            }

            Player player = Player.Get(playerID);

            player.MaxHealth = result;

            response = "... что ты пытаешься сделать?";
            return false;
        }
    }
}