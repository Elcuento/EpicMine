namespace AMTServerDLL.Dto
{
    public class RequestDataPvpJoinCreate : SendData
    {
        public int Arena;

        public RequestDataPvpJoinCreate(int arena)
        {
            Arena = arena;
        }
    }
}