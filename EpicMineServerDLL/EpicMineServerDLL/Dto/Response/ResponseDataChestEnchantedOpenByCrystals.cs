namespace AMTServerDLL.Dto
{
    public class ResponseDataChestEnchantedOpenByCrystals : SendData
    {
        public int Artifacts;

        public int SpendCrystals;

        public ResponseDataChestEnchantedOpenByCrystals(int artifacts, int spendCrystals)
        {
            Artifacts = artifacts;
            SpendCrystals = spendCrystals;
        }
    }
}