namespace Сontrol
{
    using Control;
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Control.Configs;

    public sealed class Config : IConfig
    {
        [Description("Включен ли плагин?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Включен ли дебаг-мод?")]
        public bool Debug { get; set; } = false;

        [Description("Конфиг встроенного плагина XPSystem..")]
        public XPSystem XPSystem { get; set; } = new XPSystem();
    }
}