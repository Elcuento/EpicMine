using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowDailyTaskQuestQuestTaskClosedItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private GameObject _lock;
    [SerializeField] private GameObject _completeCheck;
    [SerializeField] private Image _background;
    [SerializeField] private Color _lockColor;

    private QuestTask _subTask;

    public void Initialize(QuestTask task)
    {
        Clear();

        _subTask = task;

        if (task.IsCompleted)
        {
            _title.text = LocalizationHelper.GetLocale(_subTask.StaticTask.Id);
            _lock.SetActive(false);
            _completeCheck.SetActive(true);

        }
        else
        {
            _lock.SetActive(true);
            _completeCheck.SetActive(false);
            _background.color = _lockColor;
        }
    }
    
    private void Clear()
    {
        _lock.SetActive(false);
        _completeCheck.SetActive(false);
        _title.text = "";
        _background.color = Color.white;
    }
}
