using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;

public class WindowDailyTaskQuestQuestGoalsInfoItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _title;

    public void Initialize(Quest quest)
    {
        var typResult = QuestHelper.GetQuestGoalsInfo(quest);
        
        if(string.IsNullOrEmpty(typResult))
            _title.gameObject.SetActive(false);
        else
        {
            _description.text = "\n" + typResult;
        }
    }
}
