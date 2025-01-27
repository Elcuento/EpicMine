using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponsePvpChestGetDouble : Response<SendData>
    {

        public ResponsePvpChestGetDouble(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var priceInCrystals = staticData.Configs.Pvp.ChestDoubleBonusCrystalsCost;
                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("Not enough crystals");
                    return false;
                }

                ResponseData = new ResponseDataSpendCrystals(priceInCrystals);

                Peer.SavePlayer();
                return true;
            };


        }
    }
}