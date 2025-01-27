using System.Collections.Generic;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowDailyTaskQuestQuestDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _page;
    [SerializeField] private TextMeshProUGUI _description;

    [SerializeField] private GameObject _root;

    private int _index;
    private Quest _quest;
    private List<string> _descriptionList;

    [SerializeField] private CanvasGroup _leftArrow;
    [SerializeField] private CanvasGroup _rightArrow;
    [SerializeField] private Color _arrowColor;

    [Space]
    [SerializeField] private Checkbox _trackingToggle;

    public void Initialize(Quest quest)
    {
        _root.SetActive(true);
        _quest = quest;
        _index = 0;
        _descriptionList = new List<string>();

        if (_quest.StaticQuest.StartDescription.Count > 0)
        {
            foreach (var desc in _quest.StaticQuest.StartDescription)
            {
                _descriptionList.Add(desc);
            }
        }

        foreach (var task in _quest.TaskList)
        {
            if(!task.IsCompleted)
                continue;

            if(string.IsNullOrEmpty(task.StaticTask.EndDescription))
                continue;

            _descriptionList.Add(task.StaticTask.EndDescription);
        }

        if (quest.StaticQuest.EndDescription.Count > 0 && quest.IsReady)
        {
            foreach (var desc in quest.StaticQuest.EndDescription)
            {
                _descriptionList.Add(desc);
            }
        }

        _index = _descriptionList.Count > 0 ? _descriptionList.Count - 1 : 0;

        _trackingToggle.SetInteractable(QuestHelper.IsQuestAvailableForTracking() || _quest.IsTracking);

        _trackingToggle.SetOn(_quest.IsTracking);

        Show();
    }

    private void Start()
    {
        _trackingToggle.OnChange += OnToggleQuestTracking;
    }

    private void OnDestroy()
    {
        _trackingToggle.OnChange -= OnToggleQuestTracking;
    }
    private void OnToggleQuestTracking(bool state)
    {
        if (_quest == null)
        {
            _trackingToggle.enabled = !state;
            return;
        }

        if (_quest.IsTracking == state)
            return;

        _quest.SetTracking(state);
        _trackingToggle.SetInteractable(QuestHelper.IsQuestAvailableForTracking() || state);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    public void OnClickNext()
    {
        _index = _descriptionList.Count - 1 > _index ? _index + 1 : _index;
        Show();
    }

    public void OnClickPrev()
    {
        _index = _index > 0 ? _index - 1 : _index;
        Show();
    }

    private void Show()
    {
        _page.text = $"{(_index + 1)} / {_descriptionList.Count}";
        _description.text = _descriptionList.Count > _index ? (LocalizationHelper.GetLocale(_descriptionList[_index]))  : "";

        _leftArrow.alpha = _index <= 0 ? 0.3f : 1;
        _rightArrow.alpha = _index >= _descriptionList.Count - 1 ? 0.3f : 1;
    }
}
