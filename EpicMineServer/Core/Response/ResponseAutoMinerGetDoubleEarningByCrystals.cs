using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseAutoMinerGetDoubleEarningByCrystals : Response<SendData>
    {

        public ResponseAutoMinerGetDoubleEarningByCrystals(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var priceInCrystals = staticData.Configs.Pvp.ChestDoubleBonusCrystalsCost;

                if (Peer.IsCurrencyExist(CurrencyType.Crystals, priceInCrystals))
                {
                    if (!Peer.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
                    {
                        Peer.SendUpdateWalletCrystals();
                        return false;
                    }
                }
                else
                {
                    Peer.SendUpdateWalletCrystals();
                    return false;
                }

                ResponseData = new ResponseDataSpendCrystals(priceInCrystals);

                Peer.SavePlayer();

                return true;
            }
        }
    }
}