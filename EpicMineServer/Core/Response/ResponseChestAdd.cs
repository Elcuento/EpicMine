using System;
using AMTServerDLL.Dto;


namespace AMTServer.Core.Response
{
    public class ResponseChestAdd : Response<RequestDataChestAdd>
    {

        public ResponseChestAdd(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Burglar.Chests.Count >= 4)
                {
                    Log("Already chest >= then 4");
                    return false;
                }
                else
                {
                    var chest = new CommonDLL.Dto.Chest
                        {Id = Guid.NewGuid().ToString(), Level = Value.Level, Type = Value.Type};
                    Peer.Player.Data.Burglar.Chests.Add(chest);

                    ResponseData = new ResponseDataAddChest(chest.Id);
                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}