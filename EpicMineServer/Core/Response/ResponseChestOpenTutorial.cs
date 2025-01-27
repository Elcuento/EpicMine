using System;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Static;
using Chest = CommonDLL.Dto.Chest;


namespace AMTServer.Core.Response
{
    public class ResponseChestOpenTutorial : Response<RequestDataChestOpenTutorial>
    {

        public ResponseChestOpenTutorial(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var chest = Peer.Player.Data.Burglar.Chests.Find(x => x.Id == Value.Id);
                if (chest == null)
                {
                    LogError("Second open tutorial chest");
                    ResponseData = new ResponseDataChestOpenTutorial(0,
                        0, 0);
                    return true;
                }

                return OpenChestInternal(staticData.Configs, chest);
            }
        }


        bool OpenChestInternal(Configs data, Chest chest)
        {

            var randomDroppedCrystalsAmount = GetChestRandomDroppedCrystals(data, chest.Type);
            if (randomDroppedCrystalsAmount > 0)
            {
                Peer.AddCurrency(CurrencyType.Crystals, randomDroppedCrystalsAmount);
            }

            var randomDroppedArtefactsAmount = GetChestRandomDroppedArtefacts(data, chest.Type);

            if (randomDroppedArtefactsAmount > 0)
                Peer.Player.Data.Artifacts += randomDroppedArtefactsAmount;

            
            Peer.Player.Data.Burglar.Chests.Remove(chest);
            
            Peer.SavePlayer();

            ResponseData = new ResponseDataChestOpenTutorial(0,
                randomDroppedArtefactsAmount, randomDroppedCrystalsAmount);

            return true;
        }

        int GetChestRandomDroppedCrystals(Configs data, ChestType type)
        {

            var simpleChestDropCrystalsData = data.Burglar.Chests[ChestType.Simple].Drop[ChestItemDropType.Crystals];
            var royalChestDropCrystalsData = data.Burglar.Chests[ChestType.Royal].Drop[ChestItemDropType.Crystals];
            var simpleChestDropCrystalsChance = simpleChestDropCrystalsData.Chance;
            var royalChestDropCrystalsChance = royalChestDropCrystalsData.Chance;
            var simpleChestDropCrystalsMin = simpleChestDropCrystalsData.Min;
            var royalChestDropCrystalsMin = royalChestDropCrystalsData.Min;
            var simpleChestDropCrystalsMax = simpleChestDropCrystalsData.Max;
            var royalChestDropCrystalsMax = royalChestDropCrystalsData.Max;

            var crystalsChance = type == ChestType.Royal
                ? royalChestDropCrystalsChance
                : simpleChestDropCrystalsChance;

            var randomCrystalsChance = new Random().Next(0, 100);
            if (randomCrystalsChance > crystalsChance)
                return 0;

            var crystalsMin = type == ChestType.Royal
                ? royalChestDropCrystalsMin
                : simpleChestDropCrystalsMin;

            var crystalsMax = type == ChestType.Royal
                ? royalChestDropCrystalsMax
                : simpleChestDropCrystalsMax;

            var randomAmount = new Random().Next(crystalsMin, crystalsMax);
            return randomAmount;

        }

        int GetChestRandomDroppedArtefacts(Configs data, ChestType type)
        {

            var maxArtefactsCount = data.Dungeon.TierOpenArtefactsCost;
            
                var userArtefacts = Peer.Player.Data.Artifacts;
                maxArtefactsCount = maxArtefactsCount - userArtefacts;
            

            var simpleChestDropArtefactsData = data.Burglar.Chests[ChestType.Simple].Drop[ChestItemDropType.Artifacts];
            var royalChestDropArtefactsData = data.Burglar.Chests[ChestType.Royal].Drop[ChestItemDropType.Artifacts];
            var simpleChestDropArtefactsMin = simpleChestDropArtefactsData.Min;
            var royalChestDropArtefactsMin = royalChestDropArtefactsData.Min;
            var simpleChestDropArtefactsMax = simpleChestDropArtefactsData.Max;
            var royalChestDropArtefactsMax = royalChestDropArtefactsData.Max;

            var artefactsMin = type == ChestType.Royal
                ? royalChestDropArtefactsMin
                : simpleChestDropArtefactsMin;

            var artefactsMax = type == ChestType.Royal
                ? royalChestDropArtefactsMax
                : simpleChestDropArtefactsMax;

            if (artefactsMin > maxArtefactsCount)
                artefactsMin = maxArtefactsCount;

            if (artefactsMax > maxArtefactsCount)
                artefactsMax = maxArtefactsCount;

            var randomAmount = new Random().Next(artefactsMin, artefactsMax);
            return randomAmount;

        }
    }

   
}