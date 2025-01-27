using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Buff
    {
        public string Id;

        public long Time;

        public long NextCheck;

        public BuffType Type;

        public List<BuffValue> Values;

        public class BuffValue
        {
            public BuffValueType Type;
            public float Value;
        }

        public Buff()
        {

        }

        public Buff(Dictionary<BuffValueType, float> dic)
        {
            Values = new List<BuffValue>();
            foreach (var f in dic)
            {
                Values.Add(new BuffValue{ Type = f.Key, Value = f.Value});
            }
        }
    }

}