using BlackTemple.Common;
using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowDailyTaskQuestQuestTaskOpenItem : MonoBehaviour
{
    [SerializeField] private WindowDailyTaskQuestQuestTaskGoalItem _taskPrefab;
    [SerializeField] private Transform _subTaskContainer;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _title;

    private BlackTemple.EpicMine.Core.QuestTask _task;

    private void Start()
    {
        EventManager.Instance.Subscribe<QuestTaskCompleteEvent>(OnTaskComplete);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<QuestTaskCompleteEvent>(OnTaskComplete);
    }

    private void OnTaskComplete(QuestTaskCompleteEvent eventData)
    {
        Initialize(_task);
    }

    public void Initialize(BlackTemple.EpicMine.Core.QuestTask task)
    {
        Clear();

        _task = task;

        _title.text = LocalizationHelper.GetLocale(_task.StaticTask.Id);

        if (_task.IsCompleted)
        {
            _buyButton.image.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;
        }
        else
        {
            foreach (var questSubTask in task.GoalsList)
            {
                var subTask = Instantiate(_taskPrefab, _subTaskContainer, false);
                subTask.Initialize(questSubTask);
            }

            _buyButton.image.sprite = _task.IsReady ?
                App.Instance.ReferencesTables.Sprites.ButtonGreen : App.Instance.ReferencesTables.Sprites.ButtonGrey;
        }
    }

    public void OnClickComplete()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

        _task.Complete();
    }

    private void Clear()
    {
        _subTaskContainer.ClearChildObjects();
    }
}
