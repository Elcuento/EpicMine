namespace AMTServerDLL.Dto
{
    public class RequestDataQuestSetTracking : SendData
    {
        public string Id;
        public bool State;

        public RequestDataQuestSetTracking(string id, bool state)
        {
            Id = id;
            State = state;
        }
    }
 
}