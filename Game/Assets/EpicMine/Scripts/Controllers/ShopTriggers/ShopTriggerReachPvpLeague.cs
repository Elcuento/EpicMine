using BlackTemple.Common;
using BlackTemple.EpicMine.Assets.EpicMine.Scripts.Controllers.ShopTriggers;

namespace BlackTemple.EpicMine
{
    public class ShopTriggerReachPvpLeague : ShopTrigger
    {
        public int TargetLeague;
        public int CurrentLeague;

        public ShopTriggerReachPvpLeague(int targetLeague, string offerId, bool isCompleted = false) 
            : base(offerId, isCompleted)
        {
            CurrentLeague = PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating);
            TargetLeague = targetLeague;
        }

        public override void OnStart()
        {
            EventManager.Instance.Subscribe<PvpUpdateChangeEvent>(OnPvpLeagueChange);
        }

        public override void OnReset()
        {
            EventManager.Instance.Unsubscribe<PvpUpdateChangeEvent>(OnPvpLeagueChange);
        }

        public override void OnCheck()
        {
            if (CurrentLeague >= TargetLeague)
                SetCompleted();
        }

        public override void OnCompleted()
        {
            EventManager.Instance.Unsubscribe<PvpUpdateChangeEvent>(OnPvpLeagueChange);
        }


        public void OnPvpLeagueChange(PvpUpdateChangeEvent data)
        {
            CurrentLeague = PvpHelper.GetLeagueByRating(App.Instance.Player.Pvp.Rating);

            SetCheck();
        }

    }
}
