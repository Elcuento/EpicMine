using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

public class WindowDailyTasksSecondPartTutorialStepAssistant: WindowBase
{
    [SerializeField] private GameObject _container1;
    [SerializeField] private GameObject _container2;

    private void Start()
    {
        Clear();

        if (!Check())
            return;

        EventManager.Instance.Subscribe<DailyTaskTakeEvent>(OnDailyTaskCompleted);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<DailyTaskTakeEvent>(OnDailyTaskCompleted);
    }

    private void Clear()
    {
        _container1.SetActive(false);
        _container2.SetActive(false);
    }

    private bool Check()
    {
        if (App.Instance.Controllers.DailyTasksController.Tasks.Find(x => x.IsCompleted) == null)
        {
            _container2.SetActive(true);
        }
        else
        {
            _container1.SetActive(true);
        }

        if (App.Instance.Player.Quests.QuestList.Find(x => x.Status == QuestStatusType.Completed) != null)
        {
            Close();
            return false;
        }

        return true;
    }


    private void OnDailyTaskCompleted(DailyTaskTakeEvent eventData)
    {
        if (App.Instance.Player.Quests.QuestList.Find(x => x.Status == QuestStatusType.Completed) != null)
        {
            Close();
            return;
        }

        _container2.SetActive(true);
        _container1.SetActive(false);
    }

    public void OnClickQuestTab()
    {
        WindowManager.Instance.Get<WindowDailyTasksQuest>()
            .OpenQuests();

        Close();
    }

}
