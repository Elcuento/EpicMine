namespace AMTServerDLL.Dto
{

    public class ResponseDataChestOpenForce : SendData
    {
        public int SpendCrystals;
        public int DroppedCrystals;
        public int DroppedArtifacts;

        public ResponseDataChestOpenForce(int spendCrystals, int droppedArtifacts, int droppedCrystals)
        {
            SpendCrystals = spendCrystals;
            DroppedArtifacts = droppedArtifacts;
            DroppedCrystals = droppedCrystals;
        }

    }
}