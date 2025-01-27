using AMTServerDLL.Dto;

namespace AMTServer.Core.Response
{
    public class ResponseUpdateAdditionalInfo : Response<RequestDataUpdateAdditionalInfo>
    {

        public ResponseUpdateAdditionalInfo(ClientPeer peer, Package pack) : base(peer, pack)
        {
        }

        protected override bool OnProcess()
        {
            lock (Peer.Player)
            {
                Peer.Player.Data.AdditionalInfo.IsFourEnergyAbilityWindowShowed = Value.IsFourEnergyAbilityWindowShowed;
                Peer.Player.Data.AdditionalInfo.IsThirdEnergyAbilityWindowShowed =
                    Value.IsThirdEnergyAbilityWindowShowed;
                Peer.Player.Data.AdditionalInfo.IsSecondEnergyAbilityWindowShowed =
                    Value.IsSecondEnergyAbilityWindowShowed;

                Peer.SavePlayer();
            }

            return true;
        }
    }
}