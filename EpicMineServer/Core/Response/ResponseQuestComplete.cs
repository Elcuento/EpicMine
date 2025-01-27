using System.Collections.Generic;
using AMTServerDLL.Dto;
using CommonDLL.Static;
using Quest = CommonDLL.Dto.Quest;
using QuestTask = CommonDLL.Dto.QuestTask;

namespace AMTServer.Core.Response
{
    public class ResponseQuestComplete : Response<RequestDataQuestComplete>
    {

        public ResponseQuestComplete(ClientPeer peer, Package pack) : base(peer, pack)
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

                if (staticQuest.RewardFeatures != null && staticQuest.RewardFeatures.Count > 0)
                {
                    foreach (var staticQuestRewardFeature in staticQuest.RewardFeatures)
                    {
                        if (!Peer.Player.Data.Features.FeaturesList.Contains(staticQuestRewardFeature))
                            Peer.Player.Data.Features.FeaturesList.Add(staticQuestRewardFeature);
                    }
                }

                var playerQuest = Peer.Player.Data.Quests.QuestList.Find(x => x.id == Value.Id);
                if (playerQuest == null)
                {
                    Peer.Player.Data.Quests.QuestList.Add(new Quest(Value.Id, new List<QuestTask>(), QuestStatusType.Completed, false ));
                }
                else
                {
                    playerQuest.status = QuestStatusType.Completed;
                }

                Peer.SavePlayer();
                return true;
            }
        }
    }
}