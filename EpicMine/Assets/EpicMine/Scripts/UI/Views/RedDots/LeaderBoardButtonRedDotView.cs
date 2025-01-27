namespace BlackTemple.EpicMine
{
    public class LeaderBoardButtonRedDotView : RedDotBaseView
    {
        private void Awake()
        {
            App.Instance.Controllers.RedDotsController.OnLeaderBoardChange += OnLeaderBoardChange;
        }

        private void OnLeaderBoardChange(bool isNewLeaderBoard)
        {
            Show(isNewLeaderBoard ? 1 : 0);
        }

        private void Start()
        {
            Show(App.Instance.Controllers.RedDotsController.IsNewLeaderBoard ? 1 : 0);
        }

        private void OnDestroy()
        {
            if (App.Instance != null)
                App.Instance.Controllers.RedDotsController.OnLeaderBoardChange -= OnLeaderBoardChange;
        }
    }
}