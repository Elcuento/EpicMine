namespace AMTServerDLL.Dto
{
    public class RequestDataBugReport : SendData
    {
        public string Str;
        public string DeviceId;

        public RequestDataBugReport(string deviceId, string str)
        {
            Str = str;
            DeviceId = deviceId;
        }
    }
}