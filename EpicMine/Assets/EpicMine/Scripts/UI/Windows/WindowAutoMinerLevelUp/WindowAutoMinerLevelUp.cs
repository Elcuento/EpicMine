using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;

public class WindowAutoMinerLevelUp : WindowBase
{
    [SerializeField] private Transform _container;

    public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
    {
        base.OnShow(withPause, withCurrencies, withRating);

        Clear();
        AutoMinerHelper.GetModel(_container);
    }

    private void Clear()
    {
        _container.ClearChildObjects();
    }
}
