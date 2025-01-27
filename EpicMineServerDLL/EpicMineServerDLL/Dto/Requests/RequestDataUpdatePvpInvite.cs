namespace AMTServerDLL.Dto
{
    public class RequestDataUpdatePvpInvite : SendData
    {
        public bool State;

        public RequestDataUpdatePvpInvite(bool state)
        {
            State = state;
        }
    }
}