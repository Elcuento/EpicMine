using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{

    public class RequestDataTransferProfile : SendData
    {
        public Player Player;

        public string FaceBookId;
        public string GoogleId;
        public string FireBaseId;

        public RequestDataTransferProfile(Player player, string faceBookId, string googleId, string fireBaseId)
        {
            Player = player;
            FaceBookId = faceBookId;
            FireBaseId = fireBaseId;
            GoogleId = googleId;
        }
    }
}