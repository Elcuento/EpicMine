using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoMinerSceneCollectInfoPanel : MonoBehaviour
{
    [SerializeField] private AutoMinerSceneDropItem _dropPrefab;
    [SerializeField] private Transform _dropContainer;

    [Space]
    [SerializeField] private Color _capacityBarColorFull;
    [SerializeField] private Color _capacityBarColorDefault;
    [SerializeField] private Image _capacityBar;
    [SerializeField] private TextMeshProUGUI _capacityCount;
    [SerializeField] private TextMeshProUGUI _capacityDescription;

    [Space]
    [SerializeField] private GameObject _collectButtonEnable;
    [SerializeField] private GameObject _collectButtonDisable;

    private Tier _tier;

    private void Start()
    {
        EventManager.Instance.Subscribe<AutoMinerChangeCapacityLevelEvent>(OnAutoMinerCapacityChangeLevel);
        EventManager.Instance.Subscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerChangeCapacityLevelEvent>(OnAutoMinerCapacityChangeLevel);
        EventManager.Instance.Unsubscribe<AutoMinerChangeEvent>(OnAutoMinerChange);
    }


    public void Initialize(Tier tier)
    {
        _tier = tier;

        UpdateState();
    }

    private void OnAutoMinerCapacityChangeLevel(AutoMinerChangeCapacityLevelEvent eventData)
    {
        UpdateState();
    }

    private void OnAutoMinerChange(AutoMinerChangeEvent eventData)
    {
        UpdateState();
    }

    private void UpdateState()
    {
        var autoMiner = App.Instance.Player.AutoMiner;

        _dropContainer.ClearChildObjects();

        var nextLevel = autoMiner.CapacityLevel.NextStaticLevel == null
            ? ""
            : $" <color=#00E100>+{ (autoMiner.CapacityLevel.NextStaticLevel.Capacity - autoMiner.CapacityLevel.StaticLevel.Capacity) }</color>";

        _capacityCount.text =
            $"{ autoMiner.Capacity }/{ autoMiner.CapacityLevel.StaticLevel.Capacity }{ nextLevel }";

        _capacityBar.fillAmount = autoMiner.Capacity / (float)
                                         autoMiner.CapacityLevel.StaticLevel.Capacity;

        _capacityBar.color = _capacityBar.fillAmount >= 1 ? _capacityBarColorFull : _capacityBarColorDefault;

        foreach (var autoMinerEarnedResource in autoMiner.EarnedResources)
        {
            if(autoMinerEarnedResource.Value <= 0)
                continue;

            var item = Instantiate(_dropPrefab, _dropContainer, false);
            item.Initialize(autoMinerEarnedResource.Key, autoMinerEarnedResource.Value);
            item.name = autoMinerEarnedResource.Key;

        }

        _collectButtonDisable.SetActive(autoMiner.Capacity <= 0);
        _collectButtonEnable.SetActive(autoMiner.Capacity > 0);


        if (autoMiner.IsFull)
        {
            _capacityDescription.text = LocalizationHelper.GetLocale("autoMiner_scene_collect_info_storage_description_storage_full");
            return;
        }

        if (!autoMiner.Started)
        {
            _capacityDescription.text = LocalizationHelper.GetLocale("autoMiner_scene_collect_info_storage_description_unStarted");
        }
        else
        {
            _capacityDescription.text =
                LocalizationHelper.GetLocale("autoMiner_scene_collect_info_storage_description_select");
        }
    }

    public void ClickCollect()
    {
        if (App.Instance.Player.AutoMiner.EarnedResources.Count == 0)
            return;

        WindowManager.Instance.Show<WindowGetAutoMinerEarning>(withPause:true)
            .Initialize(App.Instance.Player.AutoMiner.EarnedResources);
    }

    public void OnClickMines()
    {
        SceneManager.Instance.LoadScene(ScenesNames.Tiers);
    }
}
