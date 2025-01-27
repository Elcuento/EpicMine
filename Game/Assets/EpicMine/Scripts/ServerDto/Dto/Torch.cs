namespace CommonDLL.Dto
{
    public class Torch
    {
        public string Id;
        public bool IsCreated;

        public Torch(string id, bool isCreated)
        {
            Id = id;
            IsCreated = isCreated;
        }
    }
}