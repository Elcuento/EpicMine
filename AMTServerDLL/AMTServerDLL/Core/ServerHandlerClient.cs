using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AMTServerDLL.Dto;

namespace AMTServerDLL.Core
{
    public class ServerHandlerClient : IDisposable
    {
        public Action<Package> OnGetData;

        public Socket Socket { get; private set; }
        
        public string UserId { get; private set; }

        public long ConnectedTime;

        public long LastTimePinged;

        internal List<PackagePart> ReceivedPackages = new List<PackagePart>();

        internal List<string> AlreadyReceivedPackages = new List<string>();

        public int BufferSize = 1048576;

        public bool IsDestroyed;

        public byte[] Buffer;

        public StringBuilder Sb;

        public string LastReceiveCommand = "";

        protected string _lastSendPackagePart;

        protected bool _lastSendPackageReturn;

        protected List<SendOperation> _sendOperations = new List<SendOperation>();

        private Action<string, bool> _onLog;

        private Utils.LogType _logType;

        public object Locker = new object();

        private readonly ServerHandler _handlerServer;

        public ServerHandlerClient(ServerHandler handlerServer, Socket socket, string userId)
        {
            _handlerServer = handlerServer;

            Socket = socket;
            LastTimePinged = Utils.GetUnixTime();
            ConnectedTime = LastTimePinged;

            Buffer = new byte[BufferSize];
            Sb = new StringBuilder();

            UserId = userId;

            _lastSendPackageReturn = true;

            Log("Connected " + UserId);

            SocketLoop();
        }

