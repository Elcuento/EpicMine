namespace BlackTemple.EpicMine
{
    public struct IapPurchaseErrorEvent
    {
        public string ProductId;

        public IapPurchaseErrorEvent(string productId)
        {
            ProductId = productId;
        }
    }
}