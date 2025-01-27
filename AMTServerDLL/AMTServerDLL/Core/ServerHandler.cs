using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AMTServerDLL.Dto;

namespace AMTServerDLL.Core
{
    public class ServerHandler : IDisposable
    {
        public Action<ServerHandlerClient> OnConnected;
        public Action<ServerHandlerClient, string> OnDisconnected;

        private Socket _socket;

        public List<ServerHandlerClient> Clients { get; } = new List<ServerHandlerClient>();

        private readonly ManualResetEvent AllDone = new ManualResetEvent(false);
        private bool _socketWasDisposed;

        private Thread _mainThread;
        private Thread _checkThread;

        private Action<string,bool> _onLog;
        
        private Utils.LogType _logType;


        public ServerHandler(IPAddress address, int port)
        {
            var localEndPoint = new IPEndPoint(address, port);

            // Create a TCP/IP socket.  
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                Blocking = false,
                NoDelay = true
            };

            _socket.Bind(localEndPoint);
            _socket.Listen(1000);

            Start();
        }

        public void Subscribe(Action<string,bool> op)
        {
            _onLog += op;
        }

        public void UnSubscribe(Action<string, bool> op)
        {
            _onLog -= op;
        }


        protected void Log(string message, Utils.LogType type = Utils.LogType.Rest)
        {
            var mes = "[ServerHandler]" + message;
            var mess = Utils.Log(_logType, mes, type);
            if (!string.IsNullOrEmpty(mess))
            {
                try
                {
                    _onLog?.Invoke(mess, type == Utils.LogType.Errors);

                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void SetLogType(Utils.LogType type)
        {
            _logType = type;
        }


        private void Start()
        {
            _mainThread = new Thread(Loop);
            _mainThread.Start();

            _checkThread = new Thread(CheckStateLoop);
            _checkThread.Start();
        }

        private void Loop()
        {
            while (true)
            {
                if (_socket == null || _socketWasDisposed)
                {
                    Log(_socket +":" + _socketWasDisposed,Utils.LogType.Errors);
                    return;
                }


                AllDone.Reset();

                _socket.BeginAccept(AcceptCallback, _socket);

                AllDone.WaitOne();

                continue;
            }

        }

        private void CheckStateLoop()
        {
            while (true)
            {
                Thread.Sleep(10000);
                lock (Clients)
                {
                    for (var index = 0; index < Clients.Count; index++)
                    {
                        var basePeer = Clients[index];
                        if (!basePeer.Check())
                        {
                            Kick(basePeer, "TimeOut");
                            index--;
                        }
                    }
                }

                Log("Clear gc and old clients", Utils.LogType.Deep);

              //  GC.Collect();
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {

            if (_socketWasDisposed)
                return;
            // Signal the main thread to continue.  
            AllDone.Set();

            // Get the socket that handles the client request.  
            var listener = (Socket)ar.AsyncState;

            var handler = listener.EndAccept(ar);

            var p = new ServerHandlerClient(this, handler, Guid.NewGuid().ToString());

            lock (Clients)
            {
                Clients.Add(p);
            }

            OnConnected?.Invoke(p);
        }

        public void KickAll()
        {
            lock (Clients)
            {
                for (var index = 0; index < Clients.Count; index++)
                {
                    var basePeer = Clients[index];
                    Kick(basePeer, "Kick All");
                    index--;
                }

                /* foreach (var serverHandlerClient in Clients)
                 {
                     OnDisconnected?.Invoke(serverHandlerClient);
                 }

                 Clients.Clear();*/
            }

        }


        public void Kick(ServerHandlerClient peer, string reason)
        {
           // Console.WriteLine("Kick " + reason);
            if (peer != null && !peer.IsDestroyed)
            {
                lock (Clients)
                {
                    lock (peer.Locker)
                    {
                        peer.Destroy(reason);
                        Clients.Remove(peer);
                        peer.UnSubscribe(_onLog);
                        OnDisconnected?.Invoke(peer, reason);

                        Log("[ServerClientHandler] Disconnect " + peer.UserId + " Reason : " + reason, Utils.LogType.Deep);
                    }
                }
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        public void Destroy()
        {
            KickAll();

            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }
                _socket.Close();
                _socket = null;
                _socketWasDisposed = true;
            }

            Dispose();
        }
    }
}
