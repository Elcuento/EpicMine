namespace CommonDLL.Dto
{
    public class Mine
    {
        public int Number;

        public bool IsComplete;

        public int Rating;

        public int HardcoreRating;

        public int HighScore;

        public bool IsHardcoreOn;

        public bool IsGhostAppear;

        public Mine(int number, bool isComplete, int rating, int hardCoreRating, int highScore, bool isHardCoreOn,
            bool isGhostAppear)
        {
            Number = number;
            IsComplete = isComplete;
            Rating = rating;
            HardcoreRating = hardCoreRating;
            HighScore = highScore;
            IsHardcoreOn = isHardCoreOn;
            IsGhostAppear = isGhostAppear;
        }

    }
}