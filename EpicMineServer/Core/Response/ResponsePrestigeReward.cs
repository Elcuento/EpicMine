using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponsePrestigeReward : Response<SendData>
    {

        public ResponsePrestigeReward(ClientPeer peer, Package pack) : base(peer, pack)
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

                var giftPickaxes = new List<CommonDLL.Static.Pickaxe>();
                foreach (var pic in staticData.Pickaxes)
                {
                    if(pic.Type == PickaxeType.God)
                    giftPickaxes.Add(pic);
                }

                userPrestigeLevel++;
                if (userPrestigeLevel > giftPickaxes.Count)
                {
                    LogError("Invalid prestige level");
                    return false;
                }

                var giftedPickaxe = giftPickaxes[userPrestigeLevel - 1];

                var existPickaxe = playerData.Blacksmith.Pickaxes.Find(x => x.Id == giftedPickaxe.Id);
                if (existPickaxe != null)
                {
                    LogError("reward already taken");
                    return false;
                }

                playerData.Blacksmith.Pickaxes.Add(new CommonDLL.Dto.Pickaxe(giftedPickaxe.Id, true, true));

                Peer.SavePlayer();

                ResponseData = new ResponseDataPrestigeReward(giftedPickaxe.Id);

                return true;
            }
        }
    }
}