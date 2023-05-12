using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System.Text;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;

namespace AdminTools.Commands.Cd
{
    using Exiled.Events.EventArgs.Scp079;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Cd : ICommand
    {

        public string Command { get; } = "cd-chambers";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "open, close, lock, unlock, destroy CD-chambers";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "cdChambers", PlayerPermissions.Broadcasting, "AdminTools", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Использование: cd-chambers (close / open / lock / unlock / destroy )";
                return false;
            }

            switch (arguments.At(0))
            {
                case "lock":
                    foreach (var door in Door.List)
                    {
                        if (door.Type == DoorType.PrisonDoor)
                        {
                            door.ChangeLock(DoorLockType.DecontLockdown);
                        }
                    }
                    response = "Двери заблокированы.";
                    break;
                case "unlock":
                    foreach (var door in Door.List)
                    {
                        if (door.Type == DoorType.PrisonDoor)
                        {
                            door.Unlock();
                        }
                    }
                    response = "Двери разблокированы.";
                    break;
                case "close":
                    foreach (var door in Door.List)
                    {
                        if (door.Type == DoorType.PrisonDoor)
                        {
                            door.IsOpen = false;

                            response = "Двери закрыты.";
                        }
                    }
                    response = "Двери закрыты.";
                    break;
                case "open":
                    foreach (var door in Door.List)
                    {
                        if (door.Type == DoorType.PrisonDoor)
                        {
                            door.IsOpen = true;
                        }
                    }
                    response = "Двери открыты.";
                    break;
                case "destroy":
                    foreach (var door in Door.List)
                    {
                        if (door.Type == DoorType.PrisonDoor)
                        {
                            door.BreakDoor(DoorDamageType.ServerCommand);
                        }
                    }
                    response = "Двери взорваны.";
                    break;

                default:
                    response = "...";
                    break;
            }
            return true; 
        }
    }
}
