using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class AppVersionInfo
    {
        public PlatformType Platform;

        public string AppVersion;
        public int ServerVersion;
        public int PhotonVersion;
        public int StaticVersion;
        public int DebugStaticVersion;
        public LocalizationVersionsInfo LocalizationVersions;

        public bool IsOutdated;
        public bool IsPreferUpdate;

        public AppVersionInfo(PlatformType platform, string appVersion, int serverVersion, int photonVersion, bool isOutdated, bool isPreferUpdate, int staticVersion,
            LocalizationVersionsInfo localizationVersions, int debugStaticVersion)
        {
            Platform = platform;
            PhotonVersion = photonVersion;
            AppVersion = appVersion;
            IsOutdated = isOutdated;
            StaticVersion = staticVersion;
            LocalizationVersions = localizationVersions;
            ServerVersion = serverVersion;
            IsPreferUpdate = isPreferUpdate;
            DebugStaticVersion = debugStaticVersion;
        }
    }

    public struct LocalizationVersionsInfo
    {
        public int Ru;
        public int Eng;
    }
}
