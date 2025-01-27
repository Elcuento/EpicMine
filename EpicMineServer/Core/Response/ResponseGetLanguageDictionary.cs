using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseGetLanguageDictionary : Response<RequestDataGetLanguageDictionary>
    {

        public ResponseGetLanguageDictionary(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            ResponseData = new ResponseDataGetLanguageDictionary(Peer.GetLanguageDictionary(Value.LanguageCode));
            return true;
        }
    }
}