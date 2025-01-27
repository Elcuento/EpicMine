namespace AMTServerDLL.Dto
{
    public class ResponseDataAddCrystal : SendData
    {
        public long Quantity;

        public ResponseDataAddCrystal(long quantity)
        {
            Quantity = quantity;
        }

    }
}