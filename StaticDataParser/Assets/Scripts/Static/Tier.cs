namespace BlackTemple.EpicMine.Static
{
    public class Tier
    {
        public string WallItem1Id { get; }

        public string WallItem2Id { get; }

        public string WallItem3Id { get; }

        public int WallMoneyMin { get; }

        public int WallMoneyMax { get; }

        public float FirstIncreaseChestProbabilityTime { get; }

        public float FirstIncreaseChestProbabilityPercent { get; }

        public float SecondIncreaseChestProbabilityTime { get; }

        public float SecondIncreaseChestProbabilityPercent { get; }

        public int RequireArtefacts { get; }

        public Tier(string wallItem1Id, string wallItem2Id, string wallItem3Id,
            int wallMoneyMin, int wallMoneyMax, float firstIncreaseChestProbabilityTime,
            float firstIncreaseChestProbabilityPercent, float secondIncreaseChestProbabilityTime,
            float secondIncreaseChestProbabilityPercent, int requireArtefacts)
        {
            WallItem1Id = wallItem1Id.ToLower();
            WallItem2Id = wallItem2Id.ToLower();
            WallItem3Id = wallItem3Id.ToLower();
            WallMoneyMin = wallMoneyMin;
            WallMoneyMax = wallMoneyMax;
            FirstIncreaseChestProbabilityTime = firstIncreaseChestProbabilityTime;
            FirstIncreaseChestProbabilityPercent = firstIncreaseChestProbabilityPercent;
            SecondIncreaseChestProbabilityTime = secondIncreaseChestProbabilityTime;
            SecondIncreaseChestProbabilityPercent = secondIncreaseChestProbabilityPercent;
            RequireArtefacts = requireArtefacts;
        }
    }
}