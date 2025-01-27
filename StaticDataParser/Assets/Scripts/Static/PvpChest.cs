using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class PvpChest
    {
        public int TierNumber { get; }

        public int GoldAmountMin { get; }

        public int GoldAmountMax { get; }

        public string FirstResourceFirstVariant { get; }

        public string FirstResourceSecondVariant { get; }

        public int FirstResourceAmountMin { get; }

        public int FirstResourceAmountMax { get; }

        public string SecondResourceFirstVariant { get; }

        public string SecondResourceSecondVariant { get; }

        public int SecondResourceAmountMin { get; }

        public int SecondResourceAmountMax { get; }

        public string ThirdResourceFirstVariant { get; }

        public string ThirdResourceSecondVariant { get; }

        public int ThirdResourceAmountMin { get; }

        public int ThirdResourceAmountMax { get; }

        public int ShardAmount { get; }

        public PvpChest(int tierNumber, int goldAmountMin, int goldAmountMax, string firstResourceFirstVariant,
            string firstResourceSecondVariant, int firstResourceAmountMin, int firstResourceAmountMax,
            string secondResourceFirstVariant, string secondResourceSecondVariant, int secondResourceAmountMin,
            int secondResourceAmountMax, string thirdResourceFirstVariant, string thirdResourceSecondVariant,
            int thirdResourceAmountMin, int thirdResourceAmountMax, int shardAmount)
        {
            TierNumber = tierNumber;
            GoldAmountMin = goldAmountMin;
            GoldAmountMax = goldAmountMax;
            FirstResourceFirstVariant = firstResourceFirstVariant.ToLower();
            FirstResourceSecondVariant = firstResourceSecondVariant.ToLower();
            FirstResourceAmountMin = firstResourceAmountMin;
            FirstResourceAmountMax = firstResourceAmountMax;
            SecondResourceFirstVariant = secondResourceFirstVariant.ToLower();
            SecondResourceSecondVariant = secondResourceSecondVariant.ToLower();
            SecondResourceAmountMin = secondResourceAmountMin;
            SecondResourceAmountMax = secondResourceAmountMax;
            ThirdResourceFirstVariant = thirdResourceFirstVariant.ToLower();
            ThirdResourceSecondVariant = thirdResourceSecondVariant.ToLower();
            ThirdResourceAmountMin = thirdResourceAmountMin;
            ThirdResourceAmountMax = thirdResourceAmountMax;

            ShardAmount = shardAmount;
        }
    }
}