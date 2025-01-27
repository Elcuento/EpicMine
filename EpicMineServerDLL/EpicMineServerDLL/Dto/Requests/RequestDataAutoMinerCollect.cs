namespace AMTServerDLL.Dto
{
    public class RequestDataAutoMinerCollect : SendData
    {
        public int Amount;

        public RequestDataAutoMinerCollect(int amount)
        {
            Amount = amount;
        }
    }
 
}