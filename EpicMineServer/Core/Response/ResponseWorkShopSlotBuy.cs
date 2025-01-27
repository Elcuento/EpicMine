using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseWorkShopSlotBuy : Response<RequestDataWorkshopSlotBuy>
    {

        public ResponseWorkShopSlotBuy(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
         Log("COME " + Value.Number);

            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var staticWorkshopSlotsCount = staticData.WorkshopSlots.Count;
                if (staticWorkshopSlotsCount <= Value.Number)
                {
                    LogError("Slots already more than static number");
                    return false;
                }

                if (Value.SlotType == WorkShopSlotType.Ore)
                {
                    if (Peer.Player.Data.Workshop.Slots.Count > Value.Number)
                    {
                        LogError("Slots already more than number");
                        return false;
                    }

                    if (staticData.WorkshopSlots[Value.Number].PriceCurrencyType == CurrencyType.Crystals)
                    {
                        if (!Peer.IsCurrencyExist(CurrencyType.Crystals,
                            staticData.WorkshopSlots[Value.Number].PriceAmount))
                        {
                            Peer.SendUpdateWalletCrystals();
                            LogError("Not enough crystals");
                            return false;
                        }

                        if (!Peer.SubsTractCurrency(CurrencyType.Crystals,
                            staticData.WorkshopSlots[Value.Number].PriceAmount))
                        {
                            Peer.SendUpdateWalletCrystals();
                            LogError("Not enough crystals");
                            return false;
                        }

                    }

                   
                    while (Peer.Player.Data.Workshop.Slots.Count <= Value.Number)
                    {
                        var slot = new CommonDLL.Dto.WorkshopSlot
                            { ItemId = "", MeltingStartTime = null, NecessaryAmount = 0 };


                        Peer.Player.Data.Workshop.Slots.Add(slot);
                    }
                }
                else if (Value.SlotType == WorkShopSlotType.Shard)
                {
                    if (Peer.Player.Data.Workshop.SlotsShard.Count > Value.Number)
                    {
                        LogError("Slots shard already more than number");
                        return false;
                    }

                    if (staticData.WorkshopSlots[Value.Number].PriceCurrencyType == CurrencyType.Crystals)
                    {
                        if (!Peer.IsCurrencyExist(CurrencyType.Crystals,
                            staticData.WorkshopSlots[Value.Number].PriceAmount))
                        {
                            LogError("Currency not exist for slots shard");
                            return false;
                        }

                        if (!Peer.SubsTractCurrency(CurrencyType.Crystals,
                            staticData.WorkshopSlots[Value.Number].PriceAmount))
                        {
                            Log("Not enough crystals for slots shards");
                            return false;
                        }

                    }

                    while (Peer.Player.Data.Workshop.SlotsShard.Count <= Value.Number)
                    {
                        var slot = new CommonDLL.Dto.WorkshopSlot
                            { ItemId = "", MeltingStartTime = null, NecessaryAmount = 0 };

                        Peer.Player.Data.Workshop.SlotsShard.Add(slot);
                    }

                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}