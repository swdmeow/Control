#pragma warning disable

namespace RainbowTags
{
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using RainbowTags.Components;
    using UnityEngine;
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;
        public void OnChangingGroup(ChangingGroupEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;

            bool hasColors = TryGetColors(ev.NewGroup?.GetKey(), out string[] colors);

            if (ev.NewGroup != null && ev.Player.Group == null && hasColors)
            {
                RainbowTagController controller = ev.Player.GameObject.AddComponent<RainbowTagController>();
                controller.Colors = colors;
                controller.Interval = plugin.Config.TagInterval;
                return;
            }

            if (!ev.Player.GameObject.TryGetComponent(out RainbowTagController rainbowTagController))
                return;

            if (hasColors)
                rainbowTagController.Colors = colors;
            else
                Object.Destroy(rainbowTagController);
        }

        private bool TryGetColors(string rank, out string[] availableColors)
        {
            availableColors = null;
            return !string.IsNullOrEmpty(rank) && plugin.Config.Sequences.TryGetValue(rank, out availableColors);
        }
    }
}