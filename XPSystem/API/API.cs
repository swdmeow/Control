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
                    Log.Info($"Dontpass, ply {ply.UserId}");
                    return;
                }
                if (ply == null || ply.UserId == null)
                {
                    Log.Warn("Not updating role: player null");
                    return;
                }
                Log.Info($"pass, {ply}");
                var log = ply.GetLog();

                Badge badge = new Badge()
                {
                    Name = $"{log.LVL} уровень",
                    Color = "grey",
                };

                foreach (var kvp in Main.Instance.Config.LevelsBadge.OrderBy(kvp => kvp.Key))
                {
                    if (log.LVL > kvp.Key && Main.Instance.Config.LevelsBadge.OrderByDescending(kvp2 => kvp2.Key).ElementAt(0).Key != kvp.Key)
                        continue;
                    badge = new Badge()
                    {
                        Name = $"{log.LVL} уровень",
                        Color = $"{kvp.Value.Color}",
                    };
                    break;
                }


                bool hasGroup = ply.Group == null || string.IsNullOrEmpty(ply.RankName);
                Log.Debug($"i is null {i == null}");
                Log.Debug($"Using i: {hasGroup && !string.IsNullOrEmpty(Main.Instance.Config.BadgeStructureNoBadge)}");
                string text = hasGroup && !string.IsNullOrEmpty(Main.Instance.Config.BadgeStructureNoBadge)
                    ? Main.Instance.Config.BadgeStructureNoBadge
                            .Replace("%lvl%", log.LVL.ToString())
                            .Replace("%badge%", badge.Name)
                    : Main.Instance.Config.BadgeStructure
                        .Replace("%lvl%", log.LVL.ToString())
                        .Replace("%badge%", badge.Name)
                        .Replace("%oldbadge%", string.IsNullOrWhiteSpace(i) ? ply.Group?.BadgeText : i);
                text += "\n";
                string color = badge.Color;

                Log.Info(text);
                ply.RankName = text;
                ply.RankColor = color;
            }
            catch(System.Exception er)
            {
                Log.Error(er);
            }
        }
    }
}