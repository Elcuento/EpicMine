using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class Buff
    {
        public string Id ;

        public int Filter ;

        public int Priority ;

        public BuffType Type ;

        public Dictionary<BuffValueType, float> Value ;

        public long Time ;

    }
}