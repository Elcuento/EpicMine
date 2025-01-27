namespace AMTServerDLL.Dto
{
    public class RequestDataQuestComplete : SendData
    {
        public string Id;

        public RequestDataQuestComplete(string id)
        {
            Id = id;
        }
    }
 
}