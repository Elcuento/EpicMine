namespace AMTServerDLL.Dto
{
    public class ResponseDataWorkShopCollect : SendData
    {
        public int CollectAmount;

        public ResponseDataWorkShopCollect(int collectAmount)
        {
            CollectAmount = collectAmount;
        }
    }
}