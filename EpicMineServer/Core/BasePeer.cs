using System;
using AMTServer.Common;
using AMTServer.Core;
using AMTServerDLL.Core;
using AMTServerDLL.Dto;
using MongoDB.Driver;
using Utils = AMTServerDLL.Utils;

namespace AMTServer
{
    public abstract class BasePeer : IDisposable
    {
        public bool IsDestroyed { get; protected set; }

        public ServerHandlerClient ClientHandler { get; protected set; }

        public bool Check()
        {
            /*lock (ReceivedPackages)
            {
                for (var index = 0; index < ReceivedPackages.Count; index++)
                {
                    var pack = ReceivedPackages[index];

                    if (pack.SendTime + 180 > new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeSeconds())
                    {
                       // ReceivedPackages.Remove(pack);
                       // Console.WriteLine("Remove package");
                    }
                }
            }*/

            return ClientHandler.Check();

        }

        protected BasePeer(ServerHandlerClient client)
        {
            ClientHandler = client;
            client.SetLogType(Utils.LogType.Important);
          //  client.Subscribe(OnLog);
            client.OnGetData += OnGetData;
            Log("Created");
        }

        public virtual void Log(string log, bool isError = false)
        {
            LogSystem.Log($"[EpicMineServer][ClientPeer][{ClientHandler.UserId}]{log}", isError);
        }

        protected virtual void OnGetData(Package package)
        {

        }

        public virtual void Destroy(string reason)
        {
            if (IsDestroyed)
                return;

            Log("Destroyed - " + reason);

            IsDestroyed = true;

            if (ClientHandler != null)
            {
                ClientHandler.OnGetData -= OnGetData;
                ClientHandler.UnSubscribe(Log);
                ClientHandler.Destroy(reason);
                ClientHandler = null;
            }

            Dispose();
        }

        public void Dispose()
        {
            ClientHandler?.Dispose();
        }
    }
}
