using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataGetAddress : SendData
    {
        public string Version;
        public PlatformType Platform;

        public RequestDataGetAddress(string version, PlatformType platform)
        {
            Version = version;
            Platform = platform;
        }
    }
}