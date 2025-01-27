using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataReloadWorkShop : SendData
    {
        public Workshop Workshop;

        public ResponseDataReloadWorkShop(Workshop workshop)
        {
            Workshop = workshop;
        }
    }
}