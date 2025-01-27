using System.Collections.Generic;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class PvpArenaMatchResult
    {
        public Dictionary<string, PvpArenaGameResoultType> Result;

        public PvpArenaMatchResult(Dictionary<string, PvpArenaGameResoultType> result)
        {
            Result = result;
        }


    }
}
