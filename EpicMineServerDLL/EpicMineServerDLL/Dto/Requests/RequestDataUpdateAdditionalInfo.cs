namespace AMTServerDLL.Dto
{
    public class RequestDataUpdateAdditionalInfo : SendData
    {
        public bool IsSecondEnergyAbilityWindowShowed;
        public bool IsThirdEnergyAbilityWindowShowed;
        public bool IsFourEnergyAbilityWindowShowed;

        public RequestDataUpdateAdditionalInfo(bool isSecondEnergyAbilityWindowShowed,
            bool isThirdEnergyAbilityWindowShowed, bool isFourEnergyAbilityWindowShowed)
        {
            IsSecondEnergyAbilityWindowShowed = isSecondEnergyAbilityWindowShowed;
            IsThirdEnergyAbilityWindowShowed = isThirdEnergyAbilityWindowShowed;
            IsFourEnergyAbilityWindowShowed = isFourEnergyAbilityWindowShowed;
        }
    }
}