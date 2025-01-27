using System;
using AMTServer.Common;
using AMTServerDLL.Dto;
using CommonDLL.Static;
using Chest = CommonDLL.Dto.Chest;


namespace AMTServer.Core.Response
{
    public class ResponseChestOpenForce : Response<RequestDataChestOpenForce>
    {

        public ResponseChestOpenForce(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();


                var simpleChestBreakingTime = staticData.Configs.Burglar.Chests[ChestType.Simple].BreakingTimeInMinutes;
                var royalChestBreakingTime = staticData.Configs.Burglar.Chests[ChestType.Royal].BreakingTimeInMinutes;

                var chestForceCompletePricePer30Minutes = staticData.Configs.Burglar.ChestForceCompletePricePer30Minutes;

                if (string.IsNullOrEmpty(Value.Id))
                {
                    var breakingMinutesLeft = Value.Type == ChestType.Royal
                        ? royalChestBreakingTime
                        : simpleChestBreakingTime;

                    var forceOpenPrice = Math.Ceiling(breakingMinutesLeft / 30f) * chestForceCompletePricePer30Minutes;

                    return OpenChestInternal(staticData.Configs, Value.Type, (int) forceOpenPrice);
                }

          
                var chestData = Peer.Player.Data.Burglar.Chests.Find(x => x.Id == Value.Id);

                if (chestData == null)
                {
                    LogError("Cant find chest in player");
                    return false;
                }

                var chestType = chestData.Type;
                var userChestStartBreakingTimeData = chestData.StartBreakingTime;

                // force open chest who not started breaking
                if (userChestStartBreakingTimeData == null)
                {
                    var breakingMinutesLeft2 = chestType == ChestType.Royal
                        ? royalChestBreakingTime
                        : simpleChestBreakingTime;

                    var forceOpenPrice2 = Math.Ceiling(breakingMinutesLeft2 / 30f) * chestForceCompletePricePer30Minutes;

                    return OpenChestInternal(staticData.Configs, chestType, (int) forceOpenPrice2, chestData);
                }

                var now = Utils.GetUnixTime();
                var minute = 60;
                var userChestStartBreakingTime = Utils.GetUnixTime((DateTime) userChestStartBreakingTimeData);
                var breakedTime = chestType == ChestType.Royal
                    ? userChestStartBreakingTime + (royalChestBreakingTime * minute)
                    : userChestStartBreakingTime + (simpleChestBreakingTime * minute);

                if (breakedTime <= now)
                {
                    LogError("Break time error");
                    return false;
                }

                var breakingMinutesLeft3 = (breakedTime - now) / (60);
                var forceOpenPrice3 = Math.Ceiling(breakingMinutesLeft3 / 30f) * chestForceCompletePricePer30Minutes;

                return OpenChestInternal(staticData.Configs, chestType, (int) forceOpenPrice3, chestData);
            }
        }


        bool OpenChestInternal(Configs data, ChestType type, int cost, Chest chest = null)
        {

            if (cost > 0)
            {
                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, cost))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("Dont have crystals");
                    return false;
                }
            }

            var randomDroppedCrystalsAmount = GetChestRandomDroppedCrystals(data, type);
            if (randomDroppedCrystalsAmount > 0)
            {
                Peer.AddCurrency(CurrencyType.Crystals, randomDroppedCrystalsAmount);
            }

            var randomDroppedArtefactsAmount = GetChestRandomDroppedArtefacts(data, type);

            if (randomDroppedArtefactsAmount > 0)
                Peer.Player.Data.Artifacts += randomDroppedArtefactsAmount;

            if (chest != null)
            {
                Peer.Player.Data.Burglar.Chests.Remove(chest);
            }

            Peer.SavePlayer();

            ResponseData = new ResponseDataChestOpenForce(cost,
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