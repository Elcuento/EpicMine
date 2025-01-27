namespace AMTServerDLL.Dto
{

    public class ResponseDataChestOpenTutorial : SendData
    {
        public int SpendCrystals;
        public int DroppedCrystals;
        public int DroppedArtifacts;

        public ResponseDataChestOpenTutorial(int spendCrystals, int droppedArtifacts, int droppedCrystals)
        {
            SpendCrystals = spendCrystals;
            DroppedArtifacts = droppedArtifacts;
            DroppedCrystals = droppedCrystals;
        }

    }
}