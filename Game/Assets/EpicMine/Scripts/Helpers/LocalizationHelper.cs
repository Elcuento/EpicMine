using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class LocalizationHelper
    {
        private static LocalizationData BuildInRuLocalizationData =>
            _buildInRuLocalizationData ?? (_buildInRuLocalizationData = Resources.Load<TextAsset>("FileData/Language/BuildInLocalization/Russian")
                .text
                .FromJson<LocalizationData>());

        private static LocalizationData BuildInEngLocalizationData =>
            _buildInEngLocalizationData ?? (_buildInEngLocalizationData = Resources.Load<TextAsset>("FileData/Language/BuildInLocalization/English")
                .text
                .FromJson<LocalizationData>());

        private static LocalizationData _buildInRuLocalizationData;
        private static LocalizationData _buildInEngLocalizationData;

        private static readonly Dictionary<SystemLanguage, string> _countryCodes = new Dictionary<SystemLanguage, string>()
        {
            { SystemLanguage.Afrikaans, "ZA" },
            { SystemLanguage.Arabic    , "SA" },
            { SystemLanguage.Basque    , "US" },
            { SystemLanguage.Belarusian    , "BY" },
            { SystemLanguage.Bulgarian    , "BJ" },
            { SystemLanguage.Catalan    , "ES" },
            { SystemLanguage.Chinese    , "CN" },
            { SystemLanguage.Czech    , "HK" },
            { SystemLanguage.Danish    , "DK" },
            { SystemLanguage.Dutch    , "BE" },
            { SystemLanguage.English    , "EN" },
            { SystemLanguage.Estonian    , "EE" },
            { SystemLanguage.Faroese    , "FU" },
            { SystemLanguage.Finnish    , "FI" },
            { SystemLanguage.French    , "FR" },
            { SystemLanguage.German    , "DE" },
            { SystemLanguage.Greek    , "JR" },
            { SystemLanguage.Hebrew    , "IL" },
            { SystemLanguage.Icelandic    , "IS" },
            { SystemLanguage.Indonesian    , "ID" },
            { SystemLanguage.Italian    , "IT" },
            { SystemLanguage.Japanese    , "JP" },
            { SystemLanguage.Korean    , "KR" },
            { SystemLanguage.Latvian    , "LV" },
            { SystemLanguage.Lithuanian    , "LT" },
            { SystemLanguage.Norwegian    , "NO" },
            { SystemLanguage.Polish    , "PL" },
            { SystemLanguage.Portuguese    , "PT" },
            { SystemLanguage.Romanian    , "RO" },
            { SystemLanguage.Russian    , "RU" },
            { SystemLanguage.SerboCroatian    , "SP" },
            { SystemLanguage.Slovak    , "SK" },
            { SystemLanguage.Slovenian    , "SI" },
            { SystemLanguage.Spanish    , "ES" },
            { SystemLanguage.Swedish    , "SE" },
            { SystemLanguage.Thai    , "TH" },
            { SystemLanguage.Turkish    , "TR" },
            { SystemLanguage.Ukrainian    , "UA" },
            { SystemLanguage.Vietnamese    , "VN" },
            { SystemLanguage.ChineseSimplified    , "CN" },
            { SystemLanguage.ChineseTraditional    , "CN" },
            { SystemLanguage.Unknown    , "US" },
            { SystemLanguage.Hungarian    , "HU" },
        };

        public static string ToCountryCode(string language)
        {
            if (!Enum.TryParse<SystemLanguage>(language, out var languageEnum))
            {
                languageEnum = SystemLanguage.English;
            }

            if (_countryCodes.TryGetValue(languageEnum, out var result))
            {
                return result;
            }
            else
            {
                return _countryCodes[SystemLanguage.English];
            }
        }

        public static string ToCountryCode(SystemLanguage language)
        {
            if (_countryCodes.TryGetValue(language, out var result))
            {
                return result;
            }
            else
            {
                return _countryCodes[SystemLanguage.English];
            }
        }

        public static SystemLanguage GetCurrentLanguage()
        {
            return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), App.Instance.CurrentLocalizationData.Code);
        }

        public static int GetLastVersion(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Russian:
                    return App.Instance.VersionInfo.LocalizationVersions.Ru;
                default:
                    return App.Instance.VersionInfo.LocalizationVersions.Eng;
            }
        
        }
        public static string GetCountryCode()
        {
            return _countryCodes.ContainsKey(Application.systemLanguage) ? 
                _countryCodes[Application.systemLanguage] : _countryCodes[SystemLanguage.English];
        }

        public static  SystemLanguage GetSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Russian:
                     return SystemLanguage.Russian;
                 default:
                     return SystemLanguage.English;
            }
        }

        public static string GetBuildInLocale(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "Invalid localization key";

            key = key.ToLower();

            LocalizationData buildInLocalizationData = null;
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                    buildInLocalizationData = BuildInRuLocalizationData;
                    break;
                default:
                    buildInLocalizationData = BuildInEngLocalizationData;
                    break;
            }

            var localizationString = buildInLocalizationData.Values.FirstOrDefault(l => l.Key == key);

            if (localizationString == null)
                return string.Format("No localization for key: " + key);

            var text = localizationString.Text;
            text = text.Replace("\\n", "\n");
            return text;
        }

        public static bool IsLocaleExist(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            key = key.ToLower();
            var localizationString = App.Instance.CurrentLocalizationData?.Values?.FirstOrDefault(l => l.Key == key);
            if (localizationString == null)
            {
                LocalizationData buildInLocalizationData = null;
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Russian:
                        buildInLocalizationData = BuildInRuLocalizationData;
                        break;
                    default:
                        buildInLocalizationData = BuildInEngLocalizationData;
                        break;
                }

                localizationString = buildInLocalizationData.Values.FirstOrDefault(l => l.Key == key);
            }

            if (localizationString == null)
                return false;

            return true;
        }


        public static string GetLocale(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "Invalid localization key";

            key = key.ToLower();
            var localizationString = App.Instance.CurrentLocalizationData?.Values?.FirstOrDefault(l => l.Key == key);
            if (localizationString == null)
            {
                LocalizationData buildInLocalizationData = null;
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Russian:
                        buildInLocalizationData = BuildInRuLocalizationData;
                        break;
                    default:
                        buildInLocalizationData = BuildInEngLocalizationData;
                        break;
                }

                localizationString = buildInLocalizationData.Values.FirstOrDefault(l => l.Key == key);
            }

            if (localizationString == null)
                return string.Format("No localization for key: " + key);

            var text = localizationString.Text;
            text = text.Replace("\\n", "\n");
            return text;
        }

        public static string GetLocaleCurrency(decimal amount)
        {
            return $"{amount:C2} руб.";
            /*switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                    return $"{amount:C2} руб.";
                default:
                    return $"${amount}";
            }*/
        }
    }
}