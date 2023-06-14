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
using Exiled.Events.EventArgs.Scp914;
using Exiled.API.Enums;
using Exiled.Events.Patches.Events.Scp330;
using Exiled.Events.EventArgs.Scp330;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Warhead;

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
        private static int CooldownPainkillers = 0;
        private static int CooldownMedkit = 0;
        private static int CooldownSCP500 = 0;


        public static CoroutineHandle? HintCooldownCoroutineHandle = null;

        public override List<string> Inventory { get; set; } = new List<string>
        {
            "Coin",
            "SCP500",
            "Painkillers",
            "Medkit",
        };
        public override SpawnProperties SpawnProperties { get; set; } = null;

        private void OnRoundStarted()
        {
            if (Exiled.API.Features.Player.List.Count() >= 15)
            {
                CustomRole.Get((uint)2).AddRole(Exiled.API.Features.Player.List.Where(x => x.Role.Type == RoleTypeId.ClassD)?.First());
            }
        }
        private void OnWaitingForPlayers()
        {
            CooldownPainkillers = 0;
            CooldownMedkit = 0;
            CooldownSCP500 = 0;
        }
        protected override void RoleAdded(Exiled.API.Features.Player player)
        {
            player.SendConsoleMessage("\nВы - SCP-343\nВыкинув <color=red>SCP-500</color> вы можете восскресить человека. Достаточно находится рядом возле его трупа.\nВыкинув <color=red>аптечку</color> или <color=white>болеутоляющее</color> вы можете восстановить здоровье человеку. Достаточно находится рядом возле него.\n<color=red>Аптечка</color> лечит всех людей в радиусе.\n<color=white>Болеутоляющее</color> лечит <b>одного</b> людей в радиусе.", "yellow");

            Exiled.API.Features.Roles.Scp173Role.TurnedPlayers.Add(player);
            Exiled.API.Features.Roles.Scp096Role.TurnedPlayers.Add(player);
            Exiled.API.Features.Roles.Scp049Role.TurnedPlayers.Add(player);

            player.IsGodModeEnabled = true;
        }

        protected override void RoleRemoved(Exiled.API.Features.Player player)
        {
            Exiled.API.Features.Roles.Scp173Role.TurnedPlayers.Remove(player);
            Exiled.API.Features.Roles.Scp096Role.TurnedPlayers.Remove(player);
            Exiled.API.Features.Roles.Scp049Role.TurnedPlayers.Remove(player);

            player.IsGodModeEnabled = false;
        }
        private void OnShooting(ShootingEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!CustomRole.Get((uint)2).Check(ev.Player)) return;

            var PlayersList = Exiled.API.Features.Player.List.Where(x => x != ev.Player && x.IsAlive && x.Role.Type != RoleTypeId.Scp079);
            Player pl = PlayersList.ToList().RandomItem();

            ev.Player.Position = pl.Position;

            ev.Player.ShowHint($"Вы были телепортированы к игроку {pl.Nickname}..", 1);
        }
        private void OnHandCuff(HandcuffingEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Target) || CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (!CustomRole.Get((uint)2).Check(ev.Player)) return;
            ev.IsAllowed = false;
        }
        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!CustomRole.Get((uint)2).Check(ev.Player)) return;

            if (ev.Item.Type == ItemType.Coin) return;

            ev.IsAllowed = false;

        }
        private void OnPickUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Pickup == null) return;

            /*if(CustomItem.Get((uint)3).Check(ev.Pickup))
            {
                ev.IsAllowed = false;
                return;
            }*/

            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                if (ev.Pickup.Type == ItemType.GunCOM15
                    || ev.Pickup.Type == ItemType.GunCOM18 // Че за хуйня блять кто это писал
                    || ev.Pickup.Type == ItemType.GunCrossvec
                    || ev.Pickup.Type == ItemType.GunFSP9)
                {
                    ev.Pickup?.Destroy();

                    Pickup.CreateAndSpawn(ItemType.Medkit, ev.Player.Position, Quaternion.Euler(Vector3.zero));

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

                    Pickup.CreateAndSpawn(ItemType.SCP500, ev.Player.Position, Quaternion.Euler(Vector3.zero));

                    ev.IsAllowed = false;

                    return;
                }

                if (ev.Pickup.Type == ItemType.MicroHID)
                {
                    ev.Pickup?.Destroy();

                    Pickup.CreateAndSpawn(ItemType.MicroHID, ev.Player.Position, Quaternion.Euler(Vector3.zero));

                    ev.IsAllowed = false;

                    return;
                }

                ev.IsAllowed = false;
            }
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                if (Exiled.API.Features.Round.ElapsedTime.TotalSeconds >= 360)
                {
                    ev.IsAllowed = true;
                }
                else
                {
                    if (Door.Get(ev.Door.Type).RequiredPermissions.RequiredPermissions != Interactables.Interobjects.DoorUtils.KeycardPermissions.None)
                    {
                        ev.Player.ShowHint($"Вы можете открывать двери через {Math.Round(360 - Exiled.API.Features.Round.ElapsedTime.TotalSeconds)} секунд", 1f);
                    }
                }
            }
        }
        private void UpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void UpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void ClosingGenerator(ClosingGeneratorEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void ActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        private void StoppingGenerator(StoppingGeneratorEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += UpgradingInventoryItem;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += UpgradingPlayer;

            Exiled.Events.Handlers.Player.OpeningGenerator += OnOpeningGenerator;
            Exiled.Events.Handlers.Player.ClosingGenerator += ClosingGenerator;
            Exiled.Events.Handlers.Player.ActivatingGenerator += ActivatingGenerator;
            Exiled.Events.Handlers.Player.StoppingGenerator += StoppingGenerator;

            Exiled.Events.Handlers.Warhead.ChangingLeverStatus += OnChangingLeverStatus;

            Exiled.Events.Handlers.Player.Handcuffing += OnHandCuff;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnteringPocketDimension;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;

            base.SubscribeEvents();
        }
        private void OnChangingLeverStatus(ChangingLeverStatusEventArgs ev)
        {
            if (CustomRole.Get((uint)2).Check(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Warhead.ChangingLeverStatus -= OnChangingLeverStatus;
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickUpItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.Shooting -= OnShooting;
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= UpgradingInventoryItem;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= UpgradingPlayer;
            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInteractingScp330;
            Exiled.Events.Handlers.Player.OpeningGenerator -= OnOpeningGenerator;
            Exiled.Events.Handlers.Player.ClosingGenerator -= ClosingGenerator;
            Exiled.Events.Handlers.Player.ActivatingGenerator -= ActivatingGenerator;
            Exiled.Events.Handlers.Player.StoppingGenerator -= StoppingGenerator;

            Exiled.Events.Handlers.Player.Handcuffing -= OnHandCuff;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnteringPocketDimension;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;

            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;


            base.UnsubscribeEvents();
        }

        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!CustomRole.Get((uint)2).Check(ev.Player)) return;

            ev.IsAllowed = false;

            if (ev.Item.Type == ItemType.Medkit || ev.Item.Type == ItemType.Painkillers)
            {
                var Players = Exiled.API.Features.Player.List.Where(x => !x.IsScp).Where(x => x != ev.Player).Where(x => (Vector3.Distance(ev.Player.Position, x.Position) <= 5f) && x.Role.Side != Side.Scp);

                if (Players.Count() <= 0) { ev.Player.ShowHint("Игроки не найдены..", 1); return; }

                if (ev.Item.Type == ItemType.Painkillers)
                {
                    if (CooldownPainkillers <= 0)
                    {
                        var pl = Players.First();

                        pl.Heal(100);

                        ev.Player.ShowHint($"Вы вылечили игрока {pl.Nickname}", 1);
                        pl.ShowHint($"Вас вылечил SCP-343 ({ev.Player.Nickname})", 1);

                        CooldownPainkillers = 60;
                    }
                    else
                    {
                        ev.Player.ShowHint($"Подождите ещё {CooldownPainkillers} секунд", 1);
                    }
                }
                if (ev.Item.Type == ItemType.Medkit)
                {
                    if (CooldownMedkit <= 0)
                    {
                        foreach (Exiled.API.Features.Player pl in Players)
                        {
                            pl.Heal(100);

                            ev.Player.ShowHint($"Вы вылечили игрока {pl.Nickname}", 1);
                            pl.ShowHint($"Вас вылечил SCP-343 ({ev.Player.Nickname})", 1);

                            CooldownMedkit = 120;
                        }
                    }
                    else
                    {
                        ev.Player.ShowHint($"Подождите ещё {CooldownMedkit} секунд", 1);
                    }
                }
            }

            if (ev.Item.Type == ItemType.SCP500)
            {
                var Ragdolls = Exiled.API.Features.Ragdoll.List.Where(x => (Vector3.Distance(ev.Player.Position, x.Position) <= 5f) && x.Role.GetSide() != Side.Scp);

                if (Ragdolls.Count() <= 0) { ev.Player.ShowHint("Игроки не найдены..", 1); return; }

                if (CooldownSCP500 <= 0)
                {
                    foreach(Ragdoll ragdoll in Ragdolls)
                    {
                        if(!ragdoll.Owner.IsAlive)
                        {
                            ragdoll.Owner.Role.Set(ragdoll.Role, Exiled.API.Enums.SpawnReason.Revived, RoleSpawnFlags.None);
                            ragdoll.Owner.Position = ragdoll.Position + Vector3.up;
                            CooldownSCP500 = 444;
                            ev.Player.ShowHint($"Вы восскресили игрока {ragdoll.Owner.Nickname}..", 1);
                            ragdoll.Owner.ShowHint($"Вас восскресил игрок {ev.Player.Nickname}..", 1);

                            ragdoll.Destroy();

                            return;
                        }
                    }
                    ev.Player.ShowHint($"Не найдено трупов поблизости..", 1);
                } else
                {
                    ev.Player.ShowHint($"Подождите ещё {CooldownSCP500} секунд", 1);
                }
            }
        }
        public static IEnumerator<float> HintCoroutine()
        {
            for (; ; )
            {
                if (CooldownPainkillers > 0) CooldownPainkillers--;
                if (CooldownMedkit > 0) CooldownMedkit--;
                if (CooldownSCP500 > 0) CooldownSCP500--;

                yield return Timing.WaitForSeconds(1f);
            }
        }
        public override void AddRole(Exiled.API.Features.Player player)
        {
            Log.Debug(Name + ": Adding role to " + player.Nickname + ".");
            TrackedPlayers.Add(player);
            if (Role != RoleTypeId.None)
            {
                if (KeepPositionOnSpawn && KeepInventoryOnSpawn)
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                }
                else if (KeepPositionOnSpawn)
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);
                }
                else if (KeepInventoryOnSpawn)
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                }
                else
                {
                    player.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.All);
                }
            }

            if (!KeepInventoryOnSpawn)
            {
                Log.Debug(Name + ": Clearing " + player.Nickname + "'s inventory.");
                player.ClearInventory();
            }

            foreach (string item in Inventory)
            {
                Log.Debug(Name + ": Adding " + item + " to inventory.");
                TryAddItem(player, item);
            }

            Log.Debug(Name + ": Setting health values.");
            player.Health = MaxHealth;
            player.MaxHealth = MaxHealth;
            player.Scale = Scale;
            Vector3 spawnPosition = GetSpawnPosition();
            if (spawnPosition != Vector3.zero)
            {
                player.Position = spawnPosition;
            }

            Log.Debug(Name + ": Setting player info");
            player.CustomInfo = player.CustomName + "\n" + CustomInfo;
            player.InfoArea &= ~(PlayerInfoArea.Nickname | PlayerInfoArea.Role);
            if (CustomAbilities != null)
            {
                foreach (CustomAbility item2 in CustomAbilities!)
                {
                    item2.AddAbility(player);
                }
            }

            ShowMessage(player);
            ShowBroadcast(player);
            RoleAdded(player);
            player.UniqueRole = Name;
            player.TryAddCustomRoleFriendlyFire(Name, CustomRoleFFMultiplier);
            if (string.IsNullOrEmpty(ConsoleMessage))
            {
                return;
            }
            // Delete stringBuilder to not cause console message
        }
    }
}