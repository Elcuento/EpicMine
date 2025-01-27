using System;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseGiftOpen : Response<SendData>
    {

        public ResponseGiftOpen(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var periodInMinutes = staticData.Configs.Gifts.PeriodInMinutes;
                var dailyCount = staticData.Configs.Gifts.DailyCount;
                var simpleArtefactsCount = staticData.Configs.Gifts.SimpleArtefactsCount;
                var royalArtefactsCount = staticData.Configs.Gifts.RoyalArtefactsCount;
                var maxArtefactsCount = staticData.Configs.Dungeon.TierOpenArtefactsCost;

                var now = Utils.GetUnixTime();

                var lastOpenTime = Peer.Player.Data.Gifts.LastOpenTime;

                var openedCount = Peer.Player.Data.Gifts.OpenedCount;

                if (lastOpenTime + 60 * 60 * 24 < now)
                {
                    openedCount = 0;
                }
                else if (lastOpenTime + 60 * periodInMinutes < now)
                {
                    if (Peer.Player.Data.Gifts.OpenedCount > dailyCount)
                    {
                        openedCount = 0;
                    }
                }
                /*else
                    return false;*/

                openedCount++;

                var artefactsCount = openedCount == dailyCount
                    ? royalArtefactsCount
                    : simpleArtefactsCount;

                var dropped = 0;

                if (Peer.Player.Data.Artifacts + artefactsCount > maxArtefactsCount)
                    dropped = maxArtefactsCount - Peer.Player.Data.Artifacts;

                Peer.Player.Data.Artifacts += dropped;
                Peer.Player.Data.Gifts.OpenedCount = openedCount;
                Peer.Player.Data.Gifts.LastOpenTime = Utils.GetUnixTime();


                ResponseData = new ResponseDataGiftOpen(dropped);
                return true;
            }
        }
    }
}