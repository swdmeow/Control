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

namespace Control.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Tesla : ICommand
    {
        public string Command { get; } = "tesla";

        public string[] Aliases { get; } = new string[] { };

        public static bool TeslaBoolean = true;
        public string Description { get; } = "Disable or Enable tesla for teams, peoples";

        public static List<Team> IgnoredTeams = new List<Team>();
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Exiled.Permissions.Extensions.Permissions.CheckPermission(sender, "ControlGG.Tesla"))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Использование: tesla (enable / disable / team / user / clear) [Название команды]";
                return false;
            }

            switch (arguments.At(0))
            {
                case "enable":
                    TeslaBoolean = true;
                    Log.Info("Catch tesla command. Enabling..");
                    response = "Тесла включена.";
                    break;
                case "disable":
                    TeslaBoolean = false;
                    Log.Info("Catch tesla command. Disabling..");
                    response = "Тесла выключена.";
                    break;
                case "team":
                    {
                        Log.Info("Catch tesla command for team. Checking..");

                        string[] args = arguments.ToString().Trim().Split(QueryProcessor.SpaceArray, 512);

                        if (args[1] == null)
                        {
                            Log.Info("Catch tesla command for team. Cancel arguments(1) is null..");
                            response = $"Список команд:\nSCPs - SCP-объекты\nFoundationForces - МОГ, охрана\nChaosInsurgency - ПХ\nScientists - научный персонал\nClassD - класс-д\nOtherAlive - туториал";

                            return false;
                        }

                        if(Enum.TryParse(arguments.At(1), out Team team) == false)
                        {
                            Log.Info("Catch tesla command for team. Cancel. Team is null");
                            response = "Ошибка. Попробуйте ввести эту команду без аргументов чтобы увидеть весь список команд.";
                            return false;
                        }

                        if(team == Team.Dead)
                        {
                            Log.Info("Catch tesla command for team. Cancel, team is Dead");
                            response = "Блять ты че нахуй делаешь";
                            return false;
                        }

                        if(IgnoredTeams.Contains(team))
                        {
                            Log.Info("Catch tesla command for team. Removing it from list..");
                            IgnoredTeams.Remove(team);
                            response = "Команда успешна убрана из списка игнорируемых команд.";
                            return true;
                        } else
                        {
                            Log.Info("Catch tesla command for team. Add it to list..");
                            IgnoredTeams.Add(team);
                            response = "Команда успешна добавлена в спискок игнорируемых команд.";
                            return true;
                        }
                    }
                case "clear":
                    {
                        Log.Info("Catch clear tesla command for team...");
                        response = "Успешно..";
                        IgnoredTeams.Clear();
                        break;
                    }
                default:
                    response = "Где аргументы..";
                    break;
            }
            return true; 
        }
    }
}
