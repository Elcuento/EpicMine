using System;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quest = BlackTemple.EpicMine.Core.Quest;

public class WindowDailyTaskQuestQuestToggleItem : MonoBehaviour
{
    [SerializeField] private Color _activeToggleColor;
    [SerializeField] private Color _unActiveToggleColor;

    [SerializeField] private Color _activeReadyToggleColor;
    [SerializeField] private Color _unActiveReadyToggleColor;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Toggle _toggle;

    [SerializeField] private GameObject _typeIcon;
    [SerializeField] private GameObject _typeIconHalloween;

    [SerializeField] private GameObject _trackingFlag;
    [SerializeField] private GameObject _completeFlag;

    private Quest _quest;
    private Action<Quest> _onSelect;
    private Action<Quest> _onForceSelect;

    public void Initialize(Quest quest, ToggleGroup group, Action<Quest> onSelect, Action<Quest> onForceSelect, bool selected)
    {
        _quest = quest;
        _typeIcon.SetActive(quest.StaticQuest.Type == QuestType.Rare || quest.StaticQuest.Type == QuestType.Epic);
        _typeIconHalloween.SetActive(quest.StaticQuest.Type == QuestType.Event);

        _title.text = LocalizationHelper.GetLocale(quest.StaticQuest.Id);
        _toggle.group = group;
        _onSelect = onSelect;
        _title.color = _unActiveToggleColor;
        _toggle.isOn = selected;

        _completeFlag.SetActive(_quest.IsReady);
        _trackingFlag.SetActive(!_completeFlag.activeSelf && _quest.IsTracking);
        
        _onForceSelect += onForceSelect;

        EventManager.Instance.Subscribe<QuestUpdateTrackingEvent>(OnQuestStartTracking);

        Draw(selected);
    }

    private void OnQuestStartTracking(QuestUpdateTrackingEvent eventData)
    {
        if(eventData.Quest == _quest && !_quest.IsReady)
            _trackingFlag.SetActive(eventData.Quest.IsTracking);
    }

    private void OnDestroy()
    {
        if (_onForceSelect != null)
            _onForceSelect -= ForceSelect;

        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<QuestUpdateTrackingEvent>(OnQuestStartTracking);
    }

    private void ForceSelect(Quest quest)
    {
        if (quest == _quest)
        {
            _toggle.isOn = true;
        }
    }

    private void Draw(bool state)
    {
        if (state)
        _title.color = _quest.IsReady ? _activeReadyToggleColor : _activeToggleColor;
        else _title.color = _quest.IsReady ? _unActiveReadyToggleColor : _unActiveToggleColor;
    }

    public void OnToggle(bool state)
    {
        Draw(state);

        if (state)
            _onSelect?.Invoke(_quest);
    }

}
