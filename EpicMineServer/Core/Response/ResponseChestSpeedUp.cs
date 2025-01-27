using System;
using AMTServer.Common;
using AMTServerDLL.Dto;
using Chest = CommonDLL.Dto.Chest;


namespace AMTServer.Core.Response
{
    public class ResponseChestSpeedUp : Response<SendData>
    {

        public ResponseChestSpeedUp(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }


        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var chestBreakingSpeedUpMinutes = staticData.Configs.Burglar.ChestBreakingSpeedUpMinutes;

                Chest chest = null;

                foreach (var burglarChest in Peer.Player.Data.Burglar.Chests)
                {
                    if (burglarChest.StartBreakingTime != null)
                    {
                        chest = burglarChest;
                    }

                }

                if (chest?.StartBreakingTime != null)
                {
                    var userBreakingChestStartTime = Utils.GetUnixTime((DateTime)chest.StartBreakingTime);
                    userBreakingChestStartTime -= chestBreakingSpeedUpMinutes * 60;

                    chest.StartBreakingTime = Utils.FromUnix(userBreakingChestStartTime);
                }

                Peer.SavePlayer();

                return true;
            }


        }
    }
}