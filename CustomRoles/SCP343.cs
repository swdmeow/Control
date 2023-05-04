using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using Exiled.API.Features;
using System.Threading.Tasks;
using MEC;
using Exiled.CustomItems.API.Features;
using Interactables.Interobjects.DoorUtils;
using System;
using PlayerExtensions = Control.Extensions.PlayerExtensions;

namespace Control.CustomRoles
{
    [CustomRole(RoleTypeId.ClassD)]
    public class SCP343 : CustomRole
    {
        public override uint Id { get; set; } = 2;
        public override string Name { get; set; } = "SCP-343";
        public override string Description { get; set; } = "Вы бог - посмотрите свои возможности в консоли на (Ё)";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public override string CustomInfo { get; set; } = "SCP-343 \"Бог\"";
        public override int MaxHealth { get; set; } = 100;
        public override bool KeepInventoryOnSpawn { get; set; } = false;
        public override bool KeepRoleOnChangingRole { get; set; } = false;
        public override bool KeepRoleOnDeath { get; set; } = false;

        private static int CooldownRevolver = 0;
        private static int CooldownPainkillers = 0;
        private static int CooldownMedkit = 0;
        private static int CooldownSCP500 = 0;


        public static CoroutineHandle? HintCooldownCoroutineHandle = null;

        public override List<string> Inventory { get; set; } = new List<string>
        {
            "Coin",
            "GunRevolver",
            "SCP500",
            "Painkillers",
            "Medkit",
        };
        public override SpawnProperties SpawnProperties { get; set; } = null;

        private void OnRoundStarted()
        {
            if (Exiled.API.Features.Player.List.Count() >= 15)
            {
                CustomRole.Get(2).AddRole(Exiled.API.Features.Player.List.Where(x => x.Role.Type == RoleTypeId.ClassD)?.First());
            }
        }
        private void OnWaitingForPlayers()
        {
            CooldownRevolver = 0;
            CooldownPainkillers = 0;
            CooldownMedkit = 0;
            CooldownSCP500 = 0;
        }
        protected override void RoleAdded(Exiled.API.Features.Player player)
        {
            player.SendConsoleMessage("\n", "yellow");

            Exiled.API.Features.Roles.Scp173Role.TurnedPlayers.Add(player);
            Exiled.API.Features.Roles.Scp096Role.TurnedPlayers.Add(player);

            player.IsGodModeEnabled = true;
        }

        protected override void RoleRemoved(Exiled.API.Features.Player player)
        {
            Exiled.API.Features.Roles.Scp173Role.TurnedPlayers.Remove(player);
            Exiled.API.Features.Roles.Scp096Role.TurnedPlayers.Remove(player);

            player.IsGodModeEnabled = false;
        }
        private void OnShooting(ShootingEventArgs ev)
        {
            if (CustomRole.Get(2).Check(ev.Player))
            {
                Exiled.API.Features.Items.Firearm fire = ev.Player.CurrentItem as Exiled.API.Features.Items.Firearm;

                fire.Ammo = 2;
            }
        }
        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!CustomRole.Get(2).Check(ev.Player)) return;

            var randPlayer = Exiled.API.Features.Player.List.ElementAt(new System.Random().Next(0, Exiled.API.Features.Player.List.Count()));
            ev.Player.Position = randPlayer.Position;

