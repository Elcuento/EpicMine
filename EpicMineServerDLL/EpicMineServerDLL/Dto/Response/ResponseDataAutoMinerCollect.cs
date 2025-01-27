namespace AMTServerDLL.Dto
{
    public class ResponseDataAutoMinerCollect : SendData
    {
        public int Amount;

        public ResponseDataAutoMinerCollect(int amount)
        {
            Amount = amount;
        }
    }
}