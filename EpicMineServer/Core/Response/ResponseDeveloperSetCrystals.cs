using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperSetCrystals : Response<RequestDataDeveloperSetCrystals>
    {

        public ResponseDeveloperSetCrystals(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.AddCurrency(CurrencyType.Crystals, Value.Val);
                Peer.SavePlayer();
            }

            return true;
        }
    }
}