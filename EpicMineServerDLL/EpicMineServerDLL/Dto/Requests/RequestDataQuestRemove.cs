namespace AMTServerDLL.Dto
{
    public class RequestDataQuestRemove : SendData
    {
        public string Id;

        public RequestDataQuestRemove(string id)
        {
            Id = id;
        }
    }
 
}