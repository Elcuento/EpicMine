using CommonDLL.Static;

namespace AMTServerDLL.Dto
{

    public class RequestDataReLogin : SendData
    {
        public string UserId;
        public PlatformType Platform;
        public string Version;

        public RequestDataReLogin(string userId, string version, PlatformType platform = PlatformType.Android)
        {
            UserId = userId;
            Version = version;
            Platform = platform;
        }
    }
}