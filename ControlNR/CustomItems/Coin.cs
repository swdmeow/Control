using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using Hazards;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp173;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Control.CustomItems
{
    [CustomItem(ItemType.Coin)]

    public class Coin : CustomItem
    {
        public override uint Id { get; set; } = 7;
        public override string Name { get; set; } = "подозрительную монетку.";
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
            RoomType.EzConference,
            RoomType.EzCrossing,
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
            RoomType.EzCafeteria,
            RoomType.EzCheckpointHallway,
            RoomType.EzGateA,
            RoomType.EzGateB
        };
        private static List<CandyKindID> _candyID = new List<CandyKindID>()
        {
            CandyKindID.Rainbow,
            CandyKindID.Yellow,
            CandyKindID.Green,
            CandyKindID.Pink,
            CandyKindID.Blue,
            CandyKindID.Red
        };
        private string[] Events { get; } = new string[] { "teleport", "boom", "ToZombie", "betrayTeam", "speed", "candy", "flash", "size", "slow", "poop", "kick" };
        public override SpawnProperties SpawnProperties { get; set; } = null;
        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (CustomItem.Get((uint)7).Check(ev.Player.CurrentItem))
            {
                var rand = new System.Random();

                string needTo = Events.ElementAt(rand.Next(0, Events.Count()));

                if (needTo == "teleport")
                {
                    ev.Player.EnableEffect(EffectType.Flashed, 1f);

                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (!Warhead.IsDetonated && !Warhead.IsInProgress) ev.Player.Position = Room.Get(_rooms.ElementAt(new System.Random().Next(0, _rooms.Count()))).transform.position + Vector3.up;

                        else
                        {
                            ev.Player.Position = Room.Get(RoomType.Surface).transform.position;
                        }
                    });

                    ev.Player.CurrentItem.Destroy();
                }
                if (needTo == "speed")
                {
                    ev.Player.CurrentItem.Destroy();

                    ev.Player.EnableEffect(EffectType.MovementBoost, 45);

                    ev.Player.GetEffect(EffectType.MovementBoost).Intensity = 150;
                }
                if (needTo == "poop")
                {
                    ev.Player.CurrentItem.Destroy();

                    Map.PlaceTantrum(ev.Player.Position);
                }

                if (needTo == "slow")
                {
                    ev.Player.CurrentItem.Destroy();

                    ev.Player.EnableEffect(EffectType.Stained, 45);
                    ev.Player.EnableEffect(EffectType.SinkHole, 20);
                    ev.Player.EnableEffect(EffectType.Blinded, 15);
                }
                if (needTo == "ToZombie")
                {
                    ev.Player.CurrentItem.Destroy();

                    if(CustomRole.Get((uint)1).Check(ev.Player))
                    {
                        // lmao..
                        ev.Player.EnableEffect(EffectType.SeveredHands, 1f);

                        return;
                    }

                    ev.Player.Role.Set(PlayerRoles.RoleTypeId.Scp0492);
                }

                if (needTo == "betrayTeam")
                {
                    ev.Player.CurrentItem.Destroy();

                    if (ev.Player.Role.Team == PlayerRoles.Team.FoundationForces || ev.Player.Role == RoleTypeId.Scientist)
                    {
                        ev.Player.Role.Set(PlayerRoles.RoleTypeId.ChaosConscript, PlayerRoles.RoleSpawnFlags.None);
                    }
                    else
                    {
                        ev.Player.Role.Set(PlayerRoles.RoleTypeId.NtfPrivate, PlayerRoles.RoleSpawnFlags.None);
                    } // 1
                }
                if (needTo == "boom")
                {
                    ev.Player.CurrentItem.Destroy();

                    ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, ev.Player);

                    grenade.FuseTime = 0.2f;
                    grenade.ScpDamageMultiplier = 55;

                    grenade.SpawnActive(ev.Player.Position + Vector3.up);
                }
                if (needTo == "candy")
                {
                    ev.Player.CurrentItem.Destroy();

                    foreach (Player pl in Player.List)
                    {
                        pl.Broadcast(new Exiled.API.Features.Broadcast($"{ev.Player.Nickname} запустил конфетную вечеринку.."));
                    }

                    Timing.RunCoroutine(EveryoneCandy());
                }
                if (needTo == "flash")
                {
                    ev.Player.CurrentItem.Destroy();

                    int flashes = 0;

                    while (flashes <= 3)
                    {
                        FlashGrenade grenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash, ev.Player);

                        grenade.FuseTime = 0.2f;

                        grenade.SpawnActive(ev.Player.Position + Vector3.up);

                        flashes++;
                    }
                }
                if (needTo == "size")
                {
                    ev.Player.CurrentItem.Destroy();

                    if(CustomRole.Get((uint)8).Check(ev.Player))
                    {
                        Pickup.CreateAndSpawn(ItemType.GrenadeHE, ev.Player.Position, new Quaternion());

                        return;
                    }
                    CustomRole.Get((uint)8).AddRole(ev.Player);
                }
                if(needTo == "kick")
                {
                    ev.Player.CurrentItem.Destroy();

                    ev.Player.Kick("Испепелён с сервера с помощью волшебной монетки.");
                    Map.Broadcast(new Exiled.API.Features.Broadcast($"{ev.Player.Nickname} испепелён с сервера с помощью волшебной монетки", 5));
                }
            }
        }
        private void OnRoundStarted()
        {
            CustomItem.Get((uint)7).Spawn(Room.List.ElementAt(new System.Random().Next(0, Room.List.Count())).transform.position + Vector3.up);
            
            foreach(Pickup _pickup in Pickup.List)
            {
                if(_pickup.Type == ItemType.Coin)
                {
                    CustomItem.Get((uint)7).Spawn(_pickup.Position + Vector3.up);

                    _pickup.Destroy();

                    return;
                }
            }
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

        public static IEnumerator<float> EveryoneCandy()
        {
            int candy = 0;
            while (candy <= 15)
            {
                foreach (Player pl in Player.List.Where(x => x.IsAlive && !x.IsScp))
                {
                    if (CustomRole.Get((uint)2).Check(pl)) continue;

                    pl.TryAddCandy(_candyID.RandomItem());

                    Scp330 bag_candy = (Scp330)Item.Create(ItemType.SCP330);
                    CandyKindID _candyId = _candyID.RandomItem();
                    bag_candy.AddCandy(_candyId);
                    Pickup pickup = bag_candy.CreatePickup(pl.Position);
                }

                candy++;
                yield return Timing.WaitForSeconds(1.5f);
            }
        }
    }
}