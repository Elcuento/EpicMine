using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseTierOpen : Response<SendData>
    {

        public ResponseTierOpen(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var staticTiersCount = staticData.Tiers.Count;
                var lastOpenTier = 0;

                for (var index = 0; index < Peer.Player.Data.Dungeon.Tiers.Count; index++)
                {
                    if (Peer.Player.Data.Dungeon.Tiers[index].IsOpen || index == 0)
                    {
                        lastOpenTier = index;
                        continue;
                    }

                    if (!Peer.Player.Data.Dungeon.Tiers[index].IsOpen)
                        break;
                    
                }

                var nextClosedTierNumber = lastOpenTier + 1;

                if (staticTiersCount <= nextClosedTierNumber)
                {
                    LogError("Tier is to much, return true any way" + nextClosedTierNumber);
                    return false;
                }

                while (Peer.Player.Data.Dungeon.Tiers.Count < lastOpenTier)
                {
                    Peer.Player.Data.Dungeon.Tiers.Add(new Tier(Peer.Player.Data.Dungeon.Tiers.Count, true));
                }

                Peer.SavePlayer();

                var tierOpenCost = staticData.Tiers[nextClosedTierNumber - 1].RequireArtefacts;
               
                if (Peer.Player.Data.Artifacts < tierOpenCost)
                {
                    LogError("No artifacts");
                    return false;
                }

                Peer.Player.Data.Artifacts -= tierOpenCost;

                Peer.SavePlayer();
            }
            return true;
        }
    }
}