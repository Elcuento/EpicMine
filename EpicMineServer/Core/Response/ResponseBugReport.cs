using AMTServerDLL.Dto;


namespace AMTServer.Core.Response
{
    public class ResponseBugReport : Response<RequestDataBugReport>
    {

        public ResponseBugReport(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }


        protected override bool OnProcess()
        {
            Peer.BugReport(Value.DeviceId, Value.Str);
            return true;
        }
    }
}