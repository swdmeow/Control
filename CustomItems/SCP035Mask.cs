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
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Control.CustomItems
{
    [CustomItem(ItemType.Painkillers)]
    public class SCP035Mask : CustomItem
    {
        public override uint Id { get; set; } = 3;
        public override string Name { get; set; } = "маску.";
        public override string Description { get; set; } = "Превращает вас в SCP-035..";
        public override ItemType Type { get; set; } = ItemType.Painkillers;
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; } = null;

        public List<(ItemType a, int b)> Types = new()
        {
            new(ItemType.Painkillers, 10),
            new(ItemType.Coin, 10),
            new(ItemType.Medkit, 20),
            new(ItemType.Adrenaline, 30),
            new(ItemType.Coin, 100),
        };
        public override Pickup Spawn(Vector3 position, Player owner = null)
        {
            Pickup pickup = Pickup.CreateAndSpawn(RandomType(), position, default);
            pickup.Weight = Weight;
            TrackedSerials.Add(pickup.Serial);
            return pickup;
        }
        private ItemType RandomType()
        {
            if (Types.Count == 1)
                return Types[0].a;

            foreach ((ItemType type, int chance) in Types)
            {
                if (Loader.Random.Next(100) <= chance)
                    return type;
            }

            return Type;
        }
        protected override void OnAcquired(Exiled.API.Features.Player player)
        {
            // Do nothing, to not cause pickup message and other things..
        }
        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (CustomItem.Get(3).Check(ev.NewItem))
            {
                ev.IsAllowed = false;
            }
        }
        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            if (CustomItem.Get(3).Check(ev.Pickup))
            {
                if (CustomRole.Get(1).Check(ev.Player) || CustomRole.Get(2).Check(ev.Player))
                {
                    ev.IsAllowed = false;
                    return;
                }
                CustomRole.Get(1).AddRole(ev.Player);

                ev.IsAllowed = false;

                ev.Pickup.Destroy();
            }
        }
    }
}