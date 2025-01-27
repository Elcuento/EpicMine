using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetVersionInfo : SendData
    {
        public List<AppVersionInfo> Data;

        public ResponseDataGetVersionInfo(List<AppVersionInfo> news)
        {
            Data = news;
        }
    }
}