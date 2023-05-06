using System;
using LiteDB;

namespace XPSystem.API.Serialization
{
    [Serializable]
    public class PlayerLog
    {
        public bool cooldownRole { get; set; } = false;
        public bool cooldownItem { get; set; } = false;
        public bool cooldownCall { get; set; } = false;
        public bool cooldownVote { get; set; } = false;
        public int GivedTimes { get; set; } = 0;
        public int ForcedTimes { get; set; } = 0;
        public int CallTimes { get; set; } = 0;

        public bool ForcedToSCP { get; set; } = false;

        [BsonId]
        public string ID { get; set; }
    }
}