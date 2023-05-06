namespace �ontrol
{
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class Config : IConfig
    {
        [Description("������� �� ������?")]
        public bool IsEnabled { get; set; } = true;
        [Description("������� �� �����-���?")]
        public bool Debug { get; set; } = false;
        [Description("���������� CASSIE.")]
        public string StartCassie { get; set; } = "��������! CD<b></b>-01 �����������. �������������� ������ D<b></b> - ���� �� �������� ���� ������ ����� �� ��������� �����������.<color=#ffffff00>h pitch_1.06 . Attention . pitch_0.96 room C D 0 1 is pitch_0.93 unstable . warning for ClassD . if you leave your chamber then you terminated . .g6 </color>";
        [Description("������� ������ ����� ��������� ����� CASSIE �����������?")]
        public float WaitingTimeToCassie { get; set; } = 1.5f;
        [Description("������� ������ ����� ��������� ����� CASSIE (� �.�.�.) �����������?")]
        public float WaitingTimeToCassieContaimentBreach { get; set; } = 31.5f;
        [Description("���������� CASSIE (� �.�.�.) � ������ ������.")]
        public string StartCassieContaimentBreach { get; set; } = "���������! ���������� ��������� ������� ���������� � ����� ���� ����������. SCP-173<b></b> �� ��������� ������. ������ ����������� ������������ ������������ ����������� � 0<b></b> ������� ������� ��� ������ �� <color=#6aa84f>˸���� ���� ����������</color>.<color=#ffffff00>h pitch_1 . .g6 Danger . detected containment breach in light containment zone . SCP 1 7 3 outside the chamber . order for security personnel to evacuate personnel with 0 or more access level from the light containment zone . .g1 . .g6 </color>";
        [Description("������� ������ ����� ��������� ����� ����� �-������ ���������?")]
        public float WaitingTimeToOpenPrisonDoors { get; set; } = 2.5f;

        [Description("����� ���� ������ J-������? (�� 0 �� 10)")]
        public int ChanceToSpawnJClass { get; set; } = 9;
        [Description("����� ���� ������ ��������� ���������? (�� 0 �� 10)")]
        public int ChanceToSpawnManager { get; set; } = 9;
        [Description("����� ������� �����������..")]
        public string[] namesOfScientists { get; set;  } = new string[] { "������ \"����\"", "������ \"�����\"" };
        [Description("����� ��������� ���������..")]
        public string[] nameOfManager { get; set; } = new string[] { "����", "�������", "������" };
        [Description("��������� ���� ��������� ���������..")]
        public string CustomInfoOfManager { get; set; } = "�������� ���������..";
        [Description("������ �����, ������� ������� � ��������� ���������..")]
        public List<ItemType> ManagerItemsType { get; set; } = new List<ItemType>
        {
            ItemType.KeycardFacilityManager,
            ItemType.Medkit,
            ItemType.ArmorLight,
            ItemType.GunCOM18,
            ItemType.Ammo9x19,
            ItemType.Ammo9x19,
            ItemType.Ammo9x19,
            ItemType.Ammo9x19,
        };
    }
}