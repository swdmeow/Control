namespace Control.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;
    using Exiled.CustomRoles.API.Features;
    using System.Collections.Generic;
    using Exiled.CustomItems.API.Features;
    using System.Linq;
    using PlayerRoles;
    using CustomPlayerEffects;
    using XPSystem.API;
    using Exiled.Permissions.Extensions;
    using XPSystem;
    using XPSystem.API.Serialization;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class leaderboard : ICommand
    {
        public string Command { get; } = "leaderboard";
        public string[] Aliases { get; } = new string[] { "lb" };
        public string Description { get; } = "Люди, которые достигли в жизни больше чем ты..";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"\n{GetTopPlayers(10)}";
            return true;
        }

        private string GetTopPlayers(int amount)
        {
            var sorted = Main.Instance.db.GetCollection<PlayerLog>("Players").FindAll().OrderByDescending(o => o.XP).OrderByDescending(o => o.LVL);
            var players = sorted.Take(amount);
            string str = "";
            int index = 1;
            foreach (var log in players)
            {
                str += $"{index}. {log.Nickname} ({log.ID}): {log.LVL} уровень, {log.XP}/{Main.Instance.Config.XPPerLevel + (Main.Instance.Config.XPPerNewLevel * log.LVL)} опыта..\n";
                index++;
            }
            return str;
        }
    }
}