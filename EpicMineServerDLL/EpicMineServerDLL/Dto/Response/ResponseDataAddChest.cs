namespace AMTServerDLL.Dto
{
    public class ResponseDataAddChest : SendData
    {
        public string Id;

        public ResponseDataAddChest(string id)
        {
            Id = id;
        }
    }
}