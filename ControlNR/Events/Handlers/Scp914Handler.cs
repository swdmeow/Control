namespace Control.Handlers.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Сontrol;
    using MEC;
    using Mirror;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;
    using Scp914Event = Exiled.Events.Handlers.Scp914;
    using Exiled.Events.EventArgs.Scp914;
    using PlayerRoles;
    using Scp914;
    using Exiled.Loader;
    using Exiled.API.Features.Pickups;
    using UnityEngine;

    internal sealed class Scp914Handler
    {
        public Scp914Handler()
        {
            Scp914Event.UpgradingPlayer += OnUpgradingPlayer;
        }
        public void OnDisabled()
        {
            Scp914Event.UpgradingPlayer -= OnUpgradingPlayer;
        }
        private void OnUpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                if (ev.KnobSetting == Scp914KnobSetting.Fine)
                {
                    ev.Player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);

                    ev.Player.Hurt(99);
                }
                else if (ev.KnobSetting == Scp914KnobSetting.VeryFine)
                {
                    if (UnityEngine.Random.Range(0, 5) > 3)
                    {
                        ev.Player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                    }
                    else
                    {
                        Pickup.CreateAndSpawn(ItemType.Medkit, ev.Player.Position, Quaternion.Euler(Vector3.zero), ev.Player);
                        ev.Player.Kill("ценой жизни переработан в аптечку..");
                    }
                }
            }
        }
    }
}