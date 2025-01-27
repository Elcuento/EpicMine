using System.Collections.Generic;
using BlackTemple.Common;

namespace BlackTemple.EpicMine.Static
{
    public class Buff
    {
        public string Id { get; }

        public int Filter { get; }

        public int Priority { get; }

        public BuffType Type { get; }

        public Dictionary<BuffValueType, float> Value { get; }

        public long Time { get; }

        public Buff(string id, int filter, int priority, BuffType type, string value, string time)
        {
            Id = id;
            Filter = filter;
            Priority = priority;
            Type = type;
            Value =  Extensions.GetDictionaryBySplitKeyValuePair<BuffValueType, float>(value); //  new Dictionary<BuffValueType, float>();//
            Time = long.Parse(time);
        }
    }
}