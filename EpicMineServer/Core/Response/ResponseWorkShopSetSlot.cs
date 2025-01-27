using System;
using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseWorkShopSetSlot : Response<RequestDataWorkshopSetSlot>
    {

        public ResponseWorkShopSetSlot(ClientPeer peer, Package pack) : base(peer, pack)
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

                var userWorkshopSlotsData = Value.SlotType
                                            == WorkShopSlotType.Shard
                    ? Peer.Player.Data.Workshop.SlotsShard
                    : Peer.Player.Data.Workshop.Slots;

                if (Value.Number >= userWorkshopSlotsData.Count && Value.Number != 0)
                {
                    LogError("Wrong slot count");
                    return false;
                }

                CommonDLL.Dto.WorkshopSlot userWorkshopSlotData = null;

                var recipeData = staticData.Recipes.Find(x => x.Id == Value.Item && x.Type == Value.Type);


                if (recipeData == null)
                {
                    LogError("Recipe not exist " + Value.Item);
                    return false;
                }

                if (Value.Number >= userWorkshopSlotsData.Count)
                {
                    while (Peer.Player.Data.Workshop.Slots.Count <= Value.Number)
                    {
                        userWorkshopSlotsData.Add(new CommonDLL.Dto.WorkshopSlot());
                    }
                }
                else
                {
                    userWorkshopSlotData = userWorkshopSlotsData[Value.Number];

                    if (userWorkshopSlotData != null && !string.IsNullOrEmpty(userWorkshopSlotData.ItemId))
                    {
                        var slotRecipeData = staticData.Recipes.Find(x =>
                            x.Id == userWorkshopSlotData.ItemId && x.Type == userWorkshopSlotData.Type);

                        if (slotRecipeData != null)
                        {
                            LogError("Slot is busy");
                            return false;
                        }
                    }
                }

                userWorkshopSlotsData[Value.Number]
                    = new CommonDLL.Dto.WorkshopSlot
                    {
                        Type = Value.Type,
                        MeltingStartTime = DateTime.UtcNow,
                        ItemId = Value.Item,
                        NecessaryAmount = Value.NecessaryAmount
                    };

                Peer.SavePlayer();

                return true;
            }
        }
    }
}