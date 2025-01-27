using AMTServerDLL.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateMinerLevels : Response<RequestDataUpdateMinerLevels>
    {

        public ResponseUpdateMinerLevels(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.AutoMiner == null)
                {
                    Peer.Player.Data.AutoMiner = new AutoMiner();
                }

                foreach (var valueSkill in Value.Skills)
                {
                    switch (valueSkill.Key)
                    {
                        case AutoMinerUpgradeType.Capacity:
                            Peer.Player.Data.AutoMiner.CapacityLevel = new AutoMinerCapacityLevel(valueSkill.Value);
                            break;
                        case AutoMinerUpgradeType.Speed:
                            Peer.Player.Data.AutoMiner.SpeedLevel = new AutoMinerSpeedLevel(valueSkill.Value);
                            break;
                    }
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}