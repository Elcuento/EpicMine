using System;
using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseWorkShopSlotCollect : Response<RequestDataWorkShopSlotCollect>
    {

        public ResponseWorkShopSlotCollect(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var staticWorkshopSlotsCount = staticData.WorkshopSlots.Count;
                if (staticWorkshopSlotsCount <= Value.Number)
                    return false;

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

                recipeData = staticData.Recipes.Find(x => x.Id == userWorkshopSlotData.ItemId 
                                                          && x.Type == userWorkshopSlotData.Type);

                if (recipeData == null)
                {
                    LogError("No recipe " + userWorkshopSlotData.ItemId +":" + userWorkshopSlotData.Type);
                    if (slots.Count == 0)
                    {
                        LogError("No slots");
                    }
                    else
                    {
                        LogError(slots.ToJson());
                    }

                    return false;
                }

                if (userWorkshopSlotData.MeltingStartTime == null)
                {
                    LogError("Melting start time null");
                    return false;
                }

                var meltingStartTime = Utils.GetUnixTime(userWorkshopSlotData.MeltingStartTime ?? DateTime.Now);
                var timeCoefficient = GetMeltingBoostCoefficient(staticData);

                var now = Utils.GetUnixTime();
                var completeAmount = 0;
                var oneItemCraftTime = (int) ((recipeData.CraftTime / timeCoefficient));
                var needAmount = Value.NeedAmount;

              //  Log("\n Look" + needAmount +":"+ completeAmount);

                if (userWorkshopSlotData.NecessaryAmount < needAmount)
                {
                    needAmount = userWorkshopSlotData.NecessaryAmount;
                }
                //Log("\n Look DIF" + meltingStartTime);

                for (var i = 1; i <= needAmount; i++)
                {
                    if (now > meltingStartTime + oneItemCraftTime)
                    {
                        completeAmount++;
                        meltingStartTime += (long)oneItemCraftTime;
                        //Log("\n Look" + needAmount + ":" + completeAmount);
                    }
                    else
                    {
                       // Log("\n Look" + now +":" + meltingStartTime +":"+ oneItemCraftTime +":" + needAmount);
                    }
                }

            //  Log(now - 5 * oneItemCraftTime +":" + (now - meltingStartTime) +":" + oneItemCraftTime);
                if (completeAmount <= 0 && !Value.WithStop)
                {
                    LogError("Complete amount or no stop " + completeAmount +":" + Value.WithStop);
                    return false;
                }

                if (completeAmount < userWorkshopSlotData.NecessaryAmount)
                {
                    if (Value.WithStop)
                    {
                        userWorkshopSlotData.ItemId = "";
                        userWorkshopSlotData.NecessaryAmount = 0;
                        userWorkshopSlotData.MeltingStartTime = null;
                    }
                    else
                    {
                        userWorkshopSlotData.NecessaryAmount = userWorkshopSlotData.NecessaryAmount - completeAmount;
                        userWorkshopSlotData.MeltingStartTime = Utils.FromUnix(meltingStartTime);
                    }
                }
                else
                {
                    userWorkshopSlotData.ItemId = "";
                    userWorkshopSlotData.NecessaryAmount = 0;
                    userWorkshopSlotData.MeltingStartTime = null;
                }

                ResponseData = new ResponseDataWorkShopCollect(completeAmount);
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

