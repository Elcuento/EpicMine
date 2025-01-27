using System.Collections.Generic;

namespace BlackTemple.EpicMine.Static
{
    public struct Configs
    {
        public ShopConfigs Shop;
        public WorkshopConfigs Workshop;
        public BurglarConfigs Burglar;
        public EnchantedChestsConfigs EnchantedChests;
        public DailyTasksConfigs DailyTasks;
        public WindowsConfigs Windows;
        public AppodealConfigs Appodeal;
        public DungeonConfigs Dungeon;
        public StartPackConfigs StartPack;
        public GiftsConfigs Gifts;
        public CustomGifts CustomGifts;
        public PvpConfigs Pvp;
        public AdsConfig Ads;
        public int Version;
        public int GameDataSynchronizePeriod;
    }

    public struct CustomGifts
    {
        public int UnlockSecondTierCrystalsAmount;
        public int FirstTradeAffairsDailyTaskCompleteCrystalsAmount;
    }

    public struct EnchantedChestsConfigs
    {
        public Dictionary<EnchantedChestType, EnchantedChestConfigs> Chests;

        public float Chance;

        public EnchantedChestDropConfigs Drop;
    }

    public struct EnchantedChestDropConfigs
    {
        public float Min;
        public float Max;
    }

    public struct PvpConfigs
    {
        public int ChestDoubleBonusCrystalsCost;
    }

    public struct EnchantedChestConfigs
    {
        public float Chance;
        public int Price;
    }

    public struct AdsConfig
    {
        public bool AdsInternalShow;
        public string AdsUnlockShopPackId;
        public int AdsCoolDown;
        public int AdsTierDisable;
        public int AdsShopOfferPeriod;
    }

    public struct GiftsConfigs
    {
        public int DailyCount;
        public int PeriodInMinutes;
        public int SimpleResourcesCoefficient;
        public int RoyalRandomItemCount;
        public int RoyalResourcesCoefficient;
        public int RoyalArtefactsCount;
        public int SimpleArtefactsCount;
    }

    public struct ShopConfigs
    {
        public List<Iap> Iaps;
        public float ResourceMaxSale;
        public List<int> GoldTiersDozenMultipliers;
    }

    public struct StartPackConfigs
    {
        public StartPackCurrenciesConfigs Currencies;
    }

    public struct StartPackCurrenciesConfigs
    {
        public int Gold;
        public int Crystals;
    }
    public struct MonstersConfig
    {
        public int HardcoreHealthCoefficient;
    }

    public struct DungeonConfigs
    {
        public int TierOpenArtefactsCost;
        public int ContinueCrystalPrice;
        public MineConfigs Mines;
    }

    public struct MineConfigs
    {
        public int MaxHealth;
        public int MaxEnergy;
        public AttackHitsConfigs AttackHits;
        public AttackPointsConfigs AttackPoints;
        public float CritDamageCoefficient;
        public RatingConfigs Rating;
        public WallsConfigs Walls;
        public MonstersConfig Monsters;
        public List<float> Combo;
    }


    public struct AttackHitsConfigs
    {
        public float MissAccuracyCoefficient;
        public float InnerAccuracyCoefficient;
    }

    public struct AttackPointsConfigs
    {
        public AttackPointsProbabilitiesConfigs Probabilities;
    }

    public struct RatingConfigs
    {
        public List<int> Hardcore;
    }

    public struct WallsConfigs
    {
        public int DefaultDropItemAmount;
        public int FortuneDropItemAmountCoefficient;
        public int HardcoreDropItemAmountCoefficient;
        public int HardcoreHealthCoefficient;
    }

    public struct AttackPointsProbabilitiesConfigs
    {
        public AttackPointsPositionsProbabilitiesConfigs Positions;
        public AttackPointsTypesProbabilitiesConfigs Types;
    }

    public struct AttackPointsPositionsProbabilitiesConfigs
    {
        public AttackPointPositionProbabilityConfigs Default;
    }

    public struct AttackPointPositionProbabilityConfigs
    {
        public int CrossChance;
    }

    public struct AttackPointsTypesProbabilitiesConfigs
    {
        public int DonateWallSectionsPassedToDisable;
        public int HelpPickaxeDestroyedAmountToEnable;
        public int HelpSectionNumberLessToEnable;
        public int HelpSectionNumberToDisable;
        public int BrakingMineCompleteAmountToEnable;
        public int BrakingPickaxeDestroyAmountToDisable;
        public AttackPointTypeProbabilityConfigs Default;
        public AttackPointTypeProbabilityConfigs Donate;
        public AttackPointTypeProbabilityConfigs Help;
        public AttackPointTypeProbabilityConfigs Braking;
    }

    public struct AttackPointTypeProbabilityConfigs
    {
        public int EnergyPointChance;
        public int HealthPointChance;
    }

    public struct InstantDamageEnergyAbilityConfigs
    {
        public int Cost;
        public float Percent;
    }

    public struct IncreaseDamageEnergyAbilityConfigs
    {
        public int Cost;
        public int MaxStacksCount;
        public float Duration;
        public float Percent;
    }

    public struct TickingDamageEnergyAbilityConfigs
    {
        public int Cost;
        public float Duration;
        public float Percent;
    }


    public struct WorkshopConfigs
    {
        public int CollectCrystalPrice;
        public WorkshopForceCompletePricesConfigs ForceCompletePrices;
    }

    public struct WorkshopForceCompletePricesConfigs
    {
        public int LessThanOneMinute;
        public int LessThanFiveMinutes;
        public int LessThanTenMinutes;
        public int MoreThanTenMinutesForEveryTen;
    }

    public struct BurglarConfigs
    {
        public int ChestBreakingSpeedUpMinutes;
        public int ChestForceCompletePricePer30Minutes;
        public Dictionary<ChestType, ChestConfigs> Chests;
    }

    public struct ChestItemDropConfigs
    {
        public int Min;
        public int Max;
        public float Chance;
    }

    public struct ChestConfigs
    {
        public int BreakingTimeInMinutes;
        public float DropChance;
        public Dictionary<ChestItemDropType, ChestItemDropConfigs> Drop;
    }

    public struct Iap
    {
        public string Id;
        public string Pack;
        public ShopProductType Type;
        public PlatformType Platform;

    }

    public struct DailyTasksConfigs
    {
        public int MaxCount;
    }


    public struct WindowsConfigs
    {
        public WindowPickaxeDestroyedConfigs PickaxeDestroyed;
    }

    public struct WindowPickaxeDestroyedConfigs
    {
        public int ContinueAvailableSectionsCount;
        public float ContinueAvailableBossHealthPercent;
        public float ContinueAvailableTime;
    }


    public struct AppodealConfigs
    {
        public AppodealKeysConfigs Dev;
        public AppodealKeysConfigs Live;
    }

    public struct AppodealKeysConfigs
    {
        public string AndroidAppKey;
        public string IosAppKey;
    }
}