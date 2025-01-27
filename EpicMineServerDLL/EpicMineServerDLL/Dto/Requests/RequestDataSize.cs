using CommonDLL.Static;

namespace AMTServerDLL.Dto
{
    public class RequestDataSize : SendData
    {
        public string Strings;

        public RequestDataSize(string strings)
        {
            Strings = strings;
        }
    }
}