        private void SocketLoop()
        {
            var clientReceivedThread = new Thread(() =>
            {
                while (true)
                {
                    if (IsDestroyed)
                    {
                        Log("destroyed");
                        return;
                    }

                    Thread.Sleep(500);

                    try
                    {
                        if (Socket == null || !Socket.Connected)
                        {
                            _handlerServer.Kick(this, "Client disconnected");
                            return;
                        }

                        if (Socket.Available < 0)
                            return;
                    }
                    catch (Exception e)
                    {
                        _handlerServer.Kick(this, "Client disconnected" +e);
                        return;
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
                    catch (Exception)
                    {
                        continue;
                    }

                    if (bytesRead <= 0)
                        continue;

                    try
                    {
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
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            });
            clientReceivedThread.Start();

        }

        private void TryExecCommand()
        {
            var s = Sb.ToString();

            if (s.StartsWith(Constants.SysCmdPrefix))
            {
                LastReceiveCommand = s;
                ParallelOnReceiveData();
            }
        }

        public void Subscribe(Action<string, bool> op)
        {
            _onLog += op;
        }

        public void UnSubscribe(Action<string, bool> op)
        {
            _onLog -= op;
        }

        public void SetLogType(Utils.LogType type)
        {
            _logType = type;
        }

        protected void Log(string message, Utils.LogType type = Utils.LogType.Rest)
        {
            var mes = "[ServerClientHandler]" + message;
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

        public void Dispose()
        {
            Socket?.Dispose();
            _onLog = null;
        }

        internal void ParallelOnReceiveData()
        {
            LastTimePinged = Utils.GetUnixTime();
            var content = LastReceiveCommand;

           // var thread = new Thread(() =>
           {
               try
               {
                   OnReceiveData(content);
               }
               catch (Exception e)
               {
                   Log(e.Message);
               }
           }
           //});

           // thread.Start();
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
            PackagePart packagePart = null;

            try
            {
                packagePart = data.FromJson<PackagePart>();
            }
            catch (Exception e)
            {
                Log(e.Message);
                return;
            }


            if (packagePart == null)
                return;

            Log("Open package part " + packagePart.Id + ": belong to :" + packagePart.PackageId);

            if (!packagePart.IsResponse)
            {
                var id = Guid.NewGuid().ToString();
                SendResponseLessNetworkMessage<SendData>(new SendDataPackagePart(packagePart.Part, packagePart.PackageId, packagePart.Id),
                    SystemCommandType.PackagePart, 0, id);

                Log("Send package part call back " + id + " for packege " + packagePart.PackageId);
            }


            Package package = null;

            lock (ReceivedPackages)
            {
              //  Log(ReceivedPackages.Count.ToString(), Utils.LogType.Important);

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
                    // Log(packagePart.Part);
                    var all = ReceivedPackages.FindAll(x => x.PackageId == packagePart.PackageId);
                    // Log(all.Count +":" + packagePart.PartCount);
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
                return;

            Log("Open package " + package.Id + ":" + package.Type);

            CompleteOperation(package.Id, (SendData)package.Data, false, package.ErrorMessage);

            Log("Command : " + package.Type);

            switch (package.Type)
            {
                case SystemCommandType.PackagePart:
                    ReceivedPackagePart(package);
                    return;

                case SystemCommandType.Ping:
                    Ping(package);
                    return;

                case SystemCommandType.Message:

                   /* lock (AlreadyReceivedPackages)
                    {
                        var packageUniqStr = package.Id + package.Command + package.Data?.GetType().Name;

                        if (AlreadyReceivedPackages.Contains(package.Id))
                        {
                            Log("Duplicate message " + package.Id, Utils.LogType.Important);
                            return;
                        }

                        if (AlreadyReceivedPackages.Count > 1000)
                            AlreadyReceivedPackages.RemoveRange(500, 500);

                        AlreadyReceivedPackages.Add(packageUniqStr);
                    }*/
                    
                    OnGetData?.Invoke(package);
                    break;

                default:
                {
                    Log(package.Type + " Not set switch", Utils.LogType.Errors);
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
            SendResponseLessNetworkMessage<SendData>(new SendDataPing(UserId), SystemCommandType.Ping, 0, pack.Id);
        }

        protected Package SendNetworkDataProceed(SendOperation operation, bool noResponse = false,
            Action onCompleted = null,
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

              //  SendData(Constants.SysCmdPrefix + packagePart.ToJson() + Constants.EndOfMessage);

                AsyncSendParts(operation, noResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log(e.Message, Utils.LogType.Errors);
                CompleteOperation(operation, null, true, "Client not connected");
                onFailed?.Invoke();
                return package;
            }


            onCompleted?.Invoke();

            return package;
        }

        internal void ReceivedPackagePart(Package pack)
        {
            try
            {
                var data = pack.Data as SendDataPackagePart;

                if (data.PackagePartId == _lastSendPackagePart)
                    _lastSendPackageReturn = true;

                lock (_sendOperations)
                {
                    var req = _sendOperations.Find(x => x.Id == data.PackageId);

                    Log("received part " + data.Part + " for " + req?.SystemCommand + ":" + (req == null ? "null" :"ok"));


                    if (req != null)
                    {
                        req.ReceivePart(data.Part);
                        Log("received part " + data.Part + " for " + req.SystemCommand + ":" + req);
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
                throw;
            }
        }


        protected void AsyncSendParts(SendOperation operation, bool noResponse = false)
        {
            Log("Start Async Send Pack " + operation.SystemCommand + ":" + operation.Id +":" + noResponse);

            var thread = new Thread(() =>
            {
                var index = 0;

                while (true)
                {
                    Thread.Sleep(500);

                    if (IsDestroyed)
                        return;

                    if (operation.IsStopped || operation.IsComplete || operation.IsError)
                    {
                        Log("Stop Async Send part " + operation.SystemCommand + ":" + operation.Id);
                        return;
                    }

                    var allSend = true;

                    if (_lastSendPackageReturn || noResponse || operation.IsPartsReceived())
                    {

                        if (!operation.IsPartsReceived())
                        {
                            for (var i = index; i < operation.Parts.Count; i++)
                            {
                                var packagePart = operation.Parts[i];

                                //Thread.Sleep(100);

                                if (!operation.IsPartReceived(packagePart.Part))
                                {
                                    allSend = false;

                                    _lastSendPackagePart = packagePart.Id;
                                    _lastSendPackageReturn = noResponse;

                                    Log("Send Package part " + packagePart.Id + ":" + operation.SystemCommand + ":" + operation.Id + ":" + packagePart.Part);

                                    SendData(Constants.SysCmdPrefix + packagePart.ToJson() + Constants.EndOfMessage);

                                    if(noResponse)
                                    operation.ReceivePart(i);

                                    index = i;
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                      //  Log("wait");
                        continue;
                    }

                    if (allSend)
                    {
                        if (!operation.IsSend)
                        {
                            operation.SetSend();
                            Log("Async all part received " + operation.Id + ":" + operation.SystemCommand +
                                " wait for end op ");
                        }
                    }
                }

            });

           thread.Start();
        }

        protected void SendData(string sendData)
        {
          //  var thread = new Thread(() =>
           // {
                var data = Encoding.UTF8.GetBytes(sendData);

                try
                {
                    Thread.Sleep(100);
                    Log("Send \n" + sendData);
                    Socket.Send(data);
                  //  Socket?.BeginSend(data, 0, data.Length, 0, ParallelSendCallback, Socket);
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
           // });

          //  thread.Start();

        }

        protected void ParallelSendCallback(IAsyncResult ar)
        {
            try
            {  

                var bytesSent = Socket.EndSend(ar);

                Log($"Sent {bytesSent} bytes to client.",Utils.LogType.Rest);
            }
            catch (Exception e)
            {
                Log("Send error " + e);
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

        internal SendOperation SendResponseLessNetworkMessage<T>(SendData data, SystemCommandType command, int localCommand, string id ="",
            Action<T> onComplete = null, Action onFailed = null) where T : SendData
        {
     

            id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;

            var operation = new SendOperation(id, data, command, localCommand);

            var thread = new Thread(() =>
            {
                AddOperation(operation);

                SendNetworkDataProceed(operation, true);

                while (!operation.IsComplete && !operation.IsError && !operation.IsStopped && !operation.IsSend)
                {
                   // Log(operation.IsSend +":" + operation.Id);
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

               RemoveOperation(operation);
            });

            thread.Start();

            return operation;
        }

        internal SendOperation SendRequestNetworkMessage<T>(SendData data, SystemCommandType command, int localCommand, string id = "",
            Action<T> onComplete = null, Action<string> onFailed = null) where T : SendData
        {
            id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;

            var operation = new SendOperation(id, data, command, localCommand);

            var thread = new Thread(() =>
            {

                AddOperation(operation);

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

                RemoveOperation(operation);
            });

            thread.Start();

            return operation;
        }

        private void AddOperation(SendOperation op)
        {
            lock (_sendOperations)
            {
                Log("Add operation " + op?.SystemCommand + ":" + op?.LocalCommand);
                _sendOperations.Add(op);
            }
        }

        private void RemoveOperation(SendOperation op)
        {
            lock (_sendOperations)
            {
                Log("Remove operation " + op?.SystemCommand + ":" + op?.LocalCommand);
                _sendOperations.Remove(op);
            }
        }

        private void StopOperation(SendOperation op)
        {
            if (op != null)
            {
                op.Stop();

                if (op.SystemCommand == SystemCommandType.Message)
                {
                    Log("Stop operation " + op.SystemCommand + ":" + op.LocalCommand, Utils.LogType.Important);
                }
            }

        }

        private void CompleteOperation(SendOperation op, SendData data, bool isError, string error)
        {
            op?.Complete(data, isError, error);
            Log("Complete send operation " + op?.SystemCommand +":" + op?.LocalCommand);
        }

        private void CompleteOperation(string opId, SendData data, bool isError, string error)
        {
            lock (_sendOperations)
            {
                var op = _sendOperations.Find(x => x.Id == opId);
                op?.Complete(data, isError, error);
                Log("Complete send operation " + op?.SystemCommand + ":" + op?.LocalCommand);
            }
            
        }

        public bool Check()
        {
            return LastTimePinged + 30 > Utils.GetUnixTime();
        }


        public void Destroy(string reason)
        {
            if (IsDestroyed)
                return;

            Log("Disconnected " + UserId + " " + reason);

            IsDestroyed = true;

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
            
            IsDestroyed = true;

            Buffer = null;
            Sb = null;
            _onLog = null;
            OnGetData = null;

            try
            {
                Socket?.Shutdown(SocketShutdown.Both);
                Socket?.Disconnect(false);
                Socket?.Close();
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
        
        }
    }
}
