using BlackTemple.Common;
using BlackTemple.EpicMine;

public class AutoMinerButtonRedDotView : RedDotBaseView
{
    private void Awake()
    {
        EventManager.Instance.Subscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
    }

    private void OnAutoMinerChange(AutoMinerChangeEvent eventData)
    {
        Calculate();
    }

    private void Calculate()
    {
        Show(App.Instance.Player.AutoMiner.IsFull ? 1 : 0);
    }

    private void Start()
    {
        Calculate();
    }

    private void OnDestroy()
    {
        if (App.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
    }

}
