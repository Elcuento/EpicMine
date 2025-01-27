using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataLogin : SendData
    {
        public Player Player;

        public string GoogleId;
        public string FaceBookId;
        public string FireBaseId;

        public AppVersionInfo Version;

        public ResponseDataLogin(Player player, AppVersionInfo version, string googleId, string faceBookId, string fireBaseId) 
        {
            Player = player;
            Version = version;
            GoogleId = googleId;
            FaceBookId = faceBookId;
            FireBaseId = fireBaseId;
        }

    }
}