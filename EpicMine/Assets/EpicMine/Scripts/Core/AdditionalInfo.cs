namespace BlackTemple.EpicMine.Core
{
    public class AdditionalInfo
    {
        public bool IsUnlockSecondTierGiftTaken;

        public bool IsFirstTradeAffairsDailyTaskCompleteGiftTaken;

        public bool IsSecondEnergyAbilityWindowShowed;

        public bool IsThirdEnergyAbilityWindowShowed;

        public bool IsFourEnergyAbilityWindowShowed;

        public bool IsReachedAbilityLevelToCurrentTier;



        public AdditionalInfo(CommonDLL.Dto.AdditionalInfo info)
        {
            IsUnlockSecondTierGiftTaken = info.IsUnlockSecondTierGiftTaken;
            IsFirstTradeAffairsDailyTaskCompleteGiftTaken = info.IsFirstTradeAffairsDailyTaskCompleteGiftTaken;
            IsSecondEnergyAbilityWindowShowed = info.IsSecondEnergyAbilityWindowShowed;
            IsThirdEnergyAbilityWindowShowed = info.IsThirdEnergyAbilityWindowShowed;
            IsFourEnergyAbilityWindowShowed = info.IsFourEnergyAbilityWindowShowed;
            IsReachedAbilityLevelToCurrentTier = info.IsReachedAbilityLevelToCurrentTier;
        }
    }
}