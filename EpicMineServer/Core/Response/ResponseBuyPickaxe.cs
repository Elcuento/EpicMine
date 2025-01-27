using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseBuyPickaxe : Response<RequestDataBuyPickaxe>
    {

        public ResponseBuyPickaxe(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var pickaxe = staticData.Pickaxes.Find(x => x.Id == Value.Id);

                if (pickaxe != null && pickaxe.Type == PickaxeType.Donate)
                {
                    if (Peer.SubsTractCurrency(CurrencyType.Crystals, pickaxe.Cost))
                    {
                        ResponseData  = new ResponseDataBuyPickaxe(pickaxe.Cost);
                    }
                    else
                    {
                        LogError("Not enough crystals");
                        Peer.SendUpdateWalletCrystals();
                        return false;
                    }
                }
                else
                {
                    LogError("Pickaxe not exist");
                    return false;
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}