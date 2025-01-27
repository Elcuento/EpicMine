using CommonDLL.Dto;
namespace AMTServerDLL.Dto
{

    public class ResponseDataGetProfile : SendData
    {
        public Player Player;
        public string FireBaseId;

        public ResponseDataGetProfile(Player player, string fireBaseId)
        {
            Player = player;
            FireBaseId = fireBaseId;
        }

    }
}