namespace BlackTemple.EpicMine.Static
{
    public class AttackPointsProbabilitySettings
    {
        public int DonateWallSectionsPassedToDisable { get; }

        public int HelpPickaxeDestroyedAmountToEnable { get; }

        public int HelpSectionNumberLessToEnable { get; }

        public int HelpSectionNumberToDisable { get; }

        public int BrakingMineCompleteAmountToEnable { get; }

        public int BrakingPickaxeDestroyAmountToDisable { get; }

        public AttackPointsProbabilitySettings(int donateWallSectionsPassedToDisable,
            int helpPickaxeDestroyedAmountToEnable, int helpSectionNumberLessToEnable, int helpSectionNumberToDisable,
            int brakingMineCompleteAmountToEnable, int brakingPickaxeDestroyAmountToDisable)
        {
            DonateWallSectionsPassedToDisable = donateWallSectionsPassedToDisable;
            HelpPickaxeDestroyedAmountToEnable = helpPickaxeDestroyedAmountToEnable;
            HelpSectionNumberLessToEnable = helpSectionNumberLessToEnable;
            HelpSectionNumberToDisable = helpSectionNumberToDisable;
            BrakingMineCompleteAmountToEnable = brakingMineCompleteAmountToEnable;
            BrakingPickaxeDestroyAmountToDisable = brakingPickaxeDestroyAmountToDisable;
        }
    }
}