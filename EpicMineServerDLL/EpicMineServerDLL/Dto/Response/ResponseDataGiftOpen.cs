namespace AMTServerDLL.Dto
{
    public class ResponseDataGiftOpen : SendData
    {
        public int Artifacts;

        public ResponseDataGiftOpen(int artifacts)
        {
            Artifacts = artifacts;
        }
    }
}