namespace AMTServerDLL.Dto
{

    public class ResponseDataChestOpen : SendData
    {
        public int DroppedCrystals;
        public int DroppedArtifacts;

        public ResponseDataChestOpen(int droppedArtifacts, int droppedCrystals)
        {
            DroppedArtifacts = droppedArtifacts;
            DroppedCrystals = droppedCrystals;
        }

    }
}