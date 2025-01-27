using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataPvpInfoUpdate : SendData
    {
        public Pvp PvpInfo;

        public ResponseDataPvpInfoUpdate(Pvp pvpInfo)
        {
            PvpInfo = pvpInfo;
        }
    }
}