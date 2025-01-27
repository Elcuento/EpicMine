using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AMTServerDLL;
using CommonDLL.Dto;
using CommonDLL.Static;
using EpicMineServerDLL.Dto;
using EpicMineServerDLL.Static.Enums;

namespace AMTServer.Core
{
    public class FileSystem
    {
        public const string PathStaticData = "/StaticData/";
        public const string PathLanguageData = "/Language/";
        public const string PathBugReport = "/BugReports/";
        public const string PathNewsData = "/News/";

        public StaticData StaticData
        {
            get
            {
                if (_staticData == null)
                    LoadStaticData();

                lock (_staticData)
                {
                    return _staticData;
                }

            }
        }

        public List<News> News
        {
            get
            {
                lock (_news)
                {
                    return _news;
                }
            }
        }

        public List<AppVersionInfo> Versions
        {
            get
            {
                lock (_versions)
                {
                    return _versions;
                }
            }
        }

        public List<ServerAddress> Addresses
        {
            get
            {
                lock (_addresses)
                {
                    return _addresses;
                }
            }
        }

        private Dictionary<string, Dictionary<int, LocalizationData>> Languages;

        private List<AppVersionInfo> _versions;
        private StaticData _staticData;
        private List<News> _news;
        private List<ServerAddress> _addresses;

        private string _path;

        private ConfigClass _config;

        private object _reportFileLocker = new object();

        public FileSystem(string path, ConfigClass config)
        {
            _config = config;

            _path = path;

            _versions = new List<AppVersionInfo>();
            _addresses = new List<ServerAddress>();
            _staticData = new StaticData();
            _news = new List<News>();
            Languages = new Dictionary<string, Dictionary<int, LocalizationData>>();

            LoadAppVersions();
            LoadAddresses();
            LoadStaticData();
            LoadLanguages();
            LoadNews();
        }

        public void Log(Exception e)
        {
            LogSystem.Log("[FileSystem]" + e, true);
        }

        public void Log(string str, bool isError = false)
        {
            LogSystem.Log("[FileSystem]" + str);
        }


        public void ReloadAll()
        {
            LoadAppVersions();
            LoadAddresses();
            LoadStaticData();
            LoadLanguages();
            LoadNews();
        }

        public void LoadAppVersions()
        {
            lock (_versions)
            {
                try
                {
                    _versions = File.ReadAllText(_path + "VersionInfo.txt").FromJson<List<AppVersionInfo>>();
                    Log("Load app version ok");
                }
                catch (Exception e)
                {
                    Log("Load addresses failed", true);
                    throw;
                }
            }
        }

        public void LoadAddresses()
        {
            lock (_addresses)
            {
                try
                {
                    _addresses = File.ReadAllText(_path + "Addresses.txt").FromJson<List<ServerAddress>>();
                    Log("Load addresses ok");
                }
                catch (Exception e)
                {

                    try
                    {
                        _addresses = new List<ServerAddress>
                        {
                            new ServerAddress(_config.IpAddress, _config.Port, ServerAddressType.Main)
                        };

                        File.WriteAllText(_path + "Addresses.txt", _addresses.ToJson());
                        Log("Create new server address");
                    }
                    catch (Exception exception)
                    {
                        Log("Create new server address failed " + exception, true);
                        throw;
                    }

                }
            }
        }

        public void LoadStaticData()
        {
            lock (_staticData)
            {
                var info = Versions.Last();

                if (!File.Exists($"{_path}{PathStaticData}{info.StaticVersion}.json"))
                {
                    Log("Error, cant find static data!",true);
                    return;
                }

                try
                {
                    var fileData = File.ReadAllBytes($"{_path}{PathStaticData}{info.StaticVersion}.json");

                    var decompress = Utils.DecompressToString(fileData);

                    _staticData = Common.Utils.Base64Decode(decompress).FromJson<StaticData>();

                    Log("Load static data ok");
                }
                catch (Exception e)
                {
                    Log("Error on parsing static data ! : " + e, true);
                    throw;
                }
            }

        }

