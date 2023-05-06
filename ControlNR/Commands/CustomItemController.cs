namespace Control.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;
    using Exiled.CustomRoles.API.Features;
    using System.Collections.Generic;
    using Exiled.CustomItems.API.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomItemController : ICommand
    {
        public string Command { get; } = "CustomItem";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Команда для выдачи кастомных предметов.. [ID предмета, ID игрока]";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "CustomItem", PlayerPermissions.PlayersManagement, "control", false))
            {
                response = "У вас нет прав..";
                return false;
            }

            if (arguments.At(0) == null || arguments.At(1) == null)
            {
                response = $"Ну где же... аргументы, ну где же аргументы?";
                return false;
            }

            if (!uint.TryParse(arguments.At(0), out uint x))
            {
                response = $"Первый аргумент не цифра..";
                return false;
            }

            CustomItem.TryGet(x, out CustomItem customItem);

            if (customItem == null)
            {
                response = $"Айди предмета не найден...";
                return false;
            }

            Player pl = Player.Get(arguments.At(1));

            if (pl == null)
            {
                response = $"Игрок не найден..";
                return false;
            }

            customItem.Give(pl);

            response = "Успешно!";
            return true;
        }
    }
}