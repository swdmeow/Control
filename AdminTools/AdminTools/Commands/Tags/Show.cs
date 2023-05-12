﻿using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AdminTools.Commands.Tags
{
    public class Show : ICommand
    {
        public string Command { get; } = "show";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Shows staff tags on the server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.tags"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: tags show";
                return false;
            }

            foreach (Player player in Player.List)
                if (player.ReferenceHub.serverRoles.RemoteAdmin && !player.ReferenceHub.serverRoles.RaEverywhere)
                    player.BadgeHidden = false;

            response = "All staff tags are now visible";
            return true;
        }
    }
}
