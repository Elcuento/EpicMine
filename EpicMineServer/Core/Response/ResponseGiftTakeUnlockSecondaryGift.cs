using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseGiftTakeUnlockSecondaryGift : Response<SendData>
    {

        public ResponseGiftTakeUnlockSecondaryGift(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                if (Peer.Player.Data.AdditionalInfo.IsUnlockSecondTierGiftTaken)
                {
                    LogError("Gift already taken");
                    return false;
                }

                var giftCrystalsAmount = staticData.Configs.CustomGifts.UnlockSecondTierCrystalsAmount;

                Peer.Player.Data.AdditionalInfo.IsUnlockSecondTierGiftTaken = true;
                Peer.AddCurrency(CurrencyType.Crystals, giftCrystalsAmount);

                Peer.SavePlayer();

                ResponseData = new ResponseDataAddCrystal(giftCrystalsAmount);

                return true;
            }
        }
    }
}