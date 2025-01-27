using AMTServerDLL.Dto;
using CommonDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateTutorialStepId : Response<RequestDataUpdateTutorialStepId>
    {

        public ResponseUpdateTutorialStepId(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                if (Peer.Player.Data.Tutorial == null)
                {
                    Peer.Player.Data.Tutorial = new Tutorial()
                    {
                        
                    };
                }

                Peer.Player.Data.Tutorial.LastCompleteStep = Value.StepId;
                Peer.SavePlayer();
            }

            return true;
        }
    }
}