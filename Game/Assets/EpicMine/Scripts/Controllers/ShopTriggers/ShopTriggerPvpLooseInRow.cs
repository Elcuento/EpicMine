using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class ShopTriggerPvpLooseInRow : ShopTrigger
    {
        public int RequiredLooseInRow;
        public int Loose;

        public ShopTriggerPvpLooseInRow(int loose, string offerId, bool isCompleted = false) 
            : base(offerId, isCompleted)
        {
            RequiredLooseInRow = loose;
        }

        public override void OnStart()
        {
            EventManager.Instance.Subscribe<PvpArenaEndGameResoultEvent>(OnGameEnd);
        }

        public override void OnReset()
        {
            EventManager.Instance.Unsubscribe<PvpArenaEndGameResoultEvent>(OnGameEnd);
        }

        public override void OnCompleted()
        {
            EventManager.Instance.Unsubscribe<PvpArenaEndGameResoultEvent>(OnGameEnd);
        }

        public override void OnCheck()
        {
            if (Loose >= RequiredLooseInRow)
            {
                SetCompleted();
            }

        }

        public void OnGameEnd(PvpArenaEndGameResoultEvent result)
        {
            if (result.Resoult == PvpArenaGameResoultType.Loose)
                Loose++;
            else Loose = 0;

            SetCheck();
        }
    }
}
