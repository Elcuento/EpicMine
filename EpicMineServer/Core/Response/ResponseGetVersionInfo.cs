using System;
using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetGameEventsInfo : Response<SendData>
    {

        public ResponseGetGameEventsInfo(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataGetGameEventsInfo(new List<GameEvent>());
            return true;
        }
    }
}