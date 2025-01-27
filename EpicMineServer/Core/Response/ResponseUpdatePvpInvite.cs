using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseQuestSetTracking : Response<RequestDataQuestSetTracking>
    {

        public ResponseQuestSetTracking(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            var quest = Peer.Player.Data.Quests.QuestList.Find(x => x.id == Value.Id);

            if (quest != null)
                quest.isTracking = Value.State;

            Peer.SavePlayer();

            return true;
        }
    }
}