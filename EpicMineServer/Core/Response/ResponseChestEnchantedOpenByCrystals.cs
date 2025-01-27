using System;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseChestEnchantedOpenByCrystals : Response<SendData>
    {

        public ResponseChestEnchantedOpenByCrystals(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var chestPriceInCrystals = 8;

                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, chestPriceInCrystals))
                {
                    LogError("Not enough money");
                    Peer.SendUpdateWalletCrystals();
                    return false;
                }

                var droppedArtifacts = GetRandomDroppedArtefactsAmount(staticData.Configs);

                Peer.Player.Data.Artifacts += droppedArtifacts;

               ResponseData = new ResponseDataChestEnchantedOpenByCrystals(droppedArtifacts, chestPriceInCrystals);
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