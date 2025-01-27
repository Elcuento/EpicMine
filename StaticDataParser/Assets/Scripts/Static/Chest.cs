namespace BlackTemple.EpicMine.Static
{
    public class Chest
    {
        public int TierNumber { get; }

        public string A1ItemId { get; }

        public string A2ItemId { get; }

        public int AAmountMin { get; }

        public int AAmountMax { get; }

        public string B1ItemId { get; }

        public string B2ItemId { get; }

        public int BAmountMin { get; }

        public int BAmountMax { get; }

        public string C1ItemId { get; }

        public string C2ItemId { get; }

        public int CAmountMin { get; }

        public int CAmountMax { get; }

        public int EAmountMin { get; }

        public int EAmountMax { get; }

        public int HiltDropCategory { get; }

        public Chest(int tierNumber, string a1ItemId, string a2ItemId, int aAmountMin, int aAmountMax, string b1ItemId,
            string b2ItemId, int bAmountMin, int bAmountMax, string c1ItemId, string c2ItemId, int cAmountMin,
            int cAmountMax, int eAmountMin, int eAmountMax, int hiltDropCategory)
        {
            TierNumber = tierNumber;
            A1ItemId = a1ItemId.ToLower();
            A2ItemId = a2ItemId.ToLower();
            AAmountMin = aAmountMin;
            AAmountMax = aAmountMax;
            B1ItemId = b1ItemId.ToLower();
            B2ItemId = b2ItemId.ToLower();
            BAmountMin = bAmountMin;
            BAmountMax = bAmountMax;
            C1ItemId = c1ItemId.ToLower();
            C2ItemId = c2ItemId.ToLower();
            CAmountMin = cAmountMin;
            CAmountMax = cAmountMax;
            EAmountMin = eAmountMin;
            EAmountMax = eAmountMax;
            HiltDropCategory = hiltDropCategory;
        }
    }
}