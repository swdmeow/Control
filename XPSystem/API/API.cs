using XPSystem.API.Serialization;

namespace XPSystem.API
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using Exiled.API.Features;
    using Badge = Features.Badge;

    public static class API
    {
        public static bool TryGetLog(string id, out PlayerLog log)
        {
            log = Main.Instance.db.GetCollection<PlayerLog>("Players")?.FindById(id);
            return log != null;
        }

        public static void UpdateBadge(Player ply, string i = null)
        {
            try
            {
                if (i != null && i.Contains("\n"))
                {
                    return;
                }
                if (ply == null || ply.UserId == null)
                {
                    return;
                }
                var log = ply.GetLog();

                Badge badge = new Badge()
                {
                    Color = "grey",
                };

                foreach (var kvp in Main.Instance.Config.LevelsBadge.OrderBy(kvp => kvp.Key))
                {
                    if (log.LVL > kvp.Key && Main.Instance.Config.LevelsBadge.OrderByDescending(kvp2 => kvp2.Key).ElementAt(0).Key != kvp.Key)
                        continue;
                    badge = new Badge()
                    {
                        Color = $"{kvp.Value.Color}",
                    };
                    break;
                }

                string text = ply.Group == null || ply.Group != null && ply.BadgeHidden == true
                    ? $"{log.LVL} уровень"
                    : $"{log.LVL} уровень | {ply.Group.BadgeText}";

                text += "\n";

                Log.Info(text);
                ply.RankName = text;
                ply.RankColor = badge.Color;
            }
            catch(System.Exception er)
            {
                Log.Error(er);
            }
        }
    }
}