namespace Alpha
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Firearms.Attachments;

    using UnityEngine;
    using Exiled.API.Enums;

    public sealed class Config : IConfig
    {
        [Description("Включен ли плагин?...")]
        public bool IsEnabled { get; set; } = true;

        [Description("Включен ли дебаг?...")]
        public bool Debug { get; set; } = true;

        [Description("Список вещей, которые нужно удалять если их кол-во превышает определенное..")]
        public List<ItemType> ListedItemType { get; set; } = new List<ItemType>
        {
            ItemType.GrenadeHE,
            ItemType.SCP244a,
            ItemType.SCP244b,
            ItemType.SCP207,
            ItemType.Ammo12gauge,
            ItemType.Ammo9x19,
            ItemType.Ammo44cal,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39,

        };
        [Description("Кол-во секунд после которых нужно уничтожить все вещи..")]
        public int ItemCountDoDestroy { get; set; } = 30;
    }
}