using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using AMTServer.Common;
using AMTServer.Dto;
using AMTServerDLL.Core;
using MongoDB.Driver;
namespace AMTServer.Core
{
    public class LoginClient
    {
        public ServerInfo Server;
        public ClientHandler Client;

        private Thread _loopThread;
        private Action<string,bool> _onLog;
        private Action<LoginClient> _onDestroy;
        private DataBaseLinks _links;
        public bool IsDestroyed;


        public LoginClient(ServerInfo server, DataBaseLinks links)
        {
            Server = server;
            _links = links;
        }

        public void Init(Action<string,bool> log, Action<LoginClient> onDestroy)
        {
            Client = new ClientHandler(new IPAddress(Server.Address.GetByteIp()), Server.Address.Port);

            _onDestroy = onDestroy;
            _onLog = log;

            _loopThread = new Thread(Loop);
            _loopThread.Start();
        }

        public void Log(string s, bool b)
        {
            var v = "[LoginClient] " + s;
            _onLog?.Invoke(v,b);
        }

        public void Loop()
        {
            var disconnectedTime = 0;
            var checkTimer = 0;
            var outDatedInfo = false;

            while (true)
            {
                Thread.Sleep(1000);

                checkTimer++;

                if (checkTimer > 10)
                {
                    var server = _links.ServerCollections.FindSync(x => x.Name == Server.Name).FirstOrDefault();
                    if (server == null)
                    {
                        Destroy();
                        return;
                    }
                    else
                    {
                        outDatedInfo = Utils.GetUnixTime() > server.LastCallBack + 60;
                    }

                    checkTimer = 0;
                }

                if (!Client.IsConnected || outDatedInfo)
                {
                    disconnectedTime++;

                    if (disconnectedTime > 60 || outDatedInfo)
                    {
                        Log("Server not response, try restart " + Server.Name, true);

                        if (RestartServer())
                        {
                            disconnectedTime = 0;
                            outDatedInfo = false;
                        }
                        else
                        {
                            Destroy();
                        }
                    }
                    
                }
            }
        }

        public bool RestartServer()
        {
            Process tempProc = null;
            try
            {
                tempProc = Process.GetProcessById(Server.ProcessId);
            }
            catch (Exception e)
            {
                Log("Process already destroyed", false);
                tempProc = null;
            }

            if (tempProc != null)
            {
                tempProc.Kill();
                tempProc.WaitForExit();
            }

            var vPath = Server.FullPath.Replace(@"\", @"\\");

            try
            {
                tempProc = Process.Start(vPath);

                Server.ProcessId = tempProc.Id;

                Log("Server restarted " + Server.Name, false);

                Thread.Sleep(10000);

            }
            catch (Exception e)
            {
                Log("Cant start server " + Server.Name + "[" + vPath + "]" + e, false);
                Destroy();
                return false;
            }

            return true;
        }

        public void Destroy()
        {
            if (IsDestroyed)
                return;

            IsDestroyed = true;

            _onDestroy?.Invoke(this);

            _loopThread?.Abort();

            _onDestroy = null;
            _onLog = null;

            if (Client != null)
            {
                Client.OnEvent -= Client.OnEvent;
                Client.Destroy();
            }

            

        }
    }
}
