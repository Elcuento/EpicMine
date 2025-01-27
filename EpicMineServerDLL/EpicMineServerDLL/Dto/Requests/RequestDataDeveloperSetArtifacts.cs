namespace AMTServerDLL.Dto
{
    public class RequestDataDeveloperSetArtifacts : SendData
    {
        public int Val;

        public RequestDataDeveloperSetArtifacts(int val)
        {
            Val = val;
        }
    }
}