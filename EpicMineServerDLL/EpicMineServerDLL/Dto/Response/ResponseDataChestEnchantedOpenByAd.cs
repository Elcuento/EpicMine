namespace AMTServerDLL.Dto
{
    public class ResponseDataChestEnchantedOpenByAd : SendData
    {
        public int Artifacts;

        public ResponseDataChestEnchantedOpenByAd(int artifacts)
        {
            Artifacts = artifacts;
        }
    }
}