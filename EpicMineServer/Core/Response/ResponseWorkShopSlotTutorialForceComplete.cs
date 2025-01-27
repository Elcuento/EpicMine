using System;
using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Static;


namespace AMTServer.Core.Response
{
    public class ResponseWorkShopSlotTutorialForceComplete : Response<RequestDataWorkShopSlotTutorialForceComplete>
    {

        public ResponseWorkShopSlotTutorialForceComplete(ClientPeer peer, Package pack) : base(peer, pack)
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
                    LogError("Static slots more then number");
                    return false;
                }

                if (Peer.Player.Data.Workshop.Slots.Count <= 0)
                {
                    LogError("No slots, or wrong number");
                    return false;
                }

                var slot = Peer.Player.Data.Workshop.Slots[Value.Number];

                var recipeData = staticData.Recipes.Find(x => x.Id == slot.ItemId);

                if (recipeData == null)
                {
                    LogError("Error recipe data");
                    return false;
                }

                var necessaryAmount = slot.NecessaryAmount;

                var timeSpend = (recipeData.CraftTime / GetMeltingBoostCoefficient(staticData) *
                                 necessaryAmount);


                var now = Utils.GetUnixTime();

                slot.MeltingStartTime =
                    Utils.FromUnix((now - (long)Math.Floor(timeSpend)));

                ResponseData = new ResponseDataWorkShopSlotTutorialForceComplete(necessaryAmount, 0);
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
                if (staticBuff == null)
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
                if (maxBuff.Value.ContainsKey(BuffValueType.Melting))
                    return maxBuff.Value[BuffValueType.Melting];
            }

            return 1;
        }
    }
}