using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using MEC;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Control.CustomItems
{
    [CustomItem(ItemType.Coin)]

    public class Coin : CustomItem
    {
        public override uint Id { get; set; } = 7;
        public override string Name { get; set; } = "монетку.";
        public override string Description { get; set; } = "Подкиньте монетку - узнайте что с вами случится..";
        public override ItemType Type { get; set; } = ItemType.Coin;
        public override float Weight { get; set; }
        private List<RoomType> _rooms = new List<RoomType>()
        {
            RoomType.Surface,
            RoomType.Hcz939,
            RoomType.Hcz106,
            RoomType.HczTCross,
            RoomType.EzCheckpointHallway,
            RoomType.EzCollapsedTunnel,
            RoomType.EzConference,
            RoomType.EzCrossing,
            RoomType.Pocket,
            RoomType.LczToilets,
            RoomType.LczTCross,
            RoomType.HczArmory,
            RoomType.LczArmory,
            RoomType.HczElevatorA,
            RoomType.LczCafe,
            RoomType.Lcz330,
            RoomType.Lcz914,
            RoomType.LczAirlock,
            RoomType.LczClassDSpawn,
            RoomType.LczCheckpointA,
            RoomType.LczCheckpointB,
            RoomType.HczHid,
        };
        private string[] Events { get; } = new string[] { "teleport", "boom", "ToZombie", "betrayTeam", "speed" };
        public override SpawnProperties SpawnProperties { get; set; } = null;
        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (CustomItem.Get((uint)7).Check(ev.Player.CurrentItem))
            {
                var rand = new System.Random();

                string needTo = Events.ElementAt(rand.Next(1, Events.Count()));

                if (needTo == "teleport")
                {

                    ev.Player.EnableEffect(EffectType.Flashed, 1f);

                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (!Warhead.IsDetonated) ev.Player.Position = Room.Get(_rooms.ElementAt(new System.Random().Next(0, _rooms.Count()))).transform.position + Vector3.up;
                        else
                        {
                            ev.Player.Position = Room.Get(RoomType.Surface).transform.position;
                        }
                    });

                    ev.Player.CurrentItem.Destroy();
                }
                if (needTo == "speed")
                {
                    ev.Player.EnableEffect(EffectType.MovementBoost, 45);

                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity = 255;
                }
                if (needTo == "ToZombie")
                {
                    ev.Player.CurrentItem.Destroy();

                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.Scp0492);
                }

                if (needTo == "betrayTeam")
                {
                    ev.Player.CurrentItem.Destroy();

                    if (ev.Player.Role.Team == PlayerRoles.Team.FoundationForces)
                    {
                        ev.Player.Role.Set(PlayerRoles.RoleTypeId.ChaosConscript, PlayerRoles.RoleSpawnFlags.None);
                    }
                    else
                    {
                        ev.Player.Role.Set(PlayerRoles.RoleTypeId.NtfPrivate, PlayerRoles.RoleSpawnFlags.None);
                    }
                }
                if (needTo == "boom")
                {
                    ev.Player.CurrentItem.Destroy();

                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);
                    grenade.FuseTime = 0.1f;
                    grenade.ScpDamageMultiplier = 5.0f;
                    grenade.SpawnActive(ev.Player.Position + Vector3.up);
                }
            }
        }
        private void OnRoundStarted()
        {
            CustomItem.Get((uint)7).Spawn(Room.List.ElementAt(new System.Random().Next(0, Room.List.Count())).transform.position + Vector3.up);
        }
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;

            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;

            base.UnsubscribeEvents();
        }
    }
}