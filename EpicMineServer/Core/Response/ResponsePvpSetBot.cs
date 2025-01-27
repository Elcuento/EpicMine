using System;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponsePvpSetBot : Response<RequestDataPvpSetBot>
    {

        public ResponsePvpSetBot(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var userInfo = Peer.SetBotToPvpArena();

                if (userInfo == null)
                    return false;

                ResponseData = new ResponseDataPvpSetBot(userInfo);
                return true;

            }
        }
    }
}