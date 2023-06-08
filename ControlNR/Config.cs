namespace �ontrol
{
    using Control;
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Control.Configs;

    public sealed class Config : IConfig
    {
        [Description("������� �� ������?")]
        public bool IsEnabled { get; set; } = true;

        [Description("������� �� �����-���?")]
        public bool Debug { get; set; } = false;

        [Description("������� �� ������ ������� ����� ����� ������� ������?")]
        public bool FullRoundRestart { get; set; } = true;

        [Description("������� �� ����� ���� � �������� ��� �� �� ��������� �� ����������?")]
        public bool RoomLootSpawn { get; set; } = true;

        [Description("������ ����������� ������� XPSystem..")]
        public XPSystem XPSystem { get; set; } = new XPSystem();
    }
}