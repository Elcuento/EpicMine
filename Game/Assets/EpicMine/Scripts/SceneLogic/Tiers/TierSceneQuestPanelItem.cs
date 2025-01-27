using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;

public class TierSceneQuestPanelItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;

    private Quest _quest;

    public void Initialize(Quest quest)
    {
        _quest = quest;
        _title.text = LocalizationHelper.GetLocale(quest.StaticQuest.Id);
    }

    public void Initialize(QuestTask task)
    {
        _quest = task.Quest;
        _title.text = LocalizationHelper.GetLocale(_quest.StaticQuest.Id);
    }

    public void OnClick()
    {
        WindowManager.Instance.Show<WindowDailyTasksQuest>()
            .ForceSelectQuest(_quest);
    }
}
