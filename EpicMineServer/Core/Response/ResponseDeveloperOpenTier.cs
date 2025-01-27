using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperOpenTier : Response<RequestDataDeveloperOpenTier>
    {

        public ResponseDeveloperOpenTier(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var tier = Value.Val;

                var staticData = Peer.GetStaticData();

                var staticTiersCount = staticData.Tiers.Count;
                var nextClosedTierNumber = Peer.Player.Data.Dungeon.Tiers.Count;

                if (nextClosedTierNumber >= staticTiersCount)
                    tier = staticTiersCount - 1;

                for (var i = nextClosedTierNumber; i < tier; i++)
                {
                    Peer.Player.Data.Dungeon.Tiers.Add(new CommonDLL.Dto.Tier(i, true));
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}