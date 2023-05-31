using System.Collections.Generic;
using Exiled.API.Features;
using XPSystem.API.Serialization;

using Badge = XPSystem.API.Features.Badge;

namespace XPSystem.API
{
    using System.Linq;
    using MEC;

    public static class Extensions
    {
        public static PlayerLog GetLog(this Player ply)
        {
            PlayerLog toInsert = null;
            if (!API.TryGetLog(ply.UserId, out var log))
            {
                toInsert = new PlayerLog()
                {
                    ID = ply.UserId,
                    LVL = 0,
                    XP = 0,
                    Nickname = ply.Nickname,
                };
                Main.Instance.db.GetCollection<PlayerLog>("Players").Insert(toInsert);
            }

            if (log is null)
                return toInsert;
            return log;
        }

        public static void UpdateLog(this PlayerLog log)
        {
            Main.Instance.db.GetCollection<PlayerLog>("Players").Update(log);
        }

        public static void AddXP(this PlayerLog log, int amount, string message = null)
        {
            log.XP += amount;
            Player ply = Player.Get(log.ID);

            int XPPerLevel = Main.Instance.Config.XPPerLevel + (Main.Instance.Config.XPPerNewLevel * log.LVL);

            int lvlsGained = log.XP / XPPerLevel;
            if (lvlsGained > 0)
            {
                log.LVL += lvlsGained;
                log.XP -= lvlsGained * Main.Instance.Config.XPPerLevel;
                if (Main.Instance.Config.ShowAddedLVL && ply != null)
                {
                    Control.Extensions.HintExtensions.XPHintQueue.Add((ply, Main.Instance.Config.AddedLVLHint
                        .Replace("%level%", log.LVL.ToString()), 3));
                }

                ply.RankName = "";
            }
            else if (Main.Instance.Config.ShowAddedXP && ply != null)
            {
                string msg = message == null ? $"+ <color=green>{amount}</color> XP" : message.Replace("%amount%", amount.ToString());
                Control.Extensions.HintExtensions.XPHintQueue.Add((ply, msg, 3));
            }
            log.UpdateLog();
        }
    }
}