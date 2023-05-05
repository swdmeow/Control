namespace Сontrol
{
    using Control;
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class Config : IConfig
    {
        [Description("Включен ли плагин?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Включен ли дебаг-мод?")]
        public bool Debug { get; set; } = false;
    }
}