using AMTServerDLL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AMTServerDLL.Core.Events;

namespace AMTServerDLL.Core
{
    public class ClientHandler : IDisposable
    {
        public Action<EventBase> OnEvent;

        protected internal Socket Socket { get; private set; }

        public bool IsDestroyed { get; private set; }

        public bool IsConnected { get; private set; }

        public bool IsPaused { get; private set; }

        public string UserId { get; private set; }

        protected internal long LastPackageTime;

        internal List<PackagePart> ReceivedPackages = new List<PackagePart>();
        
        internal List<string> AlreadyReceivedPackages = new List<string>();

        protected internal int BufferSize = 1048576; //1048576

        protected internal byte[] Buffer;

        protected internal StringBuilder Sb = new StringBuilder();

        protected internal string LastReceiveCommand = "";

        protected internal string _lastSendPackagePart;

        protected internal bool _lastSendPackageReturn;

        protected internal List<SendOperation> _sendOperations = new List<SendOperation>();

        protected internal Thread _socketThread;

        protected internal Thread _connectStateThread;

        protected internal IPAddress _address;

        protected internal int _port;

        protected internal string _uniqueId;

        protected internal Utils.LogType _logType;

        protected internal Action<string> _onLog;

        public ClientHandler(IPAddress address, int port, string uniqueId = "")
        {
            _uniqueId = string.IsNullOrEmpty(uniqueId) ? Guid.NewGuid().ToString() : uniqueId;

            _address = address;
            _port = port;

            Init();

            Start();
        }


        public void SetPause( bool state )
        {
            IsPaused = state;
        }
     

        public void SetLogType(Utils.LogType type)
        {
            _logType = type;
        }


        public void Subscribe(Action<string> op)
        {
            if (IsDestroyed)
                return;

            _onLog += op;
        }

        public void UnSubscribe(Action<string> op)
        {
            _onLog -= op;
        }