        public void LoadNews()
        {
            lock (_news)
            {
                try
                {
                    var news = File.ReadAllText($"{_path}{PathNewsData}/News.txt");

                    _news = news.FromJson<List<News>>();

                    Log("Load news ok");
                }
                catch (Exception e)
                {
                    Log("Error on parsing news data ! : " + e, true);
                    throw;
                }
            }
           
        }

        public void LoadLanguages()
        {
            lock (Languages)
            {
                try
                {

                    var ruDic = new Dictionary<int, LocalizationData>();
                    var engDic = new Dictionary<int, LocalizationData>();

                    Languages = new Dictionary<string, Dictionary<int, LocalizationData>>()
                    {
                        {"Russian", ruDic},
                        {"English", engDic}
                    };

                    foreach (var appVersionInfo in Versions)
                    {

                        var russianDic =
                            File.ReadAllText($"{_path}{PathLanguageData}/ru/{appVersionInfo.LocalizationVersions.Ru}.txt");

                        var englishDic =
                            File.ReadAllText($"{_path}{PathLanguageData}/eng/{appVersionInfo.LocalizationVersions.Eng}.txt");

                        if (!ruDic.ContainsKey(appVersionInfo.LocalizationVersions.Ru))
                            ruDic.Add(appVersionInfo.LocalizationVersions.Ru, russianDic.FromJson<LocalizationData>());

                        if (!engDic.ContainsKey(appVersionInfo.LocalizationVersions.Eng))
                            engDic.Add(appVersionInfo.LocalizationVersions.Eng, englishDic.FromJson<LocalizationData>());
                    }
                }
                catch (Exception e)
                {
                    Log("Error on parsing languages data ! : " + e, true);
                    throw;
                }
            }
           
        }

        public LocalizationData GetLanguage(AppVersionInfo info, string key)
        {
          //  Console.WriteLine(key);
            lock (Languages)
            {
                try
                {
                    if (key == "Russian")
                    {
                        if (Languages[key].ContainsKey(info.LocalizationVersions.Ru))
                        {
                            return Languages[key][info.LocalizationVersions.Ru];
                        }
                        else return Languages[key].LastOrDefault().Value;
                    }else if (key == "English")
                    {
                        if (Languages[key].ContainsKey(info.LocalizationVersions.Eng))
                        {
                            return Languages[key][info.LocalizationVersions.Eng];
                        }
                        else return Languages[key].LastOrDefault().Value;
                    }
                }
                catch (Exception l)
                {
                    Console.WriteLine(l);
                    throw;
                }

                return new LocalizationData(0,"eng", new List<LocalizationString>());
            }
           
        }

        public AppVersionInfo GetVersion(PlatformType platform, string appVersion)
        {
            lock (Versions)
            {
                foreach (var appVersionInfo in Versions)
                {
                    if (appVersionInfo.AppVersion == appVersion && (appVersionInfo.Platform == platform || appVersionInfo.Platform == PlatformType.All))
                    {
                        return appVersionInfo;
                    }
                }
            }

            return null;
        }


        public List<ServerAddress> GetServerAddress()
        {
            return Addresses;
        }

        public List<News> GetNewsCopy()
        {
            return News.ToJson().FromJson<List<News>>();
        }


        public StaticData GetStaticDataCopy()
        {
            lock (StaticData)
            {
                return StaticData;
                //.ToJson().FromJson<StaticData>();
            }
            
        }

        public void AddBugReport(string deviceId, string peerId, string valueStr)
        {
            lock (_reportFileLocker)
            {
                Common.Utils.CreateDirectoryIfNotExist($"{_path}{PathBugReport}");

                var resStr = $"[{DateTime.UtcNow}]\nPeer:{peerId}\nMessage:" + valueStr + "\n";

                try
                {
                    File.AppendAllText($"{_path}{PathBugReport}/Report_{deviceId}.txt", resStr);
                }
                catch (Exception)
                {
                    File.AppendAllText($"{_path}{PathBugReport}/Report_NoDeviceID.txt", resStr);
                }
            }
        }
    }
}
