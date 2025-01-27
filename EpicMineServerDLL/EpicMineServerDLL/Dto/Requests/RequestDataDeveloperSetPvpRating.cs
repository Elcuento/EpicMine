namespace AMTServerDLL.Dto
{
    public class RequestDataDeveloperSetPvpRating : SendData
    {
        public int Val;

        public RequestDataDeveloperSetPvpRating(int val)
        {
            Val = val;
        }
    }
}