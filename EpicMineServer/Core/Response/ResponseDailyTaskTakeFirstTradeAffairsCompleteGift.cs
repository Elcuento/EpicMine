using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseDailyTaskTakeFirstTradeAffairsCompleteGift : Response<SendData>
    {

        public ResponseDailyTaskTakeFirstTradeAffairsCompleteGift(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                if (Peer.Player.Data.AdditionalInfo.IsFirstTradeAffairsDailyTaskCompleteGiftTaken)
                {
                    LogError("Gift already taken");
                    return false;
                }

                var giftCrystalsAmount =
                    staticData.Configs.CustomGifts.FirstTradeAffairsDailyTaskCompleteCrystalsAmount;

                Peer.Player.Data.AdditionalInfo.IsFirstTradeAffairsDailyTaskCompleteGiftTaken = true;

                Peer.AddCurrency(CurrencyType.Crystals, giftCrystalsAmount);

                ResponseData = new ResponseDataAddCrystal(giftCrystalsAmount);

                Peer.SavePlayer();

                return true;
            }
        }
    }
}