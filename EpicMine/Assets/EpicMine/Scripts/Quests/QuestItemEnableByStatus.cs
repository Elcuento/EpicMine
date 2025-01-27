using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

public class QuestItemEnableByStatus : MonoBehaviour
{
    [SerializeField] private string _id;
    [SerializeField] private QuestStatusType _status;

    [SerializeField] private GameObject _object;
    [SerializeField] private Behaviour _monoBeh;

    private void Start()
    {
        var quest = App.Instance.Player.Quests.QuestList.Find(x => x.StaticQuest.Id == _id);
        if (quest != null)
        {
            Check(quest);
        }
        else return;

        EventManager.Instance.Subscribe<QuestCompleteEvent>(OnQuestComplete);
        EventManager.Instance.Subscribe<QuestActivateEvent>(OnQuestQuestActivateEvent);
        EventManager.Instance.Subscribe<QuestStartEvent>(OnQuestStat);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<QuestCompleteEvent>(OnQuestComplete);
        EventManager.Instance.Unsubscribe<QuestActivateEvent>(OnQuestQuestActivateEvent);
        EventManager.Instance.Unsubscribe<QuestStartEvent>(OnQuestStat);
    }

    private void OnQuestStat(QuestStartEvent eventData)
    {
        Check(eventData.Quest);

    }
    private void OnQuestQuestActivateEvent(QuestActivateEvent eventData)
    {
        Check(eventData.Quest);
    }

    private void OnQuestComplete(QuestCompleteEvent eventData)
    {
        Check(eventData.Quest);
    }

    private void Check(BlackTemple.EpicMine.Core.Quest quest)
    {
        if (quest.StaticQuest.Id != _id)
            return;

        var status = quest.Status == _status;

        if (_object != null)
            _object.SetActive(status);

        if (_monoBeh != null)
            _monoBeh.enabled = status;
    }
}
