using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using AMTServer.Common;
using AMTServer.Core;
using AMTServer.Dto;
using AMTServerDLL.Core;
using AMTServerDLL.Dto;
using EpicMineServerDLL.Dto;
using EpicMineServerDLL.Static.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace AMTServer
{
    public class EpicMineServer
    {
        public Action<ClientPeer> OnAddClient;
        public Action<ClientPeer> OnRemoveClient;

        public static EpicMineServer I;

        private static string _configFile = "config.ini";

        public static ConfigClass MainConfig;

        public HashSet<ClientPeer> Peers
        {
            get
            {
                lock (_clients)
                {
                    return _clients;
                }
            }
        }

        public bool IsLoginServer { get; set; }

        public static Thread StatisticTread;

        private ServerHandler _serverHandler;

        private HashSet<ClientPeer> _clients;
        
        private FileSystem _fileArchive;
        private LogSystem _logSystem;
        private StatisticSystem _statisticSystem;
        private RatingSystem _ratingSystem;
        private PvpMatchSystem _matchSystem;
        private DataBaseLinks _links;
        public long LastTimerUpdated;

        public List<ClientHandler> Clients = new List<ClientHandler>();
        
        private System.Timers.Timer _statsTimer;
        private List<ClientQuite> _clientsQuite = new List<ClientQuite>();

        private List<LoginClient> _loginClients = new List<LoginClient>();

        public static void Main(string[] args)
        {
            I = new EpicMineServer();

            var pack = new ConventionPack {new IgnoreExtraElementsConvention(true)};
            ConventionRegistry.Register("My Solution Conventions", pack, t => true);

            I.Main();
        }


        public EpicMineServer()
        {
            _clients = new HashSet<ClientPeer>();


        }

       /* public void SendItems(ClientHandler client)
        {
            var items = new List<Item>();
            for (var i = 0; i < 100; i++)
            {
                // var id = new Random((int) DateTime.Now.Ticks).Next(0, 999999);
                // Console.WriteLine(id);
                items.Add(new Item(Guid.NewGuid().ToString(), 999));
            }

            client.SendRequestNetworkMessage<SendData>(new RequestDataUpdateInventory(items),
                (int)CommandType.UpdateInventory, onComplete: (responseData) =>
                {
                    Count++;
                  
                    Console.WriteLine("Send ok " + Count);

                    client.Destroy();
                    if (Count == CountClients)
                    {
                        Console.Clear();
                        Console.WriteLine("End");

                        foreach (var clientHandler in Clients)
                        {
                            Count = 0;
                            SendItems(clientHandler);
                        }
                    }

                //    client.Destroy();
                });
        
        }
        */


       public void CalculateRating()
       {
           Log("Calculate rating");
            _ratingSystem.Calculate();
       }

       public void QuiteServer()
       {
           Log("Server start closing");
           if (_statsTimer != null)
           {
               _statsTimer.Enabled = false;
           }
       }

       public void ReloadAll()
       {
           Log("Reload all");
           _fileArchive.ReloadAll();
       }

        public void StressTest()
        {
           return;
            Clients = new List<ClientHandler>();

            var a = new Thread(() =>
            {
                var CountClients = 200;
                Console.WriteLine("Clients " + CountClients);
                for (var i = 0; i < CountClients; i++)
                {
                    Thread.Sleep(10);

                    Console.WriteLine("Up " + (int)i);
                    var client = new ClientHandler(new IPAddress(MainConfig.IpAddress), MainConfig.Port);

                    //var client = new ClientHandler(new IPAddress(new byte[] { 194, 87, 248, 57 }), 34565);

                  //  client.Connect();
                  //  client.Subscribe(LogClient);
                 //   client.SetLogType(AMTServerDLL.Utils.LogType.Important);

                    /*client.SendRequestNetworkMessage<SendData>(new RequestDataLogin("0.0.0",Guid.NewGuid().ToString(), "", ""),
                        (int) CommandType.Login, onComplete: (res) =>
                        {
                            Console.WriteLine("Login ok ");
                        });*/

                    Clients.Add(client);
                }


            });
            a.Start();

            var tre = new Thread(() =>
            {
                while (true)
                {
                    if(_serverHandler == null ||
                       _serverHandler.Clients == null)
                        continue;

                    var wait = new Random().Next(0, 2);
                    
                    Thread.Sleep(wait * 1000);
                    lock (_serverHandler.Clients)
                    {
                        if (_serverHandler.Clients.Count > 0)
                        {
                            var peer = new Random().Next(0, _serverHandler.Clients.Count);
                        //    _serverHandler.Kick(_serverHandler.Clients[peer]);
                        }
                    }

                }
            });


            tre.Start();
        }

        public void Main()
        {

           /* try
            {
                var p = new ProcessStartInfo(@"C:\\AMTServer.exe");

                Process.Start(p);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return;
            }
            Console.WriteLine("P[en ok");
            Console.ReadKey();
            return;*/
            Init();

            WriteKeyInfo();

            Log(" Server was started");

            try
            {
                StatisticTread = new Thread(SecondLoop);
                StatisticTread.Start();

                Connect();

                return;

            }
            catch (Exception e)
            {
                Console.WriteLine("Server Start Fail!!!\n" + e.Message);
            }


            StatisticTread?.Abort();
            //s
            Cleanup();

            Console.WriteLine(" Server was cleanup");
        }

        private void Connect()
        {
            Log("Connect");
            try
            {
                _serverHandler = new ServerHandler(new IPAddress(MainConfig.IpAddress), MainConfig.Port);

                _serverHandler.OnConnected += OnConnected;
                _serverHandler.OnDisconnected += OnDisconnected;
                _serverHandler.SetLogType(AMTServerDLL.Utils.LogType.Important);
                _serverHandler.Subscribe(LogServer);
            }
            catch (Exception e)
            {
                Log(e.ToString(), true);
            }
   

            WriteServerInfo();
        }


        private void OnConnected(ServerHandlerClient client)
        {
            lock (Peers)
            {
                var peer = new ClientPeer(this, client, _fileArchive,
                    new DataBaseLinks(MainConfig.DbName, MainConfig.DbIpString), _ratingSystem, _matchSystem);

                Peers.Add(peer);

                OnAddClient?.Invoke(peer);
            }
        }

        private void WriteServerInfo()
        {
            _statsTimer?.Stop();

            _statsTimer = new System.Timers.Timer();

            var id = ObjectId.GenerateNewId();
            _links.ServerCollections.DeleteMany(x => x.Name == MainConfig.ServerName);

            var a = System.Reflection.Assembly.GetEntryAssembly
                ();
            var baseDir = Path.GetDirectoryName(a.Location);

            var processId = Process.GetCurrentProcess().Id;
            var path = baseDir + "\\" + AppDomain.CurrentDomain.FriendlyName;

            LastTimerUpdated = Utils.GetUnixTime();

            _statsTimer.Elapsed += (b, s) =>
            {
               // Log("Start Write", true);
               if (LastTimerUpdated + 30 < Utils.GetUnixTime()) // Dunno ho but sometimes mongo just stop write read anythink.
               {
                    Log("Server cannot write to mongodb!",true);
                    QuiteServer();
                    return;
               }

               LastTimerUpdated = Utils.GetUnixTime();

                try
                {
                    var online = GetOnline();

                    var info = new ServerInfo
                    {
                        Id = id,
                        FullPath = path,
                        ProcessId = processId,
                        Address = new ServerAddress(MainConfig.IpAddress, MainConfig.Port, ServerAddressType.Main),
                        CurrentUsers = online,
                        IsLoginServer = IsLoginServer,
                        LastCallBack = Utils.GetUnixTime(),
                        MaxUsers = MainConfig.MaxConnectionsPerServer,
                        Name = MainConfig.ServerName,
                       
                    };

                    _links.ServerCollections.ReplaceOne(x => x.Id == id, info, new ReplaceOptions
                    {
                        IsUpsert = true
                    });
                }
                catch (Exception e)
                {
                    Log(e.ToString(),true);
                }

           //     Log("End Write", true);
            };


            _statsTimer.Interval = 5000;
            _statsTimer.Enabled = true;

        }

        public int GetOnline()
        {
            lock (Peers)
            {
                return Peers.Count;
            }
        }
        private void OnDisconnected(ServerHandlerClient client, string reason)
        {
            lock (Peers)
            {
                var peer = Peers.FirstOrDefault(x => x.ClientHandler == client);

                if (peer != null && !peer.IsDestroyed)
                {
                    Kick(peer, reason);
                }

                Peers.Remove(peer);

                OnRemoveClient?.Invoke(peer);
            }
        }

        private void Init()
        {
            var path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\");
            
            if (File.Exists(path + _configFile))
            {
                try
                {
                    MainConfig = new ConfigClass(File.ReadAllLines(path + _configFile));
                    IsLoginServer = MainConfig.IsLoginServer;
                }
                catch (Exception e)
                {
                    MainConfig = ConfigClass.GetDefault();
                }

            }
            else
            {
                MainConfig = ConfigClass.GetDefault();
            }

            _logSystem = new LogSystem(path);
            _fileArchive = new FileSystem(path, MainConfig);
            _ratingSystem = new RatingSystem(this, _fileArchive, new DataBaseLinks(MainConfig.DbName, MainConfig.DbIpString));
            _links = new DataBaseLinks(MainConfig.DbName, MainConfig.DbIpString);
            _matchSystem = new PvpMatchSystem(path,_fileArchive, this, new DataBaseLinks(MainConfig.DbName, MainConfig.DbIpString));
            _statisticSystem = new StatisticSystem(path, this, MainConfig, _fileArchive, _ratingSystem, _matchSystem);

            _matchSystem.PostInitialize();
            _ratingSystem.PostInitialize();
            _statisticSystem.PostInitialize();

            ClearResponses();
            ClearResponsesThread();

            Log("Server Initialized");
            Log("Server Address \"" + string.Join(".", MainConfig.IpAddress) + ":" + MainConfig.Port + "\"");
            Log("Login server : " + MainConfig.IsLoginServer);
            
            ClearOldUsers();
        }

        private void ClearOldUsers()
        {
            var path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\");

            if (File.Exists(path + _configFile))
            {
                try
                {
                    MainConfig = new ConfigClass(File.ReadAllLines(path + _configFile));
                    IsLoginServer = MainConfig.IsLoginServer;
                }
                catch (Exception)
                {
                    MainConfig = ConfigClass.GetDefault();
                }

            }
            else
            {
                MainConfig = ConfigClass.GetDefault();
            }

            var thread = new Thread(() =>
            {
                Log("Start Clear old users afk more than 60 days", true);

                var link = new DataBaseLinks(MainConfig.DbName, MainConfig.DbIpString);

                var endDate = Utils.GetUnixTime() - 60 * 24 * 60 * 60;

                link.UserCollection.DeleteMany(x => x.LastOnlineDate < endDate);

                Log("Clear old users complete ", true);

            });

            thread.Start();
        }


        public void SendResponseLessMessageToOther(string userId, SendData data, CommandType command)
        {
            lock (Peers)
            {
                var peer = Peers.FirstOrDefault(x => x.Player != null && x.Player.Data.Id == userId);

                peer?.SendResponseLessNetworkMessage(data, command);
            }
        }

        /*public void SendMessageToOther(string userId, SendData data, CommandType command)
        {
            lock (Peers)
            {
                var peer = Peers.FirstOrDefault(x => x.Player != null && x.Player.Data.Id == userId);

                peer?.SendResponseLessNetworkMessage(data, command);
            }

        }*/
        /*public void LoadResponseAssembly()
        {
            try
            {
                var result = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Name == typeof(Response<>).Name);

                foreach (var type in result)
                {
            
                //    if (type.IsGenericTypeDefinition)
                    {
                        //  Console.WriteLine("N" + type.Name);

                     //   var ob = Activator.CreateInstance(type);
                        // Console.WriteLine("B" + type.BaseType);

                        Console.WriteLine("C" + type.GetField("Command").GetValue(ob));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }*/


        public void Log(string log, bool isError = false)
        {
            LogSystem.Log($"[EpicMineServer]" + log, isError);
        }
  
        public void LogServer(string log, bool isError = false)
        {
            LogSystem.Log($"[EpicMineServer]" + log, isError);
        }

        public void SecondLoop()
        {
            while (true)
            {
                Thread.Sleep(1000);

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    var isIntercepted = false;

                    if (key.Key == ConsoleKey.Delete)
                        return;

                    if (key.Key == ConsoleKey.R)
                    {
                        isIntercepted = true;
                        KickAll();
                    }
                    if (key.Key == ConsoleKey.Z)
                    {
                        isIntercepted = true;
                        Cleanup();
                        Connect();
                    }
                    if (key.Key == ConsoleKey.T)
                    {
                        isIntercepted = true;
                        CalculateRating();
                    }
                    if (key.Key == ConsoleKey.Y)
                    {
                        isIntercepted = true;
                        ReloadAll();
                    }
                    if (key.Key == ConsoleKey.Q)
                    {
                        isIntercepted = true;
                        QuiteServer();
                    }
                    if (!isIntercepted)
                    {
                        WriteKeyInfo();
                    }
                }
            }
        }

        public void Kick(ClientPeer peer, string reason)
        {
            _serverHandler?.Kick(peer.ClientHandler, reason);

            lock (Peers)
            {
                peer.Destroy(reason);
                Peers.Remove(peer);
            }
        }

        public void KickAll()
        {
            Log("Kick all");

            _serverHandler?.KickAll();

            lock (Peers)
            {
                foreach (var basePeer in Peers)
                {
                    basePeer.Destroy("Kick all command");
                }

                Peers.Clear();
            }

        }

    

        private static void WriteKeyInfo()
        {
            Console.WriteLine("\n > [Delete] - Stop Server. Close all connections\n"+
                                   " > [R/r] - Kick all\n"+
                                   " > [Z/z] - Restart \n" +
                                   " > [T/t] - Calculate Ratings\n" +
                                   " > [Y/y] - Reload All \n" +
                                   " > [Q/q] - Quite server \n" +
                                   " > [I/i] - Show Server Info Status\n");
        }

        private void Cleanup()
        {

            Console.WriteLine("Clean up");
      
            KickAll();

            _serverHandler.Destroy();

            _serverHandler.OnConnected -= OnConnected;
            _serverHandler.OnDisconnected -= OnDisconnected;

            _serverHandler.UnSubscribe(LogServer);

            _statisticSystem.Dispose();
            _ratingSystem.Dispose();
            _logSystem.Dispose();

            OnAddClient = null;
            OnRemoveClient = null;

            /*  Console.WriteLine("Press <Esc> for exit");
            do
             {
                 if (Console.KeyAvailable)
                 {
                     var c = Console.ReadKey(true);
                     if (c.Key == ConsoleKey.Escape)
                         break;
                 }
                 Thread.Sleep(100);
             } while (true);*/
        }

        public ClientPeer GetClientById(string id)
        {
            lock (Peers)
            {
                return Peers.FirstOrDefault(x => x.Player?.Data != null && x.Player.Data.Id == id);
            }
        }
        public ClientPeer GetClientByNick(string nick)
        {
            lock (Peers)
            {
                return Peers.FirstOrDefault(x => x.Player?.Data?.Nickname != null && x.Player.Data.Nickname.ToLower() == nick.ToLower());
            }
        }

        public List<ServerInfo> GetServersInfo()
        {
            var timeOut = Utils.GetUnixTime() + 60;
            var list = _links.ServerCollections.FindSync(x => !x.IsLoginServer).ToList();
            var listActual = list.Where(x => x.LastCallBack <= timeOut).ToList();

            lock (_loginClients)
            {
                foreach (var serverInfo in list)
                {
                    var loginClientWatcher = _loginClients.Find(x => x.Server.Name == serverInfo.Name);
                    if (loginClientWatcher == null)
                    {
                        loginClientWatcher = new LoginClient(serverInfo, _links);
                        _loginClients.Add(loginClientWatcher);
                        loginClientWatcher.Init(Log, OnDestroyLoginClient);
                        Log("Create login client for server " + serverInfo.Name);
                    }
                }
            }

            if (listActual.Count == 0)
            {
                Log("Cant find proper servers!All servers is down",true);
                return new List<ServerInfo>();
            }

            list = list.OrderBy(x => x.CurrentUsers).ToList();

            return list;
        }

        private void OnDestroyLoginClient(LoginClient c)
        {
            lock (_loginClients)
            {
                _loginClients.Remove(c);
            }
        }

        public void ClearResponsesThread()
        {
            var thread = new Thread(() =>
            {
                var listRemove = new List<string>();

                while (true)
                {
                    var now = Utils.GetUnixTime() - 30 * 60;

                    lock (_clientsQuite)
                    {
                        foreach (var clientQuite in _clientsQuite)
                        {
                            if (clientQuite.TimeExit < now)
                                listRemove.Add(clientQuite.Id);
                        }
                    }

                    var responses = _links.PlayerResponseArchive.FindSync(x => listRemove.Contains(x.PlayerId)).ToList();
                    foreach (var response in responses)
                    {
                        if (response.LastTimeUpdate > now)
                        {
                            listRemove.Remove(response.PlayerId);
                        }
                    }

                    _links.PlayerResponseArchive.DeleteMany(x => listRemove.Contains(x.PlayerId));

                    listRemove.Clear();

                    Thread.Sleep(30 * 1000);
                }


            });

            thread.Start();
        }

        public void ClearResponses()
        {
            var responses = _links.PlayerResponseArchive.FindSync(Builders<PlayerResponseArchive>.Filter.Empty).ToList();

            var listRemove = new List<string>();

            var now = Utils.GetUnixTime() - 30 * 60;

            foreach (var resp in responses)
            {
                if(resp.LastTimeUpdate < now)
                    listRemove.Add(resp.PlayerId);
            }

            _links.PlayerResponseArchive.DeleteMany(x=> listRemove.Contains(x.PlayerId));
        }

        public void RemoveClientQuite(string dataId)
        {
            lock (_clientsQuite)
            {
                var client = _clientsQuite.Find(x => x.Id == dataId);

                if (client != null)
                {
                    _clientsQuite.Remove(client);
                }
            }
        }

        public void AddClientQuite(string dataId)
        {
            lock (_clientsQuite)
            {
                var client = _clientsQuite.Find(x => x.Id == dataId);

                if (client != null)
                {
                    client.TimeExit = Utils.GetUnixTime();
                }
            }
           
        }
    }
}
