namespace Сontrol
{
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class Config : IConfig
    {
        [Description("Включен ли плагин?")]
        public bool IsEnabled { get; set; } = true;
        [Description("Включен ли дебаг-мод?")]
        public bool Debug { get; set; } = false;
        [Description("Содержимое CASSIE.")]
        public string StartCassie { get; set; } = "Внимание! CD<b></b>-01 нестабильно. Предупреждение Классу D<b></b> - если вы покинете свою камеру тогда вы подлежите уничтожению.<color=#ffffff00>h pitch_1.06 . Attention . pitch_0.96 room C D 0 1 is pitch_0.93 unstable . warning for ClassD . if you leave your chamber then you terminated . .g6 </color>";
        [Description("Сколько секунд нужно подождать чтобы CASSIE запустилось?")]
        public float WaitingTimeToCassie { get; set; } = 1.5f;
        [Description("Сколько секунд нужно подождать чтобы CASSIE (О Н.У.С.) запустилось?")]
        public float WaitingTimeToCassieContaimentBreach { get; set; } = 31.5f;
        [Description("Содержимое CASSIE (О Н.У.С.) в начале раунда.")]
        public string StartCassieContaimentBreach { get; set; } = "Опасность! Обнаружено нарушение условий содержания в лёгкой зоне содержания. SCP-173<b></b> за пределами камеры. Приказ Сотрудникам Безопасности эвакуировать сотрудников с 0<b></b> уровнем допуска или больше из <color=#6aa84f>Лёгкой зоны содержания</color>.<color=#ffffff00>h pitch_1 . .g6 Danger . detected containment breach in light containment zone . SCP 1 7 3 outside the chamber . order for security personnel to evacuate personnel with 0 or more access level from the light containment zone . .g1 . .g6 </color>";
        [Description("Сколько секунд нужно подождать чтобы двери д-класса открылись?")]
        public float WaitingTimeToOpenPrisonDoors { get; set; } = 2.5f;

        [Description("Какой шанс спавна J-класса? (от 0 до 10)")]
        public int ChanceToSpawnJClass { get; set; } = 9;
        [Description("Какой шанс спавна менеджера комплекса? (от 0 до 10)")]
        public int ChanceToSpawnManager { get; set; } = 9;
        [Description("Имена научных сотрудников..")]
        public string[] namesOfScientists { get; set;  } = new string[] { "Доктор \"Олег\"", "Доктор \"Снорк\"" };
        [Description("Имена менеджера комплекса..")]
        public string[] nameOfManager { get; set; } = new string[] { "Олег", "Алексей", "Кирилл" };
        [Description("Кастомное инфо менеджера комплекса..")]
        public string CustomInfoOfManager { get; set; } = "Менеджер комплекса..";
        [Description("Список вещей, которые имеются у менеджера комплекса..")]
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