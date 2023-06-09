using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Map;
using Interactables.Interobjects.DoorUtils;
using MapEditorReborn.Commands.UtilityCommands;
using MEC;

namespace Control.Extensions
{
    public static class ClearMapExtensions
    {
        public static CoroutineHandle? ClearMapCoroutineHandle = null;
        public static IEnumerator<float> ClearMap()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(360.6f);

                if (Round.IsLobby) continue;

                if (Ragdoll.List.Count() > 25)
                {
                    foreach (Ragdoll ragdoll in Ragdoll.List)
                    {
                        ragdoll.Destroy();
                    }
                }

                if (Pickup.List.Count() > 350)
                {
                    foreach (Pickup pickup in Pickup.List)
                    {
                        if (pickup.Room.Type == RoomType.LczArmory
                            || pickup.Room.Type == RoomType.HczArmory
                            || pickup.Room.Type == RoomType.HczHid
                            || pickup.Room.Type == RoomType.Lcz914) continue;

                        pickup.Destroy();
                    }
                }
            }
        }
    }
}