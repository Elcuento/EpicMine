using System.Security.Permissions;
using EpicMineServerDLL.Dto;
using MongoDB.Bson;

namespace AMTServer.Dto
{
    public class ServerInfo
    {
        public ObjectId Id;
        public int ProcessId;
        public string FullPath;
        public string Name;
        public bool IsLoginServer;
        public int MaxUsers;
        public int CurrentUsers;
        public long LastCallBack;
        public ServerAddress Address;
    }
}
