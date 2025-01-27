
using CommonDLL.Static;
using Newtonsoft.Json;
using UnityEngine;

namespace BlackTemple.EpicMine.Dto
{
    public struct Buff
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("type")]
        public BuffType Type;

        [JsonProperty("time")]
        public long Time;

        [JsonProperty("nextCheck")]
        public long NextCheck;

        public Buff(string id, BuffType type, long date, long nextCheck)
        {
            Id = id;
            Type = type;
            Time = date;
            NextCheck = nextCheck;
        }

        
    }
}