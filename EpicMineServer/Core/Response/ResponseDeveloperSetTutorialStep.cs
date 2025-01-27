using System;
using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseDeveloperSetTutorialStep : Response<RequestDataDeveloperSetTutorialStep>
    {

        public ResponseDeveloperSetTutorialStep(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Tutorial == null)
                {
                    Peer.Player.Data.Tutorial = new Tutorial();
                }

                Peer.Player.Data.Tutorial.LastCompleteStep = Value.Id;
                Peer.SavePlayer();
            }

            return true;
        }
    }
}