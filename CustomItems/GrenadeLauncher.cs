using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Hints;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Control.CustomItems
{
    [CustomItem(ItemType.GunCOM15)]
    public class GrenadeLauncher : CustomItem
    {
        public override uint Id { get; set; } = 4;
        public override string Name { get; set; } = "гранатомёт";
        public override string Description { get; set; } = "Стреляет гранатами. Максимум гранат в обойме - 2. Перезарядка гранатами, 1 граната - 3 секунды";
        public override ItemType Type { get; set; } = ItemType.GunCOM15;

        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 55,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside173Armory,
                },
            }

        };

        private void OnPickUpItem(PickingUpItemEventArgs ev)
        {
            if (!CustomItem.Get(4).Check(ev.Pickup)) return;

            var FireItem = ev.Pickup as FirearmPickup;

            if(FireItem.Ammo > 3)
            {
                FireItem.Ammo = 1;
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!CustomItem.Get(4).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;

            var FireItem = ev.Player.CurrentItem as Firearm;

            if(FireItem.Ammo + 1 > 0)
            {
                ev.Player.ShowHitMarker(1);
                ev.Player.ThrowGrenade(Exiled.API.Enums.ProjectileType.FragGrenade);
                FireItem.Ammo--;
                return;
            }
        }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloading;
            Exiled.Events.Handlers.Player.UnloadingWeapon += OnUnloadingWeapon;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickUpItem;

            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloading;
            Exiled.Events.Handlers.Player.UnloadingWeapon -= OnUnloadingWeapon;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickUpItem;

            base.UnsubscribeEvents();
        }

        private async void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!CustomItem.Get(4).Check(ev.Player)) return;

            ev.IsAllowed = false;

            if (ev.Firearm.Ammo == 3)
            {
                return;
            }

            Firearm GrenadeLauncher = ev.Firearm;

            foreach (Item item in ev.Player.Items.ToList())
            {
                if (item.Type == ItemType.GrenadeHE)
                {
                    ev.Player.ShowHint("Перезарядка..", 3);

                    Log.Info("Start reload");
                    await Task.Delay(3000);
                    Log.Info("Reload end");

                    if (!CustomItem.Get(4).Check(ev.Player.CurrentItem))
                    {
                        ev.Player.ReferenceHub.hints.Show(new TextHint("Вы убрали пистолет из своей руки..", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(0f, 12f, 22f), 3f));

                        return;
                    }

                    if (GrenadeLauncher.Ammo >= 3) return;

                    ev.Player.ShowHint("Перезарядка..", 3);

                    Log.Info("Reached end");

                    GrenadeLauncher.Ammo++;
                    Log.Info(GrenadeLauncher.Ammo);
                    ev.Player.RemoveItem(item);
                }
            }
        }
        private void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            if (!CustomItem.Get(4).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;
        }
    }
}