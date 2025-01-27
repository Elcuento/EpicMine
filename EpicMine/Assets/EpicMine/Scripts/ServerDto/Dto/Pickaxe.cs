namespace CommonDLL.Dto
{
    public class Pickaxe
    {
        public string Id;

        public bool IsCreated;

        public bool IsHiltFound;

        public Pickaxe(string id, bool isCreated, bool isHiltFound)
        {
            Id = id;
            IsCreated = isCreated;
            IsHiltFound = isHiltFound;
        }
    }
}