namespace Control.Handlers.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using System;
    using Сontrol;
    using UnityEngine;
    using Exiled.CustomRoles.API.Features;
    using Exiled.API.Features.Pickups;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using MEC;
    using Control;
    using Exiled.API.Features.Items;
    using Interactables.Interobjects.DoorUtils;
    using Control.Commands;
    using Exiled.API.Features.Roles;
    using System.Threading.Tasks;
    using Mono.Security.X509.Extensions;
    using Player = Exiled.Events.Handlers.Player;
    using PlayerStatsSystem;
    using Exiled.API.Features.Pickups.Projectiles;
    using InventorySystem.Items.ThrowableProjectiles;
    using Steamworks.ServerList;
    using Exiled.Loader;
    using CommandSystem.Commands.RemoteAdmin;
    using GameCore;

    internal sealed class PlayerHandler
    {
        public PlayerHandler()
        {
            Player.UsingRadioBattery += OnUsingRadioBattery;
            Player.TriggeringTesla += OnTriggeringTesla;
            Player.Handcuffing += OnHandcuffing;

        }
        public void OnDisabled()
        {
            Player.UsingRadioBattery -= OnUsingRadioBattery;
            Player.TriggeringTesla -= OnTriggeringTesla;
            Player.Handcuffing -= OnHandcuffing;
        }
        private void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            if (radio.BooleanRadio == true)
            {
                ev.Drain = 0f;
                ev.Radio.BatteryLevel = 100;
            }
        }
        private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (Tesla.TeslaBoolean == true || Tesla.IgnoredTeams.Contains(ev.Player.Role.Team))
            {
                ev.IsInHurtingRange = false;
                ev.IsAllowed = false;
            }
        }
        private void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if(Cuff.IgnoredCuffPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
    }
}