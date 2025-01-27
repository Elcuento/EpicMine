namespace AMTServerDLL.Dto
{
    public class ResponseDataWalletCrystalsUpdate : SendData
    {
        public long Crystals;

        public ResponseDataWalletCrystalsUpdate(long crystals)
        {
            Crystals = crystals;
        }
    }
}