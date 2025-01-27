using System;
using AMTServerDLL;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class Response<T> where T : SendData
    {
        protected readonly T Value;

        protected ClientPeer Peer;

        protected Package Pack;

        protected SendData ResponseData = new SendData();

        protected bool _isLogDisabled;

        protected bool _isDisabledArchiveMessage;

        protected Response(ClientPeer peer, Package pack)
        {
            Peer = peer;
            Value = pack?.Data as T;
            Pack = pack;
        }

        public void DisableLog(bool state)
        {
            _isLogDisabled = state;
        }

        public void DisableArchive(bool b)
        {
            _isDisabledArchiveMessage = b;
        }

        public static void StartResponse(Response<T> resp)
        {
            resp.Process();
        }

        protected void LogError(string log)
        {
            if (_isLogDisabled)
                return;

            Peer.Log($"[Response][{GetType().FullName}][Error]" + log, true);
        }

        protected void Log(string log)
        {
            if (_isLogDisabled)
                return;

            Peer.Log($"[Response][{GetType().FullName}]" + log);
        }


        private void Process()
        {
            Log("[Process]");

            bool result;

            try
            {
                result = OnProcess();
            }
            catch (Exception e)
            {
                LogError(e.ToString());
                Error();
                return;
            }

            if (result)
            {
                Complete();
            }
            else
            {
                Error();
            }
        }

        private void Complete()
        {
            Log("[Completed]");

            if (ResponseData == null)
                ResponseData = new SendData();

            if (!_isDisabledArchiveMessage)
            {
                Peer.AddSendPackage(ResponseData, Pack.Command, Pack.Id);
            }

            Peer?.SendResponseLessNetworkMessage(ResponseData,
                Pack.Command, Pack.Id);
        }

        private void Error()
        {
            if (!_isLogDisabled)
            {
                LogError(Value == null ? "[Failed]" : $"[Failed][{Peer?.Version?.ToJson()}]{Value.ToJson()}");
            }
      

            if (ResponseData == null)
                ResponseData = new SendData();

            if (ResponseData.Error == null)
                ResponseData.Error = new SendDataError();

            if (!_isDisabledArchiveMessage)
            {
                Peer.AddSendPackage(ResponseData, Pack.Command, Pack.Id);
            }

            Peer?.SendResponseLessNetworkMessage(ResponseData,
                Pack.Command, Pack.Id);
        }


        protected virtual bool OnProcess()
        {
            return true;
        }
    }

}

