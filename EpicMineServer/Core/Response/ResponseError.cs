using AMTServerDLL.Dto;
using EpicMineServerDLL.Static.Enums;

namespace AMTServer.Core.Response
{
    public class ResponseError : Response<SendData>
    {
        private readonly ErrorType _errorCode;

        public ResponseError(ClientPeer peer, Package pack, ErrorType errorCode) : base(peer, pack)
        {
            _errorCode = errorCode;
        }

        protected override bool OnProcess()
        {
            ResponseData = new SendData(new SendDataError((int)_errorCode));

            return false;
        }

    }
}