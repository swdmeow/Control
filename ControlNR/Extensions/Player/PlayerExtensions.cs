using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using Interactables.Interobjects.DoorUtils;
using MEC;

namespace Control.Extensions
{
    public static class PlayerExtensions
    {
        public static CoroutineHandle? HintCoroutineHandle = null;
        public static CoroutineHandle? WriteHintCoroutineHandle = null;
        public static bool HasKeycardPermission(this Player player, KeycardPermissions permissions, bool requiresAllPermissions = false)
        {
            return requiresAllPermissions ?
                player.Items.Any(item => item is Keycard keycard && keycard.Permissions.HasFlag(permissions))
                : player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & permissions) != 0);
        }
         // 1
        internal static Dictionary<Player, List<(float, string)>> _hintQueue = new Dictionary<Player, List<(float, string)>>();
        public static IEnumerator<float> HintCoroutine()
        {
            for (; ; )
            {
                for (int i = 0; i < _hintQueue.Count; i++)
                {
                    if (_hintQueue.Count == 0 || i >= _hintQueue.Count)
                    {
                        break;
                    }
                    var kvp = _hintQueue.ElementAt(i);
                    bool display = true;
                    string hint = "";
                    if (kvp.Value.Count == 0)
                    {
                        _hintQueue.Remove(kvp.Key);
                        display = false;
                    }
                    for (int index = 0; index < kvp.Value.Count; index++)
                    {
                        if (kvp.Value.Count == 0 || index >= kvp.Value.Count)
                        {
                            display = false;
                            break;
                        }
                        var itemVar = kvp.Value[index];
                        hint += itemVar.Item2;
                        hint += "\n";
                        itemVar.Item1 -= .1f;
                        if (itemVar.Item1 <= 0)
                            _hintQueue[kvp.Key].RemoveAt(index);
                        else
                            _hintQueue[kvp.Key][index] = itemVar;
                    }
                    if (!display)
                        continue;
                    string hintNew = "";
                    foreach (var var in hint.Split('\n'))
                    {
                        hintNew += $"{var}\n";
                    }
                    kvp.Key.ShowHint(hintNew, 0.6f);
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
        public static void ShowCustomHint(this Player ply, string text, float time)
        {
            if (_hintQueue.TryGetValue(ply, out var list))
            {
                list.Add((time, text));
                return;
            }
            _hintQueue.Add(ply, new List<(float, string)>()
            {
                (time, text)
            });
        }
        public static IEnumerator<float> WriteHint()
        {
            for (; ; )
            {
                foreach (Player pl in Player.List.Where(x => !x.IsScp && !CustomRole.Get((uint)1).Check(x) && x.IsAlive))
                {
                    if (!pl.CurrentHint.Content.StartsWith("<b></b>")) { Log.Debug(pl.CurrentHint.Content); Log.Debug("Dont pass"); continue; }

                    string Hint = "<b></b><size=66%>";

                    Hint += "<br><br><br><br><br><br><b>[R<color=#0000ff>U</color><color=#ff0000>S</color>] <color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b><br>";

                    Hint += "</size>";
                    Log.Info(Hint);
                    pl.ShowHint(Hint, 0.6f);
                }

                foreach (Player pl in Player.List.Where(x => x.IsScp || CustomRole.Get((uint)1).Check(x)))
                {
                    if (!pl.CurrentHint.Content.StartsWith("<b></b>")) { Log.Debug(pl.CurrentHint.Content);  Log.Debug("Dont pass"); continue; }

                    string Hint = "<b></b><size=75%>";

                    Hint += "<b>[R<color=#0000ff>U</color><color=#ff0000>S</color>] <color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b><br>";

                    foreach (Player AddTarget in Player.List.Where(x => x.IsScp || CustomRole.Get((uint)1).Check(x)))
                    {
                        Hint += $"<pos=55%><color=red>{AddTarget.Role.Name} - {AddTarget.Health}/{AddTarget.MaxHealth}</color><br>";
                    }

                    Hint += "</size></color>";
                    Log.Info(Hint);
                    pl.ShowHint(Hint, 0.6f);
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}