namespace BlackTemple.EpicMine
{
    public struct PvpLocalConfig
    {
        public const int DuelTimeOut = 20;

        public const int NewbiePassCoefficient = 2;

        public const int PvpWinChestRequire = 5;
        public const string PvpSimpleChestItemId = "pvp_chest_simple";
        public const string PvpRoyalChestItemId = "pvp_chest_royal";
        public const string PvpWinnerChestItemId = "pvp_chest_winner";

        public const int DefaultPvpMineSectionCount = 8;
        public const int DefaultPvpMineStartTime = 3;
        public const int DefaultPvpMineVersusTime = 3;
        public const int DefaultPvpMineMatchTime = 180;
        public const int DefaultPvpMinePickaxeRestoreTIme = 6;

        public const int SendSectionPassStatisticChance = 50;

        // Bot
        public const int BotConnectTimer = 8;
        public const string BotNamePrefix = "#MBSKY#";
        public const float BotDonatePickaxeExtraDamage = 1.4f;
        public const int BotGuaranteeAppearRating = 10;
        public const int BotGuaranteeWeakRatingMax = 30;
    }
}