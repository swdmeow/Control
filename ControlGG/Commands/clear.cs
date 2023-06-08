using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using Exiled.Permissions.Extensions;
using System.Text;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using Exiled.CustomRoles.API.Features;
using System.Linq;
using PlayerRoles;
using CommandSystem.Commands.RemoteAdmin;
using Control.CustomItems;

namespace Control.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Clear : ICommand
    {
        public string Command { get; } = "clear";
        public string[] Aliases { get; } = new string[] { };
        public string Description { get; } = "Очистить абсолютно все перменные.";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Exiled.Permissions.Extensions.Permissions.CheckPermission(sender, "ControlGG.Clear"))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Использование: cuff (enable / disable / u)";
                return false;
            }

            Res.StatusEffectBase.Clear();
            Res.DiedWithSCP500R.Clear();
            Res.RoleDiedWithSCP500R.Clear();
            Tesla.TeslaBoolean = false;
            Tesla.IgnoredTeams.Clear();
            Cuff.IgnoredCuffPlayers.Clear();
            GrenadeLauncher.CooldownIsEnable = false;
            radio.BooleanRadio = false;

            response = "Успешно.";
            return true;
        }
    }
}
