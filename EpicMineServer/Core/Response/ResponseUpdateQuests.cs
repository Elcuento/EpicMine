using System;
using System.Collections.Generic;
using AMTServerDLL;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateQuests : Response<RequestDataUpdateQuests>
    {

        public ResponseUpdateQuests(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Quests == null)
                {
                    Peer.Player.Data.Quests = new Quests()
                    {
                      QuestList = new List<Quest>()
                    };
                }

                foreach (var item in Value.Items)
                {
                    var itemData = Peer.Player.Data.Quests.QuestList.Find(x => x.id == item.id);
                    if (itemData != null)
                    {
                        Peer.Player.Data.Quests.QuestList.Remove(itemData);
                    }


                    Peer.Player.Data.Quests.QuestList.Add(item);

                }

                Peer.SavePlayer();
            }

            return true;
        }
    }
}