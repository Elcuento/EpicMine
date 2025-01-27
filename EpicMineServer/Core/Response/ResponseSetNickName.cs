using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseSetNickName : Response<RequestDataSetNickName>
    {

        public ResponseSetNickName(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Value == null || string.IsNullOrEmpty(Value.NickName))
                    return false;

                if (Value.NickName.Length > 16 || Value.NickName.Length < 3)
                    return false;

                if (Value.NickName.ToLower() == "admin")
                    return false;

                var result = Peer.GetByNickName(Value.NickName);

                if (result == null)
                {
                    Peer.Player.Data.Nickname = Value.NickName;
                    Peer.SavePlayer();

                    ResponseData = new ResponseDataSetNickName(Value.NickName);
                }
                else
                {
                    return false;
                }

                return true;
            }
          
        }
    }
}