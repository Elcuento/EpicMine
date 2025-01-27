using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponsePvpChestOpen : Response<RequestDataPvpChestOpen>
    {

        public ResponsePvpChestOpen(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Value.IsInventory)
                {
                    return true;
                }

                if (Value.Type == PvpChestType.Winner)
                {
                    if (Peer.Player.Data.Pvp.Chests < 5)
                        return false;

                    Peer.Player.Data.Pvp.Chests = 0;
                    Peer.SavePlayer();
                }

                return true;
            };
        }
    }
}