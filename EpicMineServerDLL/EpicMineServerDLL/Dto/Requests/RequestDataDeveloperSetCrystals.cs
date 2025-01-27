namespace AMTServerDLL.Dto
{
    public class RequestDataDeveloperSetCrystals : SendData
    {
        public int Val;

        public RequestDataDeveloperSetCrystals(int val)
        {
            Val = val;
        }
    }
}