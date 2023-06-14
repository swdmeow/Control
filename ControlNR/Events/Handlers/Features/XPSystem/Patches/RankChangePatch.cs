namespace Control.Handlers.Events.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using API = API.API;

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetText))]
    public class RankChangePatch
    {
        internal static void Prefix(ServerRoles __instance, string i)
        {
            if (i != null && !i.Contains("\n"))
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Player pl = Player.Get(__instance._hub);
                    if(__instance.GlobalHidden == true) API.UpdateBadge(Player.Get(__instance._hub), i, true);
                    else
                    {
                        API.UpdateBadge(Player.Get(__instance._hub), i, false);
                    }
                });
            }
        }
    }
}
