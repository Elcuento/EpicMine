using System;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

using UnityEngine.UI;

public class MineSceneUiQuestPanel : MonoBehaviour
{
    public Action OnQuestsUpdate;

    [SerializeField] private Transform _questContainer;
    [SerializeField] private MineSceneUiQuestPanelItem _questPrefab;

    [Space]
    [SerializeField] private Image _arrowIcon;
    [SerializeField] private GameObject _root;

    [Space]
    [SerializeField] private GameObject _noAvailableTrackingQuests;

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

    private void Subscribe()
    {
        EventManager.Instance.Subscribe<QuestUpdateEvent>(OnQuestUpdated);
        EventManager.Instance.Subscribe<QuestUpdateTrackingEvent>(OnQuestUpdateTrackingEvent);
    }

    private void UnSubscribe()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<QuestUpdateEvent>(OnQuestUpdated);
        EventManager.Instance.Unsubscribe<QuestUpdateTrackingEvent>(OnQuestUpdateTrackingEvent);
    }

    private void OnQuestUpdated(QuestUpdateEvent data)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_questContainer.GetComponent<RectTransform>());
    }

    private void OnQuestUpdateTrackingEvent(QuestUpdateTrackingEvent data)
    {
        Initialize();
    }

    private void Initialize()
    {

        _questContainer.ClearChildObjects(new List<GameObject>{ _noAvailableTrackingQuests });

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

                  }

                  if (questIsTrackAble)
                  {
                      isTrackingExist = true;

                      Instantiate(_questPrefab, _questContainer)
                          .Initialize(quest);

                      break;
                  }
              }
          }

          _root.SetActive(isTrackingExist);
          _noAvailableTrackingQuests.SetActive(!isTrackingExist);
      }

      public void Close()
    {
        _isOpen = !_isOpen;
        _questContainer.gameObject.SetActive(_isOpen);
        _arrowIcon.transform.localEulerAngles = new Vector3(0, _isOpen ? 180 : 0 , 90);
    }
}