        protected void Log(string message, Utils.LogType type = Utils.LogType.Rest)
        {
            var mess = Utils.Log(_logType, message, type);
            if (!string.IsNullOrEmpty(mess))
            {
                try
                {
                    _onLog?.Invoke(mess);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        protected void Init()
        {
            Buffer = new byte[BufferSize];
            Sb = new StringBuilder();
        }

        protected void Connect()
        {
            if (IsDestroyed)
                return;

            Log("Connecting..." + _address +":"+ _port, Utils.LogType.Important);

            {
             
                try
                {
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    var localEndPoint = new IPEndPoint(_address, _port);


                    Socket.Connect(localEndPoint);
                    LastPackageTime = Utils.GetUnixTime();
                    _lastSendPackageReturn = true;

                    Log("Connected " + _address +":" + _port, Utils.LogType.Important);
                }
                catch (Exception e)
                {
                    Log("Connecting failed.", Utils.LogType.Important);
                    Log("Can't connect : " + e, Utils.LogType.Deep);

                    try
                    {
                        Socket?.Close();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    Socket = null;
                }
            }

        }

        protected void Disconnect()
        {
            if(IsConnected)
                Log("Disconnect", Utils.LogType.Important);
         //  var thread = new Thread(() =>
         {
             try
             {
                 if (Socket != null)
                 {
                     Socket.Shutdown(SocketShutdown.Both);
                     Socket.Disconnect(false);
                     Socket.Close();

                     Log("Disconnected", Utils.LogType.Important);
                 }

                 Socket = null;

                 var isConnected = IsConnected;
                 IsConnected = false;

                 if (isConnected)
                 {
                     OnEvent?.Invoke(new DisconnectedEvent());
                 }

             }
             catch (SocketException e)
             {
                 Log("Disconnect failed : " + e, Utils.LogType.Deep);

                  Socket = null;

                 var isConnected = IsConnected;
                 IsConnected = false;

                 if (isConnected)
                 {
                     OnEvent?.Invoke(new DisconnectedEvent());
                 }

             }
         }

        }

        private void Start()
        {
            _socketThread = new Thread(SocketLoop);
            _socketThread.Start();

            _connectStateThread = new Thread(ConnectStateLoop);
            _connectStateThread.Start();
          
        }

        private void ConnectStateLoop()
        {
            SendOperation pingRequest = null;
            // SendOperation connectRequest = null;

            while (true)
            {
                if (IsDestroyed)
                    return;

                Thread.Sleep(100);

                var instantPing = false;

                if (LastPackageTime + 10 < Utils.GetUnixTime())
                {
                    Disconnect();
                    StopOperation(pingRequest);
                    Connect();
                    instantPing = true;
                }

                if (Socket == null || !Socket.Connected)
                    continue;


                if (((pingRequest == null || pingRequest.IsComplete) && LastPackageTime + 5 < Utils.GetUnixTime()) || instantPing)
                {
                    _lastSendPackageReturn = true;

                    pingRequest = SendRequestNetworkMessage<SendDataPing>(null, SystemCommandType.Ping, 0,
                        Guid.NewGuid().ToString(),
                        (a) =>
                        {
                            UserId = a.UserId;

                            if (!IsConnected)
                            {
                                Log("Auth ok", Utils.LogType.Important);
                                IsConnected = true;
                                OnEvent?.Invoke(new ConnectedEvent());
                            }
                        }, (b) =>
                        {
                            Log("Auth failed " + b, Utils.LogType.Important);
                        });

                }
            }
        }

        private void SocketLoop()
        {
            while (true)
            {
                if (IsDestroyed)
                {
                    Log("destroyed");
                    return;
                }

                Thread.Sleep(100);

                try
                {
                    if (Socket == null || !Socket.Connected || Socket.Available < 0)
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message, Utils.LogType.Errors);
                    continue;
                }


                var data = new byte[0];

                try
                {
                    data = new byte[Socket.Available];
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }


                var bytesRead = 0;

                try
                {
                    bytesRead = Socket.Receive(data);
                }
                catch (Exception e)
                {
                    Log(e.Message,Utils.LogType.Errors);
                    continue;
                }

                if (bytesRead <= 0)
                    continue;
      
                var s = Encoding.UTF8.GetString(data, 0, bytesRead);

                Log("Come message \n" + s, Utils.LogType.Deep);

                Sb.Append(s);
                var endIndex = s.IndexOf(Constants.EndOfMessage, StringComparison.Ordinal);
                if (endIndex < 0)
                {
                    continue;
                }

                s = Sb.ToString().Trim();
                var useLast = s.EndsWith(Constants.EndOfMessage);
                var cmds = s.Split(new[] { Constants.EndOfMessage }, StringSplitOptions.RemoveEmptyEntries);
                var n = cmds.Length;
                var partOfMessage = "";
                for (var i = 0; i < n; i++)
                {
                    if (i + 1 == n)
                    {
                        if (!useLast)
                        {
                            partOfMessage = cmds[i];
                            break;
                        }
                    }
                    Sb.Clear();
                    Sb.Append(cmds[i]);
                    Log("Go On \n" + Sb, Utils.LogType.Deep);
                    TryExecCommand();
                }
                Sb.Clear();
                Sb.Append(partOfMessage);
            }

        }

        private void TryExecCommand()
        {
            var s = Sb.ToString();
            if (s.StartsWith(Constants.SysCmdPrefix))
            {
                LastReceiveCommand = s;
                LastPackageTime = Utils.GetUnixTime();
                ParallelOnReceiveData();
            }
        }

        public void Dispose()
        {
            Socket?.Dispose();
        }

        internal void ParallelOnReceiveData()
        {
            var content = LastReceiveCommand;

            Log("GO NEXT \n " + content, Utils.LogType.Deep);

          //  var thread = new Thread(() =>
          {
              try
              {
                  OnReceiveData(content);
              }
              catch (Exception e)
              {
                  Log("Parallel receive data error : " + e, Utils.LogType.Errors);
              }
          }
          // });

          //  thread.Start();
        }

        protected void OnReceiveData(string data)
        {
            if (data.StartsWith(Constants.SysCmdPrefix))
            {
                ProcessSystemMessage(data.Split(new[] { ':' }, 2)[1]);
            }

        }

        protected void ProcessSystemMessage(string data)
        {
            Log("Process \n" + data, Utils.LogType.Deep);

            PackagePart packagePart = null;

            try
            {
                packagePart = data.FromJson<PackagePart>();
            }
            catch (Exception e)
            {
                Log("ProcessSystemMessage receive data error : " + e, Utils.LogType.Errors);
                return;
            }


            if (packagePart == null)
            {
                Log("ProcessSystemMessage receive data error ");
                return;
            }

            Log("Open package part " + packagePart.Id +":" + ": belong to :" + packagePart.PackageId );

            if (!packagePart.IsResponse)
            {
                SendResponseLessNetworkMessage<SendData>(
                    new SendDataPackagePart(packagePart.Part, packagePart.PackageId, packagePart.Id),
                    SystemCommandType.PackagePart,0);
            }

            Package package = null;

            lock (ReceivedPackages)
            {
                if (ReceivedPackages.Find(x => x.PackageId == packagePart.PackageId
                                               && x.Part == packagePart.Part) != null)
                {
                    return;
                }

                if (packagePart.PartCount == 1)
                {
                    package = Encoding.UTF8.GetString(Utils.Decompress(packagePart.Data)).FromJson<Package>();
                }
                else
                {
                    ReceivedPackages.Add(packagePart);
                    // Console.WriteLine(packagePart.Part);
                    var all = ReceivedPackages.FindAll(x => x.PackageId == packagePart.PackageId);
                    // Console.WriteLine(all.Count +":" + packagePart.PartCount);
                    if (all.Count == packagePart.PartCount)
                    {
                        var order = all.OrderBy(x => x.Part);
                        var array = new List<byte>();

                        foreach (var pack in order)
                        {
                            array.AddRange(pack.Data);
                            ReceivedPackages.Remove(pack);
                        }

                        package = Encoding.UTF8.GetString(Utils.Decompress(array.ToArray())).FromJson<Package>();
                    }
                    else
                    {
                        return;
                    }
                }
            }



            if (package == null)
            {
                Log("Package null");
                return;
            }
            Log("Open package " + package.Id + ":" + package.Type);

            CompleteOperation(package.Id, (SendData)package.Data, false, package.ErrorMessage);

            Log("Command : " + package.Type, Utils.LogType.Rest);

            switch (package.Type)
            {
                case SystemCommandType.PackagePart:
                    ReceivedPackagePart(package);
                    return;

                case SystemCommandType.Ping:
                  //  Ping(package);
                    return;

                case SystemCommandType.Message:
                    OnEvent?.Invoke(new GetDataEvent(package));
                    break;

                default:
                {
                    Log(package.Type +" Not set switch", Utils.LogType.Errors);
                    return;
                }

            }
        }

        private void Ping(Package pack)
        {
            PingResponse(pack);
        }

        private void PingResponse(Package pack)
        {
            SendResponseLessNetworkMessage<SendData>(new SendData(), SystemCommandType.Ping, 0, pack.Id);
        }


        protected void ReceivedPackagePart(Package pack)
        {
            try
            {
                var data = pack.Data as SendDataPackagePart;

                if (data.PackagePartId == _lastSendPackagePart)
                    _lastSendPackageReturn = true;

                lock (_sendOperations)
                {
                    var req = _sendOperations.Find(x => x.Id == data.PackageId);

                    Log("received part " + data.Part + " for " + req?.SystemCommand + ":" + (req == null ? "null" : "ok"));

                    req?.ReceivePart(data.Part);
                }

            }
            catch (Exception e)
            {
                Log(e.Message, Utils.LogType.Errors);
                throw;
            }
        }

        protected Package SendNetworkDataProceed(SendOperation operation, bool noResponse = false, Action onCompleted = null,
            Action onFailed = null)
        {
            var package = new Package(
                Utils.GetUnixTime(),
                operation.Data,
                operation.Id,
                "",
                operation.SystemCommand,
                operation.LocalCommand);


            try
            {
                var sendData = Utils.Compress(Encoding.UTF8.GetBytes(package.ToJson()));

                var str = new List<byte>();

                var partNumb = 0;
                var messages = new List<PackagePart>();

                /* for (var index = 0; index < sendData.Length; index++)
                 {
                     str.Add(sendData[index]);

                     if (str.Count > Constants.MaxPackageSize || index == sendData.Length - 1)
                     {
                         var packagePart = new PackagePart(Guid.NewGuid().ToString(), noResponse, package.SendTime,
                             str.ToArray(), package.Id, partNumb, 0);

                         messages.Add(packagePart);

                         partNumb++;
                         str.Clear();
                     }
                 }*/
                var packagePart = new PackagePart(Guid.NewGuid().ToString(), noResponse, package.SendTime,
                    sendData.ToArray(), package.Id, partNumb, 0);
                messages.Add(packagePart);

                 var max = messages.Count;

                for (var index = 0; index < messages.Count; index++)
                {
                    var message = messages[index];
                    message.SetPartsCount(max);
                }

                operation.SetParts(messages);

                AsyncSendParts(operation, noResponse);
            }
            catch (Exception e)
            {
                Log(e.Message, Utils.LogType.Errors);
                CompleteOperation(operation, null, true, "Client not connected");
                onFailed?.Invoke();
                return package;
            }


            onCompleted?.Invoke();

            return package;
        }


        protected void AsyncSendParts(SendOperation operation, bool noResponse = false)
        {
            Log("Start Async Send part " + operation.SystemCommand + ":" + operation.Id + ":" + noResponse);


            var thread = new Thread(() =>
           {
                var index = 0;

                while (true)
                {
                    Thread.Sleep(100);

                    if (IsDestroyed)
                        break;

                    if (operation.IsStopped || operation.IsComplete || operation.IsError)
                    {
                        Log("Stop Async Send part " + operation.SystemCommand + ":" + operation.Id);
                        break;
                    }

                    if (!IsConnected && (operation.SystemCommand != SystemCommandType.Ping))
                    {
                        Log("Async send wait connection");
                        operation.ClearReceivedParts();
                        index = 0;
                        Thread.Sleep(100);
                        continue;
                    }

    
                    // Thread.Sleep(100);
                    var allSend = true;

                    if (_lastSendPackageReturn || noResponse || operation.IsPartsReceived())
                    {
                        // Log(_lastSendPackageReturn +":"+ noResponse +":" + operation.IsPartsReceived());
                        if (!operation.IsPartsReceived())
                        {
                            for (var i = index; i < operation.Parts.Count; i++)
                            {
                                var packagePart = operation.Parts[i];

                              //  Thread.Sleep(100);


                                if (!operation.IsPartReceived(packagePart.Part))
                                {
                                    allSend = false;

                                    _lastSendPackagePart = packagePart.Id;
                                    _lastSendPackageReturn = false;

                                    Log("Send Package part " + packagePart.Id + ":" + operation.SystemCommand + ":" + operation.Id + ":" + packagePart.Part);
                                    SendData(Constants.SysCmdPrefix + packagePart.ToJson() + Constants.EndOfMessage);

                                    if (noResponse)
                                        operation.ReceivePart(i);

                                    index = i;
                                    break;
                                }
                            }
                        }

                    }
                    else
                   {
                       Log("Wait " + operation.Id + ":" + operation.SystemCommand + " wait for end op ");
                        continue;
                    }

                    if (allSend)
                    {
                        if (!operation.IsSend)
                        {
                            operation.SetSend();
                            Log("Async all part received " + operation.Id + ":" + operation.SystemCommand + " wait for end op ");
                        }
                    }
                }

                Log("Async send " + operation.Id + ":" + operation.SystemCommand + " end ");

            });

            thread.Start();
        }

        protected void SendData(string sendData)
        {
            //var thread = new Thread(() =>
            {
                var data = Encoding.UTF8.GetBytes(sendData);
                try
                {
                    Thread.Sleep(100);
                    Socket?.BeginSend(data, 0, data.Length, 0, ParallelSendCallback, Socket);
                }
                catch (Exception e)
                {
                    Log(e.Message, Utils.LogType.Errors);
                }
            }
            //  });

           // thread.Start();

        }

        protected void ParallelSendCallback(IAsyncResult ar)
        {
            try
            {
                var bytesSent = Socket.EndSend(ar);

                Log($"Sent {bytesSent} bytes to client.", Utils.LogType.Rest);
            }
            catch (Exception e)
            {
                Log("Send error " + e,Utils.LogType.Errors);
            }
        }

        public SendOperation SendResponseLessNetworkMessage<T>(SendData data, int command,
            string id = "",
            Action<T> onComplete = null, Action onFailed = null) where T : SendData
        {
            return SendResponseLessNetworkMessage(data, SystemCommandType.Message, command, id, onComplete, onFailed);
        }

        public SendOperation SendRequestNetworkMessage<T>(SendData data, int command, string id = "",
            Action<T> onComplete = null, Action<string> onFailed = null) where T : SendData
        {
            return SendRequestNetworkMessage(data, SystemCommandType.Message, command, id, onComplete, onFailed);
        }

        internal SendOperation SendResponseLessNetworkMessage<T>(SendData data, SystemCommandType command, int localCommand, string id = "",
            Action<T> onComplete = null, Action onFailed = null) where T : SendData
        {
            id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
          
            var operation = new SendOperation(id, data, command, localCommand);

            var thread = new Thread(() =>
            {
                lock (_sendOperations)
                {
                    _sendOperations.Add(operation);
                }

                SendNetworkDataProceed(operation, true);

                while (!operation.IsComplete && !operation.IsError && !operation.IsStopped && !operation.IsSend)
                {
                    Thread.Sleep(100);
                }

                if (operation.IsError || operation.IsStopped)
                {
                    Log("Failed " + operation.SystemCommand);
                    onFailed?.Invoke();
                    return;
                }
                else
                {
                    if (operation.IsSend)
                    {
                        CompleteOperation(operation, null, false, "");
                    }

                    onComplete?.Invoke(operation.ResponseData as T);
                }

                lock (_sendOperations)
                {
                    _sendOperations.Remove(operation);
                }
            });

            thread.Start();

            return operation;
        }

        internal SendOperation SendRequestNetworkMessage<T>(SendData data, SystemCommandType command, int localCommand, string id = "",
            Action<T> onComplete = null, Action<string> onFailed = null) where T : SendData
        {
            id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;

            var operation = new SendOperation(id, data, command,localCommand);

            var thread = new Thread(() =>
            {
 
                lock (_sendOperations)
                {
                    _sendOperations.Add(operation);
                }

                SendNetworkDataProceed(operation);

                while (!operation.IsComplete && !operation.IsError && !operation.IsStopped)
                {
                    Thread.Sleep(100);
                }

                if (operation.IsError || operation.IsStopped)
                {
                    onFailed?.Invoke(operation.ErrorMessage);
                    return;
                }
                else
                {
                    onComplete?.Invoke(operation.ResponseData as T);
                }

                lock (_sendOperations)
                {
                    _sendOperations.Remove(operation);
                }
            });

            thread.Start();

            return operation;
        }

        public void StopOperation(SendOperation op)
        {
            if (op != null)
            {
                op.Stop();

                if (op.SystemCommand == SystemCommandType.Message)
                    Log("Stop operation " + op.SystemCommand + ":" + op.LocalCommand, Utils.LogType.Important);
            }
            
        }

        private void AddOperation(SendOperation op)
        {
            lock (_sendOperations)
            {
                _sendOperations.Add(op);
            }
        }

        private void RemoveOperation(SendOperation op)
        {
            lock (_sendOperations)
            {
                _sendOperations.Remove(op);
            }
        }

        private void CompleteOperation(SendOperation op, SendData data, bool isError, string error)
        {
            op?.Complete(data, isError, error);
        }


        private void CompleteOperation(string opId, SendData data, bool isError, string error)
        {
            lock (_sendOperations)
            {
                var op = _sendOperations.Find(x => x.Id == opId);
                op?.Complete(data, isError, error);
            }

        }

        public void Destroy(bool force = true)
        {
            Log("Destroy client handler", Utils.LogType.Important);

            _socketThread?.Abort();
            _connectStateThread?.Abort();

            try
            {
                lock (ReceivedPackages)
                {
                    ReceivedPackages?.Clear();
                }

                lock (_sendOperations)
                {
                    foreach (var op in _sendOperations)
                    {
                        StopOperation(op);
                    }

                    _sendOperations.Clear();
                }
            }
            catch (Exception e)
            {
                Log(e.Message, Utils.LogType.Errors);
            }


            Buffer = null;
            Sb = null;
            _onLog = null;
            OnEvent = null;

            try
            {
                Socket?.Shutdown(SocketShutdown.Both);
                Socket?.Disconnect(false);
                Socket?.Close();
            }
            catch (Exception e)
            {
                Log(e.Message, Utils.LogType.Errors);
                // ignored
            }

            try
            {
                Socket?.Dispose();
            }
            catch (Exception e)
            {
                Log(e.Message, Utils.LogType.Errors);
            }

            IsConnected = false;
            IsDestroyed = true;
        }
    }
}
