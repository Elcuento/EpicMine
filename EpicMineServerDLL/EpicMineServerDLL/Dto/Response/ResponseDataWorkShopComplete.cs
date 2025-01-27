namespace AMTServerDLL.Dto
{
    public class ResponseDataWorkShopComplete : SendData
    {
        public int NecessaryAmount;
        public int Cost;

        public ResponseDataWorkShopComplete(int necessaryAmount, int cost)
        {
            NecessaryAmount = necessaryAmount;
            Cost = cost;
        }
    }
}