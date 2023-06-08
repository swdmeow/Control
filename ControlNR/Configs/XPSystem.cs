using Exiled.API.Features;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Badge = Control.Handlers.Events.API.Features.Badge;

namespace Control.Configs
{
    using Exiled.API.Enums;
    using PlayerRoles;

    public sealed class XPSystem
    {

        [Description("(You may add your own entries) Role1: Role2: XP player with Role1 gets for killing a person with Role2 ")]
        public Dictionary<RoleTypeId, int> KillXP { get; set; } = new Dictionary<RoleTypeId, int>()
        {
            [RoleTypeId.Scientist] = 50,
            [RoleTypeId.ClassD] = 50,
            [RoleTypeId.FacilityGuard] = 50,
            [RoleTypeId.NtfPrivate] = 75,
            [RoleTypeId.NtfSergeant] = 100,
            [RoleTypeId.NtfCaptain] = 125,
            [RoleTypeId.ChaosMarauder] = 125,
            [RoleTypeId.ChaosConscript] = 100,
            [RoleTypeId.ChaosRepressor] = 75,
            [RoleTypeId.ChaosRifleman] = 50,
            [RoleTypeId.Scp049] = 500,
            [RoleTypeId.Scp0492] = 50,
            [RoleTypeId.Scp106] = 500,
            [RoleTypeId.Scp173] = 500,
            [RoleTypeId.Scp096] = 500,
            [RoleTypeId.Scp939] = 500,
            [RoleTypeId.NtfSpecialist] = 125,
        };

        [Description("How many XP should a player get if their team wins.")]
        public int TeamWinXP { get; set; } = 100;

        [Description("How many XP is required to advance a level.")]
        public int XPPerLevel { get; set; } = 1000;

        [Description("Сколько опыта нужно после каждого нового уровня.")]
        public int XPPerNewLevel { get; set; } = 500;

        [Description("Show a mini-hint if a player gets XP")]
        public bool ShowAddedXP { get; set; } = true;

        [Description("Show a hint to the player if he advances a level.")]
        public bool ShowAddedLVL { get; set; } = true;

        [Description("What hint to show if player advances a level. (if ShowAddedLVL = false, this is irrelevant)")]
        public string AddedLVLHint { get; set; } = "Новый уровень: <color=red>%level%</color>";

        [Description("(You may add your own entries) How many XP a player gets for escaping")]
        public Dictionary<RoleTypeId, int> EscapeXP { get; set; } = new Dictionary<RoleTypeId, int>()
        {
            [RoleTypeId.ClassD] = 250,
            [RoleTypeId.Scientist] = 250,
        };

        [Description("(You may add your own entries) Level threshold and a badge. %color%. if you get a TAG FAIL in your console, either change your color, or remove special characters like brackets.")]
        public Dictionary<int, Badge> LevelsBadge { get; set; } = new Dictionary<int, Badge>()
        {
            [0] = new Badge
            {
                Name = "Первопроходец",
                Color = "silver"
            },  
            [1] = new Badge
            {
                Color = "crimson"
            },
            [2] = new Badge
            {
                Color = "aqua"
            },
            [3] = new Badge
            {
                Color = "orange"
            },
            [4] = new Badge
            {
                Color = "lime"
            },
            [5] = new Badge
            {
                Color = "silver"
            },
            [10] = new Badge
            {
                Color = "emerald"
            },
            [15] = new Badge
            {
                Color = "green"
            },
            [50] = new Badge
            {
                Color = "yellow"
            }
        };
     
        
        [Description("Override colors for people who already have a rank")]
        public bool OverrideColor { get; set; } = false;
    }
}
