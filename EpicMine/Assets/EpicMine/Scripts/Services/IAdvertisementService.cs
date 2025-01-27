using BlackTemple.Common;

namespace BlackTemple.EpicMine
{
    public interface IAdvertisementService
    {
        event EventHandler<AdSource, bool, string, int> OnRewardedVideoCompleted;

        event EventHandler OnInterstitialCompleted;

        void Initialize(string userId = "");

        void ShowRewardedVideo(AdSource adSource);

        void ShowInterstitialVideo(bool withCoolDown = true);

        void ShowForceAds(bool withCoolDown = true);

        void DisableForceAds(bool state);

        bool IsForceAdsAvailable();

        int GetCurrentForceAdsPeriod();
    }

    public enum AdSource
    {
        SpeedupChestBreaking,
        RefillPickaxeHealth,
        MultiplySmeltedResources,
        OpenEnchantedChest,
        UnlockPickaxe,
        UnlockTorch,
        MultiplyPvpChestReward,
        GoldAdReward,
        CrystalAdReward,
    }
}