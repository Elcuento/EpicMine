using System;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseChestEnchantedOpenByAd : Response<SendData>
    {

        public ResponseChestEnchantedOpenByAd(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var droppedArtifacts = GetRandomDroppedArtefactsAmount(staticData.Configs);

                Peer.Player.Data.Artifacts += droppedArtifacts;

               ResponseData = new ResponseDataChestEnchantedOpenByAd(droppedArtifacts);
               return true;
            }
        }

        int GetRandomDroppedArtefactsAmount(Configs configsData)
        {

            var maxArtefactsCount = configsData.Dungeon.TierOpenArtefactsCost;

            maxArtefactsCount = maxArtefactsCount - Peer.Player.Data.Artifacts;

            var artefactsMin = configsData.EnchantedChests.Drop.Min;
            var artefactsMax = configsData.EnchantedChests.Drop.Max;

            if (artefactsMin > maxArtefactsCount)
                artefactsMin = maxArtefactsCount;

            if (artefactsMax > maxArtefactsCount)
                artefactsMax = maxArtefactsCount;

            var randomAmount = new Random().Next((int)artefactsMin, (int)artefactsMax);
            return randomAmount;

        }
    }
}