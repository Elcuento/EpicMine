

using CommonDLL.Static;

namespace BlackTemple.EpicMine.Dto
{
    public struct AttackPointProbability
    {
        public AttackPointTypeProbability Type;

        public int DonateWallSectionsPassedAmount;

        public AttackPointProbability(AttackPointTypeProbability type, int donateWallSectionsPassedAmount)
        {
            Type = type;
            DonateWallSectionsPassedAmount = donateWallSectionsPassedAmount;
        }
    }
}