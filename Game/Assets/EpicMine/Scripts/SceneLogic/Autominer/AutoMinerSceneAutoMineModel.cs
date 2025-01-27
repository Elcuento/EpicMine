using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;

public class AutoMinerSceneAutoMineModel : MonoBehaviour
{
    [SerializeField] private Transform _container;

    private void Start()
    {
        EventManager.Instance.Subscribe<AutoMinerChangeMinerLevelEvent>(OnAutoMinerLevelUp);
        SetAutoMine();
    }

    private void SetAutoMine()
    {
        _container.ClearChildObjects();

        AutoMinerHelper.GetModel(_container);
    }

    private void OnAutoMinerLevelUp(AutoMinerChangeMinerLevelEvent eventData)
    {
        SetAutoMine();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerChangeMinerLevelEvent>(OnAutoMinerLevelUp);
    }
}
