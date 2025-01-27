namespace AMTServerDLL.Dto
{
    public class RequestDataPvpCreate : SendData
    {
        public int Arena;

        public RequestDataPvpCreate(int arena)
        {
            Arena = arena;
        }
    }
}