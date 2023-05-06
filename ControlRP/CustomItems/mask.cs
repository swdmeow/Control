using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems;

using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using Steamworks.Data;
using System.Collections.Generic;
using System.Linq;
using Сontrol;
using System;
using System.Threading;

namespace Control.CustomItems
{
    [CustomItem(ItemType.GunCOM15)]
    public class GrenadeLauncher : CustomItem
    {
        public override uint Id { get; set; } = 2;

        public override string Name { get; set; } = "гранатомет нахуй";
        public override string Description { get; set; } = "Да.";
        public override ItemType Type { get; set; } = ItemType.GunCOM15;
        public override SpawnProperties SpawnProperties { get; set; }

        public void OnPickUpItem(PickingUpItemEventArgs ev)
        {
            if (!CustomItem.Get(2).Check(ev.Pickup)) return;

            var FireItem = ev.Pickup as FirearmPickup;

            if(FireItem.Ammo > 3)
            {
                FireItem.Ammo = 1;
            }
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if (!CustomItem.Get(2).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;

            var FireItem = ev.Player.CurrentItem as Firearm;

            if(FireItem.Ammo + 1 > 0)
            {
                ev.Player.ShowHitMarker(1);
                ev.Player.ThrowGrenade(Exiled.API.Enums.ProjectileType.FragGrenade);
                FireItem.Ammo--;
                return;
            }
            ev.Player.ShowHint("У вас нет патрон..", 3);
        }

        public void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!CustomItem.Get(2).Check(ev.Player)) return;
            
            ev.IsAllowed = false;

            var CountItem = 0;

            if(ev.Firearm.Ammo == 3)
            {
                return;
            }

            ev.Player.ShowHint("Перезарядка..", 3);

            foreach (Item item in ev.Player.Items.ToList())
            {
                Thread.Sleep(1500);

                if (!CustomItem.Get(2).Check(ev.Player.CurrentItem))
                {
                    ev.Player.ShowHint("Вы убрали пистолет из руки..", 3);

                    return;
                }

                if (item.Type == ItemType.GrenadeHE && ev.Firearm.Ammo != 3)
                {

                    ev.Firearm.Ammo++;
                    ev.Player.RemoveItem(item);

                    CountItem++;
                }
            }
            ev.Player.ShowHint($"+{CountItem}", 3);

            CountItem = 0;
        }
        public void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            if (!CustomItem.Get(2).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;
        }
    }
}