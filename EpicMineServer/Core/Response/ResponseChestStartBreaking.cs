using System;
using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseChestStartBreaking : Response<RequestDataChestStartBreaking>
    {

        public ResponseChestStartBreaking(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {

                foreach (var chest in Peer.Player.Data.Burglar.Chests)
                {
                    if (chest.StartBreakingTime != null)
                    {
                        LogError("Some chest already breaking");
                        return false;
                    }
                }
                
                var chestData = Peer.Player.Data.Burglar.Chests.Find(x => x.Id == Value.Id);

                if (chestData == null)
                {
                    LogError("Chest not exist");
                    return false;
                }

                chestData.StartBreakingTime = DateTime.UtcNow;

                Peer.SavePlayer();

                return true;
            }
        }
    }
}