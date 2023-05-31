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

        [Description("������ ����������� ������� XPSystem..")]
        public XPSystem XPSystem { get; set; } = new XPSystem();
    }
}