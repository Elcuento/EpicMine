using System;
using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseWorkShopComplete : Response<RequestDataWorkShopSlotComplete>
    {

        public ResponseWorkShopComplete(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var staticWorkshopSlotsCount = staticData.WorkshopSlots.Count;
                if (staticWorkshopSlotsCount <= Value.Number)
                {
                    LogError("No static data slots" + staticData.WorkshopSlots.Count + ":" + Value.Number);
                    return false;
                }

                var slots = Value.SlotType == WorkShopSlotType.Ore ? Peer.Player.Data.Workshop.Slots :
                    Peer.Player.Data.Workshop.SlotsShard;

                if (slots.Count == 0)
                {
                    LogError("No slots");
                    return false;
                }

                if (slots.Count < Value.Number)
                {
                    LogError("Slots more then value");
                    return false;
                }

                var userWorkshopSlotData = slots[Value.Number];


                if (userWorkshopSlotData == null)
                {
                    LogError("User workshop slot data null");
                    return false;
                }

                Recipe recipeData = null;

                recipeData = staticData.Recipes.Find(x => x.Id == userWorkshopSlotData.ItemId && x.Type == userWorkshopSlotData.Type);

                if (recipeData == null)
                {
                    LogError("No recipe " + userWorkshopSlotData.ItemId +":" + userWorkshopSlotData.Type);
                    return false;
                }

                if (userWorkshopSlotData.MeltingStartTime == null)
                {
                    LogError("Melting start time null");
                    return false;
                }

                var meltingStartTime = Utils.GetUnixTime(userWorkshopSlotData.MeltingStartTime ?? DateTime.Now);
                var necessaryAmount = userWorkshopSlotData.NecessaryAmount;

               // var craftEndTime = meltingStartTime + (recipeData.CraftTime  * necessaryAmount);
                var now = Utils.GetUnixTime();

                var timeSpend = (recipeData.CraftTime / GetMeltingBoostCoefficient(staticData) *
                                 necessaryAmount);

                var craftEndTime = Math.Floor(meltingStartTime + timeSpend);

                if (now > craftEndTime)
                    return false;

                var minutesLeft = Math.Ceiling(((double)craftEndTime - now) / (60));
                var cost = 0;

                if (minutesLeft <= 1)
                    cost = staticData.Configs.Workshop.ForceCompletePrices.LessThanOneMinute;
                else if (minutesLeft <= 5)
                    cost = staticData.Configs.Workshop.ForceCompletePrices.LessThanFiveMinutes;
                else if (minutesLeft <= 10)
                    cost = staticData.Configs.Workshop.ForceCompletePrices.LessThanTenMinutes;
                else
                {
                    var numberOfTen = Math.Ceiling((minutesLeft - 10) / 10);
                    cost = (int)(staticData.Configs.Workshop.ForceCompletePrices.LessThanTenMinutes + numberOfTen *
                            staticData.Configs.Workshop.ForceCompletePrices.MoreThanTenMinutesForEveryTen);
                }

                if (!Peer.SubsTractCurrency(CurrencyType.Crystals, cost))
                {
                    Peer.SendUpdateWalletCrystals();
                    LogError("Not enough crystals");
                    return false;
                }


                userWorkshopSlotData.MeltingStartTime =
                    Utils.FromUnix((now - (long)Math.Floor(timeSpend)));

                ResponseData = new ResponseDataWorkShopComplete(necessaryAmount,cost);
                Peer.SavePlayer();
            }

            return true;
        }

        private float GetMeltingBoostCoefficient(StaticData data)
        {

            var dateNow = Utils.GetUnixTime();

            if (Peer.Player.Data.Effect.BuffList.Count == 0)
                return 1;

            Buff maxBuff = null;
            int maxPriority = -1;

            foreach (var buff in Peer.Player.Data.Effect.BuffList)
            {
                var staticBuff = data.Buffs.Find(x => x.Id == buff.Id);
                if(staticBuff == null)
                    continue;
                

                if (buff.Time > dateNow)
                {
                    if (buff.Type == BuffType.Boost && staticBuff.Priority > maxPriority)
                    {
                        maxBuff = staticBuff;
                        maxPriority = staticBuff.Priority;
                    }
                }
            }

            if (maxBuff?.Value == null)
            {
                return 1;
            }
            else
            {
                if(maxBuff.Value.ContainsKey(BuffValueType.Melting))
                return maxBuff.Value[BuffValueType.Melting];
            }

            return 1;
        }
    }

 
}