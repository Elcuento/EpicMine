using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using Quest = BlackTemple.EpicMine.Core.Quest;

public class VillageSceneQuestPanelItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Transform _goalContainer;
    [SerializeField] private VillageSceneQuestPanelGoalItem _goalPrefab;


    public void Initialize(Quest quest)
    {
        _title.text = LocalizationHelper.GetLocale(quest.StaticQuest.Id);

        var exist = false;

        foreach (var questTask in quest.TaskList)
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

        gameObject.SetActive(exist);
    }
}
