namespace Control.Events
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;
    using System;
    using Сontrol;
    using UnityEngine;

    internal sealed class PlayerHandler
    {
        public void OnSpawned(SpawnedEventArgs ev)
        {
            CustomItem.Get(2).Spawn(ev.Player);
            CustomItem.Get(2).Spawn(new Vector3(40, 1014, -32));


            var rand = new System.Random();
            if (ev.Player.Role.Type == PlayerRoles.RoleTypeId.ClassD)
            {
                if (rand.Next(1, 10) > control.Instance.Config.ChanceToSpawnJClass)
                {
                    ev.Player.DisplayNickname = $"J-{rand.Next(1000, 9999)} [{ev.Player.Nickname}]";
                    ev.Player.ShowHint("Вы - Уборщик.", 5);

                    ev.Player.AddItem(ItemType.KeycardJanitor);
                }
                else
                {
                    ev.Player.DisplayNickname = $"D-{rand.Next(1000, 9999)} [{ev.Player.Nickname}]";
                }
            }

            if (ev.Player.IsScp)
            {
                ev.Player.DisplayNickname = $"{ev.Player.Role.Name} [{ev.Player.Nickname}]";
            }

            var isUsed = false;

            if (ev.Player.Role.Type == PlayerRoles.RoleTypeId.Scientist)
            {
                ev.Player.DisplayNickname = $"{control.Instance.Config.namesOfScientists[rand.Next(control.Instance.Config.namesOfScientists.Length)]} [{ev.Player.Nickname}]";

                if (isUsed != true)
                {
                    if (rand.Next(1, 10) > control.Instance.Config.ChanceToSpawnManager)
                    {
                        ev.Player.Position = new UnityEngine.Vector3(165, -999, 60);

                        ev.Player.ShowHint("Вы - Менеджер комплекса.", 5);

                        ev.Player.DisplayNickname = $"{control.Instance.Config.nameOfManager[rand.Next(control.Instance.Config.nameOfManager.Length)]} [{ev.Player.Nickname}]";

                        foreach (var item in control.Instance.Config.ManagerItemsType)
                        {
                            ev.Player.AddItem(item);
                        }
                    }
                    isUsed = true;
                }
            }
        }
    }
}