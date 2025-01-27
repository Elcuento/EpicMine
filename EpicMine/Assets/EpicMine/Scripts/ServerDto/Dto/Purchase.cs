using System;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
    public class Purchase
    {
        public string Receipt;
        public DateTime Time;
        public ShopPurchaseStatus Status;
        public string Id;

        public Purchase(string id, string receipt, ShopPurchaseStatus status, DateTime time)
        {
            Receipt = receipt;
            Time = time;
            Status = status;
            Id = id;
        }
    }
}
