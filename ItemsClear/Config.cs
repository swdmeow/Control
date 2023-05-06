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
        [Description("������� �� ������?...")]
        public bool IsEnabled { get; set; } = true;

        [Description("������� �� �����?...")]
        public bool Debug { get; set; } = true;

        [Description("������ �����, ������� ����� ������� ���� �� ���-�� ��������� ������������..")]
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
        [Description("���-�� ������ ����� ������� ����� ���������� ��� ����..")]
        public int ItemCountDoDestroy { get; set; } = 30;
    }
}