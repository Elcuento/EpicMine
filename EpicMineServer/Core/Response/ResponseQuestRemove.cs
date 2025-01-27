using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Static;
using Quest = CommonDLL.Dto.Quest;
using QuestTask = CommonDLL.Dto.QuestTask;

namespace AMTServer.Core.Response
{
    public class ResponseQuestRemove : Response<RequestDataQuestComplete>
    {

        public ResponseQuestRemove(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                var staticData = Peer.GetStaticData();

                var staticQuest = staticData.Quests.Find(x => x.Id == Value.Id);

                if (staticQuest == null)
                    return false;

                Peer.Player.Data.Quests.QuestList.Remove(Peer.Player.Data.Quests.QuestList.Find(x => x.id == Value.Id));

                Peer.SavePlayer();
                return true;
            }
        }
    }
}