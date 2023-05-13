﻿using System;
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
    public static class HintExtensions
    {
        public static CoroutineHandle? WriteHintCoroutineHandle = null;
        public static IEnumerator<float> WriteHint()
        {
            for (; ; )
            {
                foreach (Player pl in Player.List)
                {
                    if (pl.CurrentHint != null)
                    {
                        if (!pl.CurrentHint.Content.StartsWith("<b></b>"))
                        {
                            continue;
                        }
                    }

                    string Hint = "<b></b>";

                    if (!pl.IsScp && !CustomRole.Get((uint)1).Check(pl))
                    {
                        Hint += "<size=66%>";
                        Hint += "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><b><color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b><br>";

                        Hint += "</size>";
                        pl.ShowHint(Hint, 0.7f);
                    }

                    if (pl.IsScp || CustomRole.Get((uint)1).Check(pl))
                    {
                        Hint += "<size=59%><pos=75%>";

                        Hint += "<b><color=#8DFF29>bezname</color> | <color=#00B7EB>NoRules</color></b></size><br>";

                        foreach (Player AddTarget in Player.List.Where(x => x.IsScp || CustomRole.Get((uint)1).Check(x)))
                        {
                            Hint += $"<size=75%><pos=75%><color=red>{(AddTarget.IsScp ? AddTarget.Role.Name : "SCP-035")} - {(AddTarget.HumeShieldStat.CurValue > 0 ? $"</color><color=#3A3679>{Math.Ceiling(AddTarget.Health + AddTarget.HumeShieldStat.CurValue)}</color>" : $"</color><color=red>{(AddTarget.Health == 0 ? Generator.List.Where(x => x.IsEngaged).Count() : Math.Ceiling(AddTarget.Health))}</color>")}/{(AddTarget.HumeShieldStat.CurValue > 0 ? $"</color><color=#3A3679>{Math.Ceiling(AddTarget.MaxHealth + AddTarget.HumeShieldStat.MaxValue)}</color>" : $"</color><color=red>{(AddTarget.Health == 0 ? Generator.List.Count() : Math.Ceiling(AddTarget.MaxHealth))}</color>")}</color></size><br>";
                        }

                        Hint += "</size></color>";
                        pl.ShowHint(Hint, 0.7f);
                    }
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}