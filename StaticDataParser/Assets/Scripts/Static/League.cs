namespace BlackTemple.EpicMine.Static
{
    public class League
    {
        public string Id { get; }

        public int Rating { get; }

        public float Coefficient { get; }
        
        public float WinCoefficient;

        public float LooseCoefficient;

        public League(string id, int rating, float coefficient, float winCoefficient, float looseCoefficient)
        {
            Id = id.ToLower();
            Rating = rating;
            Coefficient = coefficient;
            LooseCoefficient = looseCoefficient;
            WinCoefficient = winCoefficient;
        }
    }
}
