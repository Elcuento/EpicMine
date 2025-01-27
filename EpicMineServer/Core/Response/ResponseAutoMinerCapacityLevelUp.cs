using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseAutoMinerCapacityLevelUp : Response<RequestDataAutoMinerCapacityLevelUp>
    {

        public ResponseAutoMinerCapacityLevelUp(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                long cost = 0;

                foreach (var l in staticData.AutoMinerCapacityLevels[Value.Level].CurrencyCost)
                {
                    if(l.Key == CurrencyType.Crystals)
                    cost += l.Value;
                }


                if (Peer.SubsTractCurrency(CurrencyType.Crystals, cost))
                    ResponseData = new ResponseDataSpendCrystals(cost);
                else
                {
                    Log("Not enough crystals cost ");
                    Peer.SendUpdateWalletCrystals();
                    return false;
                }


                Peer.SavePlayer();
            }

            return true;
        }
    }
}