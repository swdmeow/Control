#pragma warning disable SA1600
#pragma warning disable SA1516
#pragma warning disable SA1507

namespace RainbowTags
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;


        [Description("The time, in seconds, between switching to the next color in a sequence.")]
        public float TagInterval { get; set; } = 0.5f;

        [Description("A collection of group names with their respective color sequences.")]
        public Dictionary<string, string[]> Sequences { get; set; } = new Dictionary<string, string[]>
        {
            ["owner"] = new[]
            {
                "red",
                "orange",
                "yellow",
                "green",
                "blue_green",
                "magenta",
            },
            ["admin"] = new[]
            {
                "green",
                "silver",
                "crimson",
            },
        };
    }
}