namespace Control.Handlers.Events.API
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using Control.Handlers.Events.API.Serialization;
    using Exiled.API.Features;
    using Exiled.Loader.Features;
    using Сontrol;
    using Badge = Features.Badge;

    public static class API
    {
        public static bool TryGetLog(string id, out PlayerLog log)
        {
            log = ControlNR.Singleton.XPdb.GetCollection<PlayerLog>("Players")?.FindById(id);
            return log != null;
        }

        public static void UpdateBadge(Player ply, string i = null, bool hidden = false)
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

                foreach (var kvp in ControlNR.Singleton.Config.XPSystem.LevelsBadge.OrderBy(kvp => kvp.Key))
                {
                    if (log.LVL > kvp.Key && ControlNR.Singleton.Config.XPSystem.LevelsBadge.OrderByDescending(kvp2 => kvp2.Key).ElementAt(0).Key != kvp.Key)
                        continue;
                    badge = new Badge()
                    {
                        Color = $"{kvp.Value.Color}",
                    };
                    break;
                }

                Log.Info(hidden);

                string text = ply.Group == null || hidden
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