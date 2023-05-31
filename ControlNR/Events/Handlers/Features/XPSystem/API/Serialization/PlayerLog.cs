using System;
using LiteDB;

namespace Control.Handlers.Events.API.Serialization
{
    [Serializable]
    public class PlayerLog
    {
        public int LVL { get; set; }
        public string Nickname { get; set; }

        public int XP { get; set; }
        [BsonId]
        public string ID { get; set; }
    }
}