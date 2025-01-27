using System;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quest = BlackTemple.EpicMine.Core.Quest;

public class MineSceneUiQuestPanelItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Transform _goalContainer;
    [SerializeField] private MineSceneUiQuestPanelItemGoal _goalPrefab;

    private Quest _quest;
    
    public void Initialize(Quest quest)
    {
        _quest = quest;      

        _title.text = LocalizationHelper.GetLocale(_quest.StaticQuest.Id);

         UpdateItem();

         _quest.SubscribeUpdate(UpdateItem);
    }

    private void OnDestroy()
    {
        _quest?.UnSubscribeUpdate(UpdateItem);
    }

    private void UpdateItem()
    {
        _goalContainer.ClearChildObjects();

        var exist = false;

        foreach (var questTask in _quest.TaskList)
        {
            if (questTask.IsCompleted)
                continue;

            foreach (var questTaskGoal in questTask.GoalsList)
            {
                if (questTaskGoal.IsCompleted || (questTaskGoal.StaticGoal.Type != QuestTaskType.Collect &&
                                                  questTaskGoal.StaticGoal.Type != QuestTaskType.Kill))
                    continue;

                Instantiate(_goalPrefab, _goalContainer)
                    .Initialize(questTaskGoal);
                exist = true;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());

        if (!exist)
        Destroy(gameObject);
    }
}