            PlayerExtensions.ShowCustomHint(ev.Player, $"Вы были телепортированы к игроку {randPlayer.Nickname}..", 1);
        }
        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;

            if (CustomRole.Get(2).Check(ev.Attacker))
            {
                ev.IsAllowed = false;
                if (CooldownRevolver <= 0)
                {
                    CooldownRevolver = 180;

                    PlayerExtensions.ShowCustomHint(ev.Player, $"Вы были оглушены игроком {ev.Attacker.Nickname}", 2);

                    Vector3 pos = ev.Player.Position;


                    PlayerExtensions.ShowCustomHint(ev.Attacker, $"Вы оглушили игрока {ev.Player.Nickname}", 2);

                    ev.Player.IsGodModeEnabled = true;
                    Exiled.API.Features.Ragdoll ragdoll = Exiled.API.Features.Ragdoll.CreateAndSpawn(ev.Player.Role, ev.Player.Nickname, "наелся и спит..", ev.Player.Position, new Quaternion(0, 0, 0, 0), ev.Player);

                    // Yeah, without it, this shit don't work..
                    Timing.CallDelayed(0.1f, () => ev.Player.Position = new Vector3(-1000, -1000, -1000));

                    Timing.CallDelayed(5f, () =>
                    {
                        ev.Player.Position = pos;
                        ragdoll.Destroy();

                        Timing.CallDelayed(0.1f, () => ev.Player.IsGodModeEnabled = false);

                    });
                }
                else
                {
                    PlayerExtensions.ShowCustomHint(ev.Attacker, $"Подождите {CooldownRevolver} секунд, перед тем как снова оглушить игрока..", 1);
                }
            }
        }
        private void OnHandCuff(HandcuffingEventArgs ev)
        {
            if (!CustomRole.Get(2).Check(ev.Target)) return;

            ev.IsAllowed = false;
        }
        private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (!CustomRole.Get(2).Check(ev.Player)) return;
            ev.IsAllowed = false;
        }
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!CustomRole.Get(2).Check(ev.Player)) return;

            if (ev.Item.Type == ItemType.Coin) return;

            ev.IsAllowed = false;
        }
        private void OnPickUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Pickup == null) return;

            if(CustomItem.Get(3).Check(ev.Pickup))
            {
                ev.IsAllowed = false;
                return;
            }

            if (CustomRole.Get(2).Check(ev.Player))
            {
                if (ev.Pickup.Type == ItemType.GunCOM15
                    || ev.Pickup.Type == ItemType.GunCOM18 // Че за хуйня блять кто это писал
                    || ev.Pickup.Type == ItemType.GunCrossvec
                    || ev.Pickup.Type == ItemType.GunFSP9)
                {
                    ev.Pickup?.Destroy();

                    Pickup.CreateAndSpawn(ItemType.Medkit, ev.Player.Position, new Quaternion(0f, 0f, 0f, 0f));

                    ev.IsAllowed = false;

                    return;
                }

                if (ev.Pickup.Type == ItemType.GunAK
                    || ev.Pickup.Type == ItemType.GunCom45
                    || ev.Pickup.Type == ItemType.GunE11SR // Не ну реально заберите у него компьютер
                    || ev.Pickup.Type == ItemType.GunLogicer
                    || ev.Pickup.Type == ItemType.GunRevolver
                    || ev.Pickup.Type == ItemType.GunShotgun)
                {
                    ev.Pickup?.Destroy();

                    Pickup.CreateAndSpawn(ItemType.SCP500, ev.Player.Position, new Quaternion(0f, 0f, 0f, 0f));

                    ev.IsAllowed = false;

                    return; // А это что????
                }

                ev.IsAllowed = false;
            }
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            if (CustomRole.Get(2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private async void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (CustomRole.Get(2).Check(ev.Player))
            {
                if (Exiled.API.Features.Round.ElapsedTime.TotalSeconds >= 360)
                {
                    ev.IsAllowed = true;
                }
                else
                {
                    DoorPermissions doorPerms = Door.Get(ev.Door.Type).RequiredPermissions;
                    if (doorPerms.RequiredPermissions != Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
                    {
                        int CountSeconds = 0;

                        while (CountSeconds < 3)
                        {
                            string msg = $"Вы можете открывать двери через {Math.Round(360 - Exiled.API.Features.Round.ElapsedTime.TotalSeconds)} секунд";

                            for(int pl = 0; pl <= CountSeconds; pl++)
                            {
                                msg += ".";
                            }

                            PlayerExtensions.ShowCustomHint(ev.Player, msg, 1f);

                            CountSeconds++;

                            await Task.Delay(999);
                        }
                    }
                }
            }
        }
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;


            Exiled.Events.Handlers.Player.Handcuffing += OnHandCuff;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnteringPocketDimension;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;

            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickUpItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;


            Exiled.Events.Handlers.Player.Handcuffing -= OnHandCuff;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnteringPocketDimension;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;


            base.UnsubscribeEvents();
        }

        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!CustomRole.Get(2).Check(ev.Player)) return;

            ev.IsAllowed = false;

            if (ev.Item.Type == ItemType.Medkit || ev.Item.Type == ItemType.Painkillers)
            {
                var Players = Exiled.API.Features.Player.List.Where(x => x != ev.Player).Where(x => Vector3.Distance(ev.Player.Position, x.Position) <= 5f);

                if (Players.Count() <= 0) { PlayerExtensions.ShowCustomHint(ev.Player, "Игроки не найдены..", 1); return; }

                if (ev.Item.Type == ItemType.Painkillers)
                {
                    if (CooldownPainkillers <= 0)
                    {
                        var pl = Players.First();

                        pl.Heal(100);

                        PlayerExtensions.ShowCustomHint(ev.Player, $"Вы вылечили игрока {pl.Nickname}", 1);
                        PlayerExtensions.ShowCustomHint(pl, $"Вас вылечил SCP-343 ({ev.Player.Nickname})", 1);

                        CooldownPainkillers = 160;
                    }
                    else
                    {
                        PlayerExtensions.ShowCustomHint(ev.Player, $"Подождите ещё {CooldownPainkillers} секунд", 1);
                    }
                }
                if (ev.Item.Type == ItemType.Medkit)
                {
                    if (CooldownMedkit <= 0)
                    {
                        foreach (Exiled.API.Features.Player pl in Players)
                        {
                            pl.Heal(100);

                            PlayerExtensions.ShowCustomHint(ev.Player, $"Вы вылечили игрока {pl.Nickname}", 1);
                            PlayerExtensions.ShowCustomHint(pl, $"Вас вылечил SCP-343 ({ev.Player.Nickname})", 1);

                            CooldownPainkillers = 160;
                        }
                    }
                    else
                    {
                        PlayerExtensions.ShowCustomHint(ev.Player, $"Подождите ещё {CooldownPainkillers} секунд", 1);
                    }
                }
            }

            if (ev.Item.Type == ItemType.SCP500)
            {
                var Ragdolls = Exiled.API.Features.Ragdoll.List.Where(x => Vector3.Distance(ev.Player.Position, x.Position) <= 5f);

                if (Ragdolls.Count() <= 0) { PlayerExtensions.ShowCustomHint(ev.Player, "Игроки не найдены..", 1); return; }

                if (CooldownSCP500 <= 0)
                {
                    foreach(Ragdoll ragdoll in Ragdolls)
                    {
                        Log.Info(ragdoll.CreationTime.Second >= new DateTime().Second);
                        if(!ragdoll.Owner.IsAlive)
                        {
                            ragdoll.Owner.Role.Set(ragdoll.Role);
                            ragdoll.Owner.Position = ragdoll.Position + Vector3.up;
                            CooldownSCP500 = 500;
                            PlayerExtensions.ShowCustomHint(ev.Player, $"Вы восскресили игрока {ragdoll.Owner.Nickname}..", 1);
                            PlayerExtensions.ShowCustomHint(ragdoll.Owner, $"Вас восскресил игрок {ev.Player.Nickname}..", 1);

                            return;
                        }
                    }
                    PlayerExtensions.ShowCustomHint(ev.Player, $"Не найдено трупов поблизости..", 1);
                } else
                {
                    PlayerExtensions.ShowCustomHint(ev.Player, $"Подождите ещё {CooldownSCP500} секунд", 1);
                }
            }
        }
        public static IEnumerator<float> HintCoroutine()
        {
            for (; ; )
            {
                Log.Debug("Ticked cooldown..");
                if (CooldownPainkillers > 0) CooldownPainkillers--;
                if (CooldownRevolver > 0) CooldownPainkillers--;
                if (CooldownMedkit > 0) CooldownPainkillers--;
                if (CooldownSCP500 > 0) CooldownSCP500--;

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}