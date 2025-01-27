namespace AMTServerDLL.Dto
{
    public class RequestDataAutoMinerCapacityLevelUp : SendData
    {
        public int Level;

        public RequestDataAutoMinerCapacityLevelUp(int level)
        {
            Level = level;
        }
    }
}