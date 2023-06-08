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

        [Description("Включен ли полный рестарт после конца каждого раунда?")]
        public bool FullRoundRestart { get; set; } = true;

        [Description("Включен ли спавн лута в комнатах где он по умолчанию не появляется?")]
        public bool RoomLootSpawn { get; set; } = true;

        [Description("Конфиг встроенного плагина XPSystem..")]
        public XPSystem XPSystem { get; set; } = new XPSystem();
    }
}