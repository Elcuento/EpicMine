using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

using UnityEngine.UI;

public class VillageSceneQuestPanel : MonoBehaviour
{
    [SerializeField] private Transform _questContainer;
    [SerializeField] private VillageSceneQuestPanelItem _questPrefab;

    [Space]
    [SerializeField] private Image _arrowIcon;
    [SerializeField] private GameObject _root;

    [Space]
    [SerializeField] private GameObject _noTrackingQuests;

    private bool _isOpen;

    private void Start()
    {
        _isOpen = true;
        Initialize();
        Subscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void Initialize()
    {
        _questContainer.ClearChildObjects(1);

        var isTrackingExist = false;

        foreach (var quest in App.Instance.Player.Quests.QuestList)
        {
            if (quest.Status != QuestStatusType.Started)
                continue;

            if (!quest.IsTracking)
                continue;

            foreach (var questTask in quest.TaskList)
            {
                var questIsTrackAble = false;

                if (questTask.IsCompleted)
                    continue;


                foreach (var questTaskGoal in questTask.GoalsList)
                {
                    if (questTaskGoal.IsCompleted || (questTaskGoal.StaticGoal.Type != QuestTaskType.Collect &&
                                                      questTaskGoal.StaticGoal.Type != QuestTaskType.Kill))
                        continue;

                    questIsTrackAble = true;
                    break;
                }

                if (questIsTrackAble)
                {
                    Instantiate(_questPrefab, _questContainer)
                        .Initialize(quest);

                    isTrackingExist = true;
                    break;
                }
            }
        }

        _root.SetActive(isTrackingExist);
        _noTrackingQuests.SetActive(!isTrackingExist);
    }

    private void Subscribe()
    {
        EventManager.Instance.Subscribe<QuestUpdateEvent>(OnQuestUpdated);
        EventManager.Instance.Subscribe<QuestUpdateTrackingEvent>(OnQuestTrackerUpdated);
    }

    private void UnSubscribe()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<QuestUpdateEvent>(OnQuestUpdated);
        EventManager.Instance.Unsubscribe<QuestUpdateTrackingEvent>(OnQuestTrackerUpdated);
    }

    private void OnQuestUpdated(QuestUpdateEvent eventData)
    {
        Initialize();
    }

    private void OnQuestTrackerUpdated(QuestUpdateTrackingEvent eventData)
    {
        Initialize();
    }


    public void Close()
    {
        _isOpen = !_isOpen;
        _questContainer.gameObject.SetActive(_isOpen);
        _arrowIcon.transform.localEulerAngles = new Vector3(0, _isOpen ? 180 : 0, 90);

    }
}
