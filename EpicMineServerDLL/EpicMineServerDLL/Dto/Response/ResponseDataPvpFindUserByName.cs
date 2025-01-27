namespace AMTServerDLL.Dto
{

    public class ResponseDataPvpFindUserByName : SendData
    {
        public string Id;
        public string Pickaxe;
        public long Rating;
        public long LastTimeOnline;

        public ResponseDataPvpFindUserByName(string id,string nickName, string pickaxe, long rating, long lastTimeOnline)
        {
            Id = id;
            Pickaxe = pickaxe;
            Rating = rating;
            LastTimeOnline = lastTimeOnline;
        }

    }
}