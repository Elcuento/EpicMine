using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseWorkShopSlotCollectCrystalPay : Response<RequestDataWorkShopSlotCollectCrystalPay>
    {

        public ResponseWorkShopSlotCollectCrystalPay(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var staticWorkshopSlotsCount = staticData.WorkshopSlots.Count;
                if (staticWorkshopSlotsCount <= Value.Number)
                    return false;

                var priceInCrystals = staticData.Configs.Workshop.CollectCrystalPrice;

                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, priceInCrystals))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("Not enough crystals");
                    return false;
                }

                ResponseData = new ResponseDataSpendCrystals(priceInCrystals);

                Peer.SavePlayer();
            }

            return true;
        }
    }
}