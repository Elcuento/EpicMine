using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseRestorePickaxeByCrystals : Response<SendData>
    {

        public ResponseRestorePickaxeByCrystals(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var priceInCrystals = Peer.GetStaticData().Configs.Dungeon.ContinueCrystalPrice;

                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("Not enough crystals");
                    return false;
                }

                ResponseData = new ResponseDataSpendCrystals(priceInCrystals);
                return true;
            }
            
        }
    }
}