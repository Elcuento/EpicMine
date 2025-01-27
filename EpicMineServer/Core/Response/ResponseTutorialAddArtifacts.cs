using AMTServerDLL.Dto;


namespace AMTServer.Core.Response
{
    public class ResponseTutorialAddArtifacts : Response<RequestDataChestAdd>
    {

        public ResponseTutorialAddArtifacts(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var tierOpenCost = staticData.Configs.Dungeon.TierOpenArtefactsCost;

                var giftedArtefacts = tierOpenCost - Peer.Player.Data.Artifacts;

                Peer.Player.Data.Artifacts += giftedArtefacts;

                ResponseData = new AMTServerDLL.Dto.ResponseTutorialAddArtifacts(giftedArtefacts);
                Peer.SavePlayer();

                return true;
               
            }
        }
    }
}