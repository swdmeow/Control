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
    using Control.Handlers.Events.API;
    using Сontrol;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class xp : ICommand
    {
        public string Command { get; } = "xp";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Команда для получения вашего опыта..";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender);

            if (!API.TryGetLog(ply.UserId, out var log))
            {
                response = "Ошибка.";
                return false;
            }

            log = ply.GetLog();

            response = $"\nУровень: {log.LVL}\nОпыт: {log.XP}/{ControlNR.Singleton.Config.XPSystem.XPPerLevel + (ControlNR.Singleton.Config.XPSystem.XPPerNewLevel * log.LVL)}";
            return true;
        }
    }
}