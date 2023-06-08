using Control.Extensions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.API.Interfaces;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Networking;

namespace Control.CustomItems
{
    [CustomItem(ItemType.GunCOM18)]
    public class GrenadeLauncher : CustomItem
    {
        public override uint Id { get; set; } = 4;
        public override string Name { get; set; } = "гранатомёт";
        public override string Description { get; set; } = "Стреляет гранатами. Перезарядка гранат - <b>раз в 30 секунд..</b>";
        public override ItemType Type { get; set; } = ItemType.GunCOM18;

        public static bool CooldownIsEnable = false;
        public override float Weight { get; set; }
        public override SpawnProperties SpawnProperties { get; set; } = null;
        private void OnStartedRound()
        {
            FirearmPickup pickup;

            if (Loader.Random.Next(100) > 50)
            {
                pickup = CustomItem.Get((uint)4).Spawn(Room.Get(RoomType.LczArmory).transform.position + Vector3.up) as FirearmPickup;
            }
            else
            {
                pickup = CustomItem.Get((uint)4).Spawn(Room.Get(RoomType.HczArmory).transform.position + Vector3.up) as FirearmPickup;
            }
            Timing.CallDelayed(0.1f, () => { pickup.Ammo = 1; });
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!CustomItem.Get((uint)4).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;

            var FireItem = ev.Player.CurrentItem as Firearm;

            if (FireItem.Ammo + 1 > 0)
            {
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
            Exiled.Events.Handlers.Server.RoundStarted += OnStartedRound;


            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloading;
            Exiled.Events.Handlers.Player.UnloadingWeapon -= OnUnloadingWeapon;
            Exiled.Events.Handlers.Server.RoundStarted -= OnStartedRound;

            base.UnsubscribeEvents();
        }

        private void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!CustomItem.Get((uint)4).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;

            if (CooldownIsEnable == true || ev.Firearm.Ammo >= 1) return;

            new RequestMessage(ev.Firearm.Serial, RequestType.Reload).SendToAuthenticated();

            ev.Firearm.Ammo = 1;
            CooldownIsEnable = true;
            Timing.CallDelayed(30f, () => CooldownIsEnable = false);
        }
        private void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            if (!CustomItem.Get((uint)4).Check(ev.Player.CurrentItem)) return;

            ev.IsAllowed = false;
        }
    }
}