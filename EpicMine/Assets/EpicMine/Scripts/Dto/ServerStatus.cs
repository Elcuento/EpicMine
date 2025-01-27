using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Dto
{
    public struct ServerStatus
    {
        public bool Enable;
        public List<ServerInfo> Redirects;
    }

    public struct ServerInfo
    {
        public string Platform;
        public string Version;
        public ServerType Server;
    }
}