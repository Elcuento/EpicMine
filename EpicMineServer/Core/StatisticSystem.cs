using System;
using System.IO;
using System.Threading;
using AMTServerDLL;
using EpicMineServerDLL.Static.Enums;

namespace AMTServer.Core
{
    public class StatisticSystem : IDisposable
    {
        public class Statistic
        {
            public int PlayerOnlineMax;
            public int PlayerOnlineCurrent;
            public long UpTime;
        }


        private Statistic _data;
        private int _updatePeriod = 15;
        private int _savePeriod = 60;
        private string _path;

        private FileSystem _fileSystem;
        private RatingSystem _ratingSystem;
        private PvpMatchSystem _matchSystem;
        private EpicMineServer _server;
        private ConfigClass _config;

        private string _address;

        private long _startTime;

        private Thread _updateThread;
        private Thread _saveThread;

        public StatisticSystem(string path, EpicMineServer server, ConfigClass config, FileSystem fileSystem, RatingSystem rating, PvpMatchSystem matchSystem)
        {
            _path = path + "\\Statistic\\";
            _server = server;
            _fileSystem = fileSystem;
            _ratingSystem = rating;
            _config = config;
            _matchSystem = matchSystem;

            server.OnRemoveClient += OnRemoveClient;
            server.OnAddClient += OnAddClient;

            try
            {
                _data = File.ReadAllText(_path + "Statistics.txt").FromJson<Statistic>() ?? new Statistic();

            }
            catch (Exception e)
            {
                _data = new Statistic();
            }
            
            _startTime = Utils.GetUnixTime();

            _data.PlayerOnlineCurrent = 0;
            _data.UpTime = 0;

            _saveThread = new Thread(SaveThread);
            _saveThread.Start();

           _updateThread = new Thread(UpdateThread);
           _updateThread.Start();

        }

        public void PostInitialize()
        {
            _address = string.Join(".", _config.IpAddress) + ":" + _config.Port;
        }

        public void Log(Exception e)
        {
            LogSystem.Log("[StatisticSystem]" + e, true);
        }

        public void Log(string str, bool isError = false)
        {
            LogSystem.Log("[StatisticSystem]" + str);
        }


        private void OnRemoveClient(ClientPeer peer)
        {
            _data.PlayerOnlineCurrent--;
        }

        private void OnAddClient(ClientPeer peer)
        {
            _data.PlayerOnlineCurrent++;

            if (_data.PlayerOnlineCurrent > _data.PlayerOnlineMax)
                _data.PlayerOnlineMax = _data.PlayerOnlineCurrent;
        }

        public void SaveThread()
        {
            while (true)
            {
                Thread.Sleep(_savePeriod * 1000);
                File.WriteAllText(_path + "Statistics.txt",_data.ToJson());
            }
        }

        public void UpdateThread()
        {
            while (true)
            {
                Thread.Sleep(_updatePeriod * 1000);

                _data.UpTime = Utils.GetUnixTime() - _startTime;

                ShowStatistic();
            }
        }

        public void ShowStatistic()
        {

           /* long av = 0;
            long max = 0;

            lock (Peers)
            {
                foreach (var basePeer in Peers)
                {
                    if (basePeer.IsDestroyed)
                        continue;

                    var time = Utils.GetUnixTime() - basePeer.ClientHandler.ConnectedTime;
                    av += time;

                    if (time > max)
                        max = time;
                }

                av = av / (Peers.Count == 0 ? 1 : Peers.Count);
            }*/

           Console.Clear();

            Console.WriteLine("\n ***** Server Info Status *****");
            Console.WriteLine(" ***** Address: " + _address);
            Console.WriteLine(" Is Login Server : " + _config.IsLoginServer);

            if (_fileSystem.Addresses.Count > 0)
            {
                var dir = _fileSystem.Addresses.Find(x => x.Type == ServerAddressType.Main);

                if (dir != null)
                {
                    Console.WriteLine(" ***** Redirect to: " + dir.Ip + ":" +
                                      dir.Port);
                }
            }

            Console.WriteLine(" ***** Users count max: " + _data.PlayerOnlineMax);
            Console.WriteLine(" ***** Users count: " + _data.PlayerOnlineCurrent);
            Console.WriteLine(" ***** Pvp match count : " + _matchSystem.MatchCount);
            Console.WriteLine(" ***** Up Time: " + Common.Utils.GetTime(_data.UpTime));

            if (_server.IsLoginServer)
            {
                var spend = Common.Utils.GetTime(_ratingSystem.LastCalculateSpend);
                spend = spend == "" ? "0" : spend;
                Console.WriteLine(" ***** Rating last update time: " + _ratingSystem.LastTimeUpdate.ToLocalTime() + " spend : " + spend);

                if (_ratingSystem.IsUpdating)
                {
                    Console.WriteLine(" ***** Rating is updating...");
                }
                else
                {
                    Console.WriteLine(" ***** Rating next update time " + _ratingSystem.NextUpdateTime.ToLocalTime());
                }
            
            }

            Console.WriteLine("\n ***** Last statistic updated : " + DateTime.Now);

        }

        public void Dispose()
        {
         _updateThread?.Abort();
         _saveThread?.Abort();
        }
    }
}
