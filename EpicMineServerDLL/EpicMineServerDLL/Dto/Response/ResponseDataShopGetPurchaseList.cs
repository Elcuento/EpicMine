using System.Collections.Generic;
using CommonDLL.Dto;

namespace AMTServerDLL.Dto
{
    public class ResponseDataShopGetPurchaseList : SendData
    {
        public List<Purchase> PurchaseList;

        public ResponseDataShopGetPurchaseList(List<Purchase> list)
        {
            PurchaseList = list;
        }
    }
}