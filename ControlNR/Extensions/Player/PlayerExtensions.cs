using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Interactables.Interobjects.DoorUtils;
using MEC;

namespace Control.Extensions
{
    public static class PlayerExtensions
    {
        public static CoroutineHandle? HintCoroutineHandle = null;

        public static bool HasKeycardPermission(this Player player, KeycardPermissions permissions)
        {
            return player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & permissions) != 0);
        }
    }
}