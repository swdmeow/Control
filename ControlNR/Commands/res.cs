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
    using UnityEngine;
    using MEC;
    using Exiled.API.Enums;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Res : ICommand
    {
        public string Command { get; } = "res";

        public static List<Player> DiedWithSCP500R = new List<Player>();
        public static List<RoleTypeId> RoleDiedWithSCP500R = new List<RoleTypeId>();
        public static List<StatusEffectBase> StatusEffectBase = new List<StatusEffectBase>();
        public string[] Aliases { get; } = new string[] { "respawn" };
        public string Description { get; } = "<b>Команда для возвраждения, если вы имели на тот момент SCP-500-R..</b>";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if(player.IsAlive)
            {
                response = "... что ты пытаешься сделать?";
                return false;
            }

            if (DiedWithSCP500R.Contains(player))
            {
                Player oldPlayer = DiedWithSCP500R.Find(x => x == player);
                RoleTypeId role = RoleDiedWithSCP500R.First();

                player.Role.Set(role, RoleSpawnFlags.None);

                if(Warhead.IsDetonated == true)
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.Position = Room.Get(Exiled.API.Enums.RoomType.Surface).transform.position + Vector3.up;
                    });
                }
                if(player.CurrentRoom.Type == RoomType.HczTestRoom)
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.Position = Room.Get(Exiled.API.Enums.RoomType.HczTestRoom).transform.position + new Vector3(0, 1, 5);
                    });
                }
                if (player.CurrentRoom.Type == RoomType.HczArmory)
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.Position = Room.Get(Exiled.API.Enums.RoomType.HczArmory).transform.position + Vector3.up;
                    });
                }
                if (player.CurrentRoom.Type == RoomType.Hcz106)
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        player.Position = Room.Get(Exiled.API.Enums.RoomType.Hcz106).transform.position + Vector3.up;
                    });
                }

                foreach (StatusEffectBase effect in StatusEffectBase)
                {
                    player.EnableEffect(effect, effect.Duration);
                }
                player.ShowHint("", 0.1f);
                DiedWithSCP500R.Clear();
                RoleDiedWithSCP500R.Clear();
                StatusEffectBase.Clear();
                response = "Успешно?..";
                return true;
            }

            response = "... что ты пытаешься сделать?";
            return false;
        }
    }
}