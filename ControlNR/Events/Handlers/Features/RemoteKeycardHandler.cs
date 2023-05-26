namespace Control.Handlers.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.Ammo;
    using MEC;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using System.Collections.Generic;
    using System.Linq;
    using CustomPlayerEffects;
    using Interactables.Interobjects.DoorUtils;
    using Control.Extensions;

    internal class RemoteKeycardHandler
    {
        public RemoteKeycardHandler()
        {
            PlayerEvent.InteractingDoor += OnInteractingDoor;
            PlayerEvent.UnlockingGenerator += OnUnlockingGenerator;
            PlayerEvent.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            PlayerEvent.InteractingLocker += OnInteractingLocker;
        }
        public void OnDisabled()
        {
            PlayerEvent.InteractingDoor -= OnInteractingDoor;
            PlayerEvent.UnlockingGenerator -= OnUnlockingGenerator;
            PlayerEvent.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            PlayerEvent.InteractingLocker -= OnInteractingLocker;
        }
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Door.RequiredPermissions.RequiredPermissions))
                ev.IsAllowed = true;
        }
        private void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(ev.Generator.Base._requiredPermission))
                ev.IsAllowed = true;
        }
        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!ev.IsAllowed && ev.Chamber != null && ev.Player.HasKeycardPermission(ev.Chamber.RequiredPermissions))
            {
                ev.IsAllowed = true;
            }
        }
        private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            // Disabled due exiled bug


            /*
            if (!ev.IsAllowed && ev.Player.HasKeycardPermission(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead))
                ev.IsAllowed = true;*/
        }
    }
}