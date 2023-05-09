namespace Control.Events
{
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Сontrol;
    using MEC;
    using Mirror;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;
    using Scp914 = Exiled.Events.Handlers.Scp914;
    using Exiled.Events.EventArgs.Scp914;
    using PlayerRoles;
    using Scp914;
    using Exiled.Loader;
    using Exiled.API.Features.Pickups;

    internal sealed class Scp914Handler
    {
        public void OnEnabled()
        {
            Scp914.UpgradingPlayer += OnUpgradingPlayer;
        }
        public void OnDisabled()
        {
            Scp914.UpgradingPlayer -= OnUpgradingPlayer;
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
                    if (Loader.Random.Next(5) > 3)
                    {
                        if (ev.Player.Role == RoleTypeId.Scp0492)
                        {
                            ev.Player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                        }
                    } else
                    {
                        Pickup.CreateAndSpawn(ItemType.Medkit, ev.Player.Position, new UnityEngine.Quaternion(), ev.Player);
                        ev.Player.Kill("ценой жизни переработан в аптечку..");
                    }
                }
            }
        }
    }
}