namespace BlackTemple.EpicMine
{
    public class RatingHelper
    {
        public static int GetRating(int currentHealth)
        {
            if (currentHealth >= 9)
                return 3;

            return currentHealth >= 6 ? 2 : 1;
        }

        public static int GetSectionsCountForHardcoreRating(int rating)
        {
            var sectionsCount = 0;
            
            switch (rating)
            {
                case 1:
                    sectionsCount = App.Instance.StaticData.Configs.Dungeon.Mines.Rating.Hardcore[0];
                    break;
                case 2:
                    sectionsCount = App.Instance.StaticData.Configs.Dungeon.Mines.Rating.Hardcore[1];
                    break;
                case 3:
                    sectionsCount = App.Instance.StaticData.Configs.Dungeon.Mines.Rating.Hardcore[2];
                    break;
            }

            return sectionsCount;
        }
        
        public static int GetMineHardcoreRating(int currentSectionNumber)
        {
            var sectionsCountForRatingOne = GetSectionsCountForHardcoreRating(1); // 10
            var sectionsCountForRatingTwo = GetSectionsCountForHardcoreRating(2); // 8
            var sectionsCountForRatingThree = GetSectionsCountForHardcoreRating(3); // 5

            currentSectionNumber += 1;                                                 //5 =  6
            
            if (currentSectionNumber > sectionsCountForRatingOne + sectionsCountForRatingTwo + sectionsCountForRatingThree)
                return 3;
            if (currentSectionNumber > sectionsCountForRatingOne + sectionsCountForRatingTwo)
                return 2;
            if (currentSectionNumber > sectionsCountForRatingOne)
                return 1;

            return 0;
        }
    }
}