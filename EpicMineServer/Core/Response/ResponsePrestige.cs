using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using Tier = CommonDLL.Dto.Tier;

namespace AMTServer.Core.Response
{
    public class ResponsePrestige : Response<SendData>
    {

        public ResponsePrestige(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var playerData = Peer.Player.Data;

                var staticTiersCount = staticData.Tiers.Count;
                var lastOpenedTierNumber = playerData.Dungeon.Tiers.Count - 1;

                if (lastOpenedTierNumber < staticTiersCount - 1)
                {
                    LogError("Tiers does not completed");
                    return false;
                }

                var isLastBossComplete = playerData.Dungeon.Tiers[lastOpenedTierNumber].Mines[5].IsComplete;
                if (!isLastBossComplete)
                {
                    LogError("Last mine does not completed");
                    return false;
                }

                var userPrestigeLevel = playerData.Prestige;


                var pickaxes = 0;
                foreach (var pickaxe in staticData.Pickaxes)
                {
                    if (pickaxe.Type == PickaxeType.God)
                        pickaxes++;
                }

                userPrestigeLevel++;
                if (userPrestigeLevel > pickaxes)
                {
                    LogError("Invalid prestige level");
                    return false;
                }

                playerData.Prestige = userPrestigeLevel;

                var itemsLeft = new List<Item>();

                foreach (var inventoryItem in playerData.Inventory.Items)
                {
                    if (inventoryItem.Id.Contains("tnt_1") ||
                        inventoryItem.Id.Contains("shard") ||
                        inventoryItem.Id.Contains("potion"))
                    {
                        itemsLeft.Add(inventoryItem);
                    }
                }

                playerData.Artifacts = 0;
                playerData.Dungeon.Tiers = new List<Tier>() {new Tier(0, true)};
                playerData.Abilities.Acid = null;
                playerData.Abilities.ExplosiveStrike = null;
                playerData.Abilities.Freezing = null;
                playerData.Skills.Critical = null;
                playerData.Skills.Damage = null;
                playerData.Skills.Fortune = null;
                playerData.Inventory = new Inventory();
                playerData.Quests = new Quests();
                playerData.Workshop = new Workshop();
                playerData.Burglar = new Burglar();
                playerData.DailyTasks = new DailyTasks();
                playerData.Gifts = new Gifts();
                playerData.Burglar = new Burglar();

                playerData.Inventory.Items = itemsLeft;

                Peer.SavePlayer();

                return true;
            }
        }
    }
}