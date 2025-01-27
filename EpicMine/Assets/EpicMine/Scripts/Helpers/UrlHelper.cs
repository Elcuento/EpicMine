namespace BlackTemple.EpicMine
{
    public static class UrlHelper
    {
        private const string GooglePlayUrl = "https://play.google.com/store/apps/details?id=ru.blacktemple.epicmine";

        private const string AppStoreUrl = "http://itunes.apple.com/app/id1468078559";

        public const string TermsOfUseUrl = "http://www.blacktemple.ru/terms-of-use";

        public const string PrivacyPolicyUrl = "http://www.blacktemple.ru/privacy-policy";

        public static string GetMarketUrl()
        {
#if UNITY_ANDROID
            return GooglePlayUrl;
#elif UNITY_IOS
            return AppStoreUrl;
#endif
            return "";
        }
    }
}