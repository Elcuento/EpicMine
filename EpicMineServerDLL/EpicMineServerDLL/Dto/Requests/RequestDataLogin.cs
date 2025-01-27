using System.IO;
using CommonDLL.Static;

namespace AMTServerDLL.Dto
{

    public class RequestDataLogin : SendData
    {
        public string AppVersion;
        public string DeviceId;
        public string GoogleId;
        public string FaceBookId;
        public string Localate;
        public PlatformType Platform;

        public RequestDataLogin(string appVersion, string deviceId, string googleId, string faceBookId, string localate, PlatformType platform = PlatformType.Android)
        {
            AppVersion = appVersion;
            DeviceId = deviceId;
            GoogleId = googleId;
            FaceBookId = faceBookId;
            Localate = localate;
            Platform = platform;

        }
    }
}