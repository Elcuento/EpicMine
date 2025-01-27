namespace AMTServerDLL.Dto
{
    public class ResponseDataBuyPickaxe : SendData
    {
        public int Cost;

        public ResponseDataBuyPickaxe(int cost)
        {
            Cost = cost;
        }
    }
}