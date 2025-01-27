using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class Mine
    {
        public Tier Tier { get; }

        public int Number { get; }

        public bool IsOpen
        {
            get
            {
                if (Tier.IsOpen == false)
                    return false;

                return Number == 0 || Tier.Mines[Number - 1].IsComplete;
            }
        }

        public bool IsComplete { get; private set; }

        public int Rating { get; private set; }

        public int HardcoreRating { get; private set; }

        public int Highscore { get; private set; }

        public bool IsHardcoreOn { get; private set; }

        public bool IsLast => Number == LocalConfigs.TierMinesCount - 1;

        public bool IsGhostAppear;

        public Mine(Tier tier, int number)
        {
            Tier = tier;
            Number = number;
        }

        public Mine(Tier tier, int number, Mine mineData)
        {
            Tier = tier;
            Number = number;
            IsComplete = mineData.IsComplete;
            Rating = mineData.Rating;
            HardcoreRating = mineData.HardcoreRating;
            Highscore = mineData.Highscore;
            IsHardcoreOn = mineData.IsHardcoreOn;
            IsGhostAppear = mineData.IsGhostAppear;
        }

        public Mine(Tier tier, int number, CommonDLL.Dto.Mine mineData)
        {
            Tier = tier;
            Number = number;
            IsComplete = mineData.IsComplete;
            Rating = mineData.Rating;
            HardcoreRating = mineData.HardcoreRating;
            Highscore = mineData.HighScore;
            IsHardcoreOn = mineData.IsHardcoreOn;
            IsGhostAppear = mineData.IsGhostAppear;
        }

        public void SetGhost()
        {
            IsGhostAppear = true;
            EventManager.Instance.Publish(new MineChangeEvent(this));
        }

        public void Complete()
        {
            IsComplete = true;
            EventManager.Instance.Publish(new MineCompleteEvent(this));
            EventManager.Instance.Publish(new MineChangeEvent(this));
            App.Instance.Services.AnalyticsService.UserLevelUp(StaticHelper.GetProgressLevel(Tier.Number, Number, App.Instance.Player.Prestige));
        }

        public void ToggleHardcore(bool isOn)
        {
            IsHardcoreOn = isOn;
        }

        public void SetRating(int newValue, bool isHardcore = false)
        {
            if (!IsComplete)
                return;

            if (isHardcore && HardcoreRating < newValue)
            {
                HardcoreRating = newValue;
                EventManager.Instance.Publish(new MineChangeEvent(this));
            }
            else if (Rating < newValue)
            {
                Rating = newValue;
                EventManager.Instance.Publish(new MineChangeEvent(this));
            }
        }

        public void SetHighscore(int value)
        {
            if (value <= Highscore)
                return;

            Highscore = value;
            EventManager.Instance.Publish(new MineChangeEvent(this));
        }
    }
}