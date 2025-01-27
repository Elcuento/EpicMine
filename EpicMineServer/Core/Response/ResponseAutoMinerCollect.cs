using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseAutoMinerCollect : Response<RequestDataAutoMinerCollect>
    {

        public ResponseAutoMinerCollect(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {

                var staticData = Peer.GetStaticData();

                var autoMinerLevelData = Peer.Player.Data.AutoMiner.CapacityLevel;


                var capacity = 0;

                if (autoMinerLevelData != null)
                {
                    var autoMinerLevelVal = autoMinerLevelData.Number;
                    capacity = staticData.AutoMinerCapacityLevels[autoMinerLevelVal].Capacity;
                }
                else
                {
                    capacity = staticData.AutoMinerCapacityLevels[0].Capacity;
                }

                if (capacity > Value.Amount)
                    capacity = Value.Amount;

                ResponseData = new ResponseDataAutoMinerCollect(capacity);

                Peer.SavePlayer();
            }

            return true;
        }
    }
}