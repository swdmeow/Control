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
using Control.Commands;
using System.Linq;
using Control.Extensions;
using CustomPlayerEffects;

namespace Control.CustomItems
{
    [CustomItem(ItemType.SCP500)]
    public class SCP500R : CustomItem
    {
        public override uint Id { get; set; } = 5;
        public override string Name { get; set; } = "SCP-500-R.";
        public override string Description { get; set; } = "Воскрещает вас при смерти?..";
        public override ItemType Type { get; set; } = ItemType.SCP500;
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; } = null;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;

            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;

            base.UnsubscribeEvents();
        }
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (CustomItem.Get((uint)5).Check(ev.Item))
            {
                ev.IsAllowed = false;
            }
        }

        private void OnDying(DyingEventArgs ev)
        {
            foreach(Item item in ev.Player.Items)
            {
                if (CustomItem.Get((uint)5).Check(item))
                {
                    ev.Player.ShowHint("Вы можете возвродиться в любой момент, написав в консоль команду \".res\"", 120);

                    Res.DiedWithSCP500R.Add(ev.Player);
                    Res.RoleDiedWithSCP500R.Add(ev.Player.Role.Type);

                    foreach(StatusEffectBase eff in ev.Player.ActiveEffects)
                    {
                        if (eff.name == "CardiacArrest") continue;

                        Res.StatusEffectBase.Add(eff);
                    }

                    item.Destroy();

                    Timing.CallDelayed(0.1f, () =>
                    {
                        ev.Player.IsOverwatchEnabled = true;
                    });

                    return;
                }
            }
        }
        private void OnRoundStarted()
        {
            Timing.CallDelayed(0.2f, () =>
            {
                Pickup item = Pickup.List.ToList().Where(x => x.Type == ItemType.SCP500).First();

                item.Destroy();

                CustomItem.Get((uint)5).Spawn(item.Position);
            });
        }
    }
}