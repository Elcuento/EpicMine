using EpicMineServerDLL.Static.Enums;

namespace AMTServerDLL.Dto
{
    public class ResponseDataGetServerAddress : SendData
    {
        public string Ip;
        public int Port;
        public ServerAddressType Type;

        public ResponseDataGetServerAddress(string ip, int port, ServerAddressType type)
        {
            Ip = ip;
            Port = port;
            Type = type;
        }

    }
}