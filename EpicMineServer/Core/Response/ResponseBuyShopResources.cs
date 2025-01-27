using System;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseBuyShopResources : Response<RequestDataBuyShopResources>
    {

        public ResponseBuyShopResources(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var shopPack = staticData.ShopResources.Find(x => x.Id == Value.Id);
                if (shopPack == null)
                {
                    LogError("Pack not exist");
                    return false;
                }

                var sale = Value.Sale;


                if (sale > staticData.Configs.Shop.ResourceMaxSale)
                {
                    if (Value.Amount >= 10)
                    {
                        sale = (Value.Amount / 300f) * staticData.Configs.Shop.ResourceMaxSale;

                        if (sale > staticData.Configs.Shop.ResourceMaxSale)
                        {
                            sale = staticData.Configs.Shop.ResourceMaxSale;
                        }
                    }
                    else
                    {
                        sale = 0;
                    }
                }

                var price = Value.Amount * shopPack.CrystalCost;

                if (sale > 0)
                {
                    price = (long) (price - Math.Round(price * sale));
                }

                if (!Peer.IsCurrencyExist(CurrencyType.Crystals, price))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("No enough currency");
                    return false;
                }

                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, price))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("Cant subtract currency");
                    return false;
                }

                ResponseData = new ResponseDataSpendCrystals((int)price);
                Peer.SavePlayer();
            }

            return true;
        }
    }
}
