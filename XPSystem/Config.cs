using Exiled.API.Features;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Badge = XPSystem.API.Features.Badge;

namespace XPSystem
{
    using Exiled.API.Enums;
    using PlayerRoles;

    public class Config : IConfig
    {
        [Description("Enable plugin?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Show debug messages?")] 
        public bool Debug { get; set; } = false;

        [Description("(You may add your own entries) Role1: Role2: XP player with Role1 gets for killing a person with Role2 ")]
        public Dictionary<RoleTypeId, int> KillXP { get; set; } = new Dictionary<RoleTypeId, int>()
        {
            [RoleTypeId.Scientist] = 200,
            [RoleTypeId.ClassD] = 200,
            [RoleTypeId.FacilityGuard] = 150,
            [RoleTypeId.NtfPrivate] = 200,
            [RoleTypeId.NtfSergeant] = 250,
            [RoleTypeId.NtfCaptain] = 300,
            [RoleTypeId.ChaosMarauder] = 300,
            [RoleTypeId.ChaosConscript] = 250,
            [RoleTypeId.ChaosRepressor] = 200,
            [RoleTypeId.ChaosRifleman] = 150,
            [RoleTypeId.Scp049] = 500,
            [RoleTypeId.Scp0492] = 100,
            [RoleTypeId.Scp106] = 500,
            [RoleTypeId.Scp173] = 500,
            [RoleTypeId.Scp096] = 500,
            [RoleTypeId.Scp939] = 500,
        };

        [Description("How many XP should a player get if their team wins.")]
        public int TeamWinXP { get; set; } = 250;

        [Description("How many XP is required to advance a level.")]
        public int XPPerLevel { get; set; } = 1000;

        [Description("Сколько опыта нужно после каждого нового уровня.")]
        public int XPPerNewLevel { get; set; } = 250;

        [Description("Show a mini-hint if a player gets XP")]
        public bool ShowAddedXP { get; set; } = true;

        [Description("Show a hint to the player if he advances a level.")]
        public bool ShowAddedLVL { get; set; } = true;

        [Description("What hint to show if player advances a level. (if ShowAddedLVL = false, this is irrelevant)")]
        public string AddedLVLHint { get; set; } = "Новый уровень: <color=red>%level%</color>";

        [Description("(You may add your own entries) How many XP a player gets for escaping")]
        public Dictionary<RoleTypeId, int> EscapeXP { get; set; } = new Dictionary<RoleTypeId, int>()
        {
            [RoleTypeId.ClassD] = 500,
            [RoleTypeId.Scientist] = 300
        };

        [Description("(You may add your own entries) Level threshold and a badge. %color%. if you get a TAG FAIL in your console, either change your color, or remove special characters like brackets.")]
        public Dictionary<int, Badge> LevelsBadge { get; set; } = new Dictionary<int, Badge>()
        {
            [0] = new Badge
            {
                Color = "cyan"
            },
            [1] = new Badge
            {
                Color = "orange"
            },
            [5] = new Badge
            {
                Color = "yellow"
            },
            [10] = new Badge
            {
                Color = "red"
            },
            [50] = new Badge
            {
                Color = "lime"
            }
        };
       

        [Description("The structure of the badge displayed in-game. Variables: %lvl% - the level. %badge% earned badge in specified in LevelsBadge. %oldbadge% - base-game badge, like ones specified in config-remoteadmin, or a global badge. can be null.")]
        public string BadgeStructure { get; set; } = "%badge% | %oldbadge%";
        [Description("See above, just for people who dont have a badge. If empty, badgestructure will be used instead.")]
        public string BadgeStructureNoBadge { get; set; } = "%badge%";
        
        [Description("Path the database gets saved to. Requires change on linux.")]
        public string SavePath { get; set; } = Path.Combine(Paths.Configs, @"Players.db");

        [Description("Path the text file for translations get saved to. Requires change on linux.")]
        public string SavePathTranslations { get; set; } = Path.Combine(Paths.Configs, @"xp-translations.yml");
        
        [Description("Override colors for people who already have a rank")]
        public bool OverrideColor { get; set; } = false;
        
        [Description("Size of hints.")]
        public byte HintSize { get; set; } = 100;
        
        [Description("Spacing of the in (horizontal offset)")]
        public short HintSpace { get; set; } = 0;
        
        [Description("Vertical offset of hints.")]
        public byte VOffest { get; set; } = 0;
        
        [Description("Duration of hints.")]
        public float HintDuration { get; set; } = 3;
    }
}
