using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetServerStatus : Response<ResponseDataServerStatus>
    {

        public ResponseGetServerStatus(ClientPeer peer, Package pack) : base(peer, pack)
        {
         
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataServerStatus(true);
            return true;
        }
    }
}