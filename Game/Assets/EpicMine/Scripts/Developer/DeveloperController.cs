using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperController : MonoBehaviour {

	[SerializeField] private ScrollRect _leftPanelContainer;
    [SerializeField] private Transform _unDestroyAbleItems;

    [SerializeField] private Toggle _enableConsoleToggle;
    [SerializeField] private Toggle _enableStatsToggle;

    [SerializeField] private TextMeshProUGUI _statsText;

    [SerializeField] private DeveloperStats _stats;

    [SerializeField] private DeveloperControlPanelVerticalScrollItem _verticalScrollPrefab;
    [SerializeField] private DeveloperControlPanelButtonItem _buttonPrefab;
    [SerializeField] private DeveloperControlPanelInputField _inputPrefab;
    [SerializeField] private DeveloperControlPanelDropDownItem _dropDownPrefab;
    [SerializeField] private DeveloperControlPanelToggleItem _togglePrefab;
    [SerializeField] private DeveloperControlPanelContainerItem _containerPrefab;

    private GameObject _developerPanel;

    private void Start ()
    {
        _leftPanelContainer.gameObject.SetActive(false);
        _enableConsoleToggle.isOn = false;
        _enableStatsToggle.isOn = false;

        SceneManager.Instance.OnSceneChange += OnSceneChange;

        OnSceneChange("", SceneManager.Instance.CurrentScene);
    }


    private void OnDestroy()
    {
        if(SceneManager.Instance!=null)
        SceneManager.Instance.OnSceneChange -= OnSceneChange;
    }

    
    public void OnToggleConsole()
    {
        _leftPanelContainer.gameObject.SetActive(_enableConsoleToggle.isOn);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_leftPanelContainer.content);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_leftPanelContainer.GetComponent<RectTransform>());

        if (!_enableConsoleToggle.isOn)
        {
            for (var i = 1; i < _leftPanelContainer.content.childCount; i++)
            {
                var child = _leftPanelContainer.content.transform.GetChild(i).GetComponent<DeveloperControlPanelVerticalScrollItem>();
                if (child.Content.childCount > 0)
                {
                    for (var j = 0; j < child.Content.childCount; j++)
                    {
                        var item = child.Content.GetChild(j).GetComponent<DeveloperControlPanelItem>();
                        if (item.DontDestroyObject)
                        {
                            item.gameObject.transform.SetParent(_unDestroyAbleItems);
                            j--;
                        }
                    }

                }

                DestroyImmediate(_leftPanelContainer.content.GetChild(i).gameObject);
            }
        }
    }

    public void Show(bool hide)
    {
        _verticalScrollPrefab.gameObject.SetActive(hide);
    }

    public void OnToggleStats()
    {
       // return;
       _stats.enabled = _enableStatsToggle.isOn;
        _stats.AttackTo(_statsText);
        _statsText.gameObject.SetActive(_enableStatsToggle.isOn);
    }


    private void OnSceneChange(string from, string to)
    {
        if (from == to)
            return;

        _leftPanelContainer.content.ClearChildObjects();

        if (_developerPanel != null)
        {
            Destroy(_developerPanel);
        }

        switch (to)
        {
            case ScenesNames.EntryPoint:
                var eDev = Instantiate(Resources.Load<EntrySceneDeveloperPanel>("Prefabs/Developer/EntrySceneDeveloperPanel"), transform);
                eDev.Initialize(this);
                _developerPanel = eDev.gameObject;
                break;
            case ScenesNames.Mine:
            case ScenesNames.PvpArena:
                var mDev = Instantiate(Resources.Load<MineSceneDeveloperPanel>("Prefabs/Developer/MineSceneDeveloperPanel"),transform);
                mDev.Initialize(this);
                _developerPanel = mDev.gameObject;
                break;
            case ScenesNames.Village:
            case ScenesNames.Tiers:
                var vDev = Instantiate(Resources.Load<VillageSceneDeveloperPanel>("Prefabs/Developer/VillageSceneDeveloperPanel"), transform);
                vDev.Initialize(this);
                _developerPanel = vDev.gameObject;
                break;
        }
    }

    private void LoadSceneAndGetMonster()
    {
        App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, App.Instance.Player.Dungeon.LastOpenedTier);
        App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, App.Instance.Player.Dungeon.LastOpenedTier.LastOpenedMine);
        SceneManager.Instance.LoadScene(ScenesNames.Mine);

    }


    public void Rebuild()
    {
        foreach (Transform trans in _leftPanelContainer.content)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(trans.gameObject.GetComponent<RectTransform>());
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_leftPanelContainer.content.GetComponent<RectTransform>());
    }

    public DeveloperControlPanelVerticalScrollItem CreateVerticalScroll(DeveloperControlPanelButtonItem source, List<DeveloperControlPanelItem> content)
    {
        if (source != null)
        {
            var number = source.VerticalScroll.transform.GetSiblingIndex();
            for (var i = number + 1; i < _leftPanelContainer.content.childCount; i++)
            {
                var child = _leftPanelContainer.content.transform.GetChild(i).GetComponent<DeveloperControlPanelVerticalScrollItem>();
                if (child.Content.childCount > 0)
                {
                    for (var j = 0; j < child.Content.childCount; j++)
                    {
                        var item = child.Content.GetChild(j).GetComponent<DeveloperControlPanelItem>();
                        if (item.DontDestroyObject)
                        {
                            item.gameObject.transform.SetParent(_unDestroyAbleItems);
                            j--;
                        }
                    }
      
                }

                DestroyImmediate(child.gameObject);
                
                i--;

            }
        }


        var scroll = Instantiate(_verticalScrollPrefab, _leftPanelContainer.content, false);
        var controlItem = AddControlItem(source == null ? "Default" : source.GetText());
        content.Insert(0, controlItem);

        foreach (var o in content)
        {
            o.SetVerticalScroll(scroll);
            o.transform.SetParent(scroll.Content,false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.GetComponent<ScrollRect>().content);
        return scroll;
    }

    public DeveloperControlPanelContainerItem AddControlItem(Transform container, string label, GameObject obj, Transform trans)
    {
        var controlItemContainer = Instantiate(_containerPrefab, container, false);
        controlItemContainer.Initialize(label, obj);

        return controlItemContainer;
    }

    public DeveloperControlPanelContainerItem AddControlItem(string label, GameObject obj, Transform trans)
    {
        var controlItemContainer = Instantiate(_containerPrefab, _leftPanelContainer.content, false);
        controlItemContainer.Initialize(label, obj);
        return controlItemContainer;
    }

    public DeveloperControlPanelContainerItem AddControlItem(string label, Transform trans = null)
    {
        var controlItemContainer = Instantiate(_containerPrefab, trans, false);
        controlItemContainer.Initialize(label);

        return controlItemContainer;
    }

    public DeveloperControlPanelToggleItem AddToggle(Action<bool> action, string text, Transform container = null, bool defaultVal = true)
    {
        var item = Instantiate(_togglePrefab, container == null ? _unDestroyAbleItems : container, false);
        item.Initialize(action, text, defaultVal);

        return item;
    }

    public DeveloperControlPanelDropDownItem AddDropDown(Action<int> action, string text, List<string> options, Transform container = null)
    {
        var item = Instantiate(_dropDownPrefab, container == null ? _unDestroyAbleItems : container, false);
        item.Initialize(action, text, options);

        return item;
    }

    public DeveloperControlPanelInputField AddInputField(Action<string> action, string text, Transform container = null)
    {
        var item  = Instantiate(_inputPrefab, container == null ? _unDestroyAbleItems : container, false);
        item.Initialize(action, text);

        return item;
    }

    public DeveloperControlPanelButtonItem AddButton(Action action, string text, Transform container = null)
    {
        var item = Instantiate(_buttonPrefab, container, false);
        item.Initialize(action, text);

        return item;
    }

    ///
    ///
    ///

    public DeveloperControlPanelButtonItem AddButton(Action action, string text)
    {
        var item = Instantiate(_buttonPrefab, null, false);
        item.Initialize(action, text);

        return item;
    }

    public DeveloperControlPanelButtonItem AddButton(Action<DeveloperControlPanelButtonItem> action, string text)
    {
        var item = Instantiate(_buttonPrefab);
        item.Initialize(action, text);

        return item;
    }

}
