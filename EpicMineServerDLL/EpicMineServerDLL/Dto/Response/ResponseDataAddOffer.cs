namespace AMTServerDLL.Dto
{
    public class ResponseDataAddOffer : SendData
    {
        public long Data;

        public ResponseDataAddOffer(long date)
        {
            Data = date;
        }
    }
}