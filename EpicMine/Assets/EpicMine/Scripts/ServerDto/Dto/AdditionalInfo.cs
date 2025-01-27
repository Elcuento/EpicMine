namespace CommonDLL.Dto
{
    public class AdditionalInfo
    {
        public bool IsUnlockSecondTierGiftTaken;

        public bool IsFirstTradeAffairsDailyTaskCompleteGiftTaken;

        public bool IsSecondEnergyAbilityWindowShowed;

        public bool IsThirdEnergyAbilityWindowShowed;

        public bool IsFourEnergyAbilityWindowShowed;

        public bool IsReachedAbilityLevelToCurrentTier;

        public AdditionalInfo()
        {

        }
        public AdditionalInfo(BlackTemple.EpicMine.Core.AdditionalInfo data)
        {
            IsUnlockSecondTierGiftTaken = data.IsUnlockSecondTierGiftTaken;
            IsFirstTradeAffairsDailyTaskCompleteGiftTaken = data.IsFirstTradeAffairsDailyTaskCompleteGiftTaken;
            IsSecondEnergyAbilityWindowShowed = data.IsSecondEnergyAbilityWindowShowed;
            IsThirdEnergyAbilityWindowShowed = data.IsThirdEnergyAbilityWindowShowed;
            IsFourEnergyAbilityWindowShowed = data.IsFourEnergyAbilityWindowShowed;
            IsReachedAbilityLevelToCurrentTier = data.IsReachedAbilityLevelToCurrentTier;
        }

    }
}