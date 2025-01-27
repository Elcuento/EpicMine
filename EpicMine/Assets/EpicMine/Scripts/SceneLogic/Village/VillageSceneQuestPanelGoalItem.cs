using System;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QuestTaskGoal = BlackTemple.EpicMine.Core.QuestTaskGoal;

public class VillageSceneQuestPanelGoalItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _quantity;
    [SerializeField] private GameObject _completeCheck;

    [Space]
    [SerializeField] private Color _collectedColor;
    [SerializeField] private Color _inProgressColor;

    private QuestTaskGoal _goal;


    private void OnDestroy()
    {
        _goal?.UnSubscribeUpdate(OnChange);
    }

    private void OnChange()
    {
        Initialize(_goal);
    }

    public void Initialize(QuestTaskGoal goal)
    {
        Clear();
        _goal = goal;

        _goal.UnSubscribeUpdate(OnChange);
        _goal.SubscribeUpdate(OnChange);


        _completeCheck.SetActive(_goal.IsCompleted);

        switch (_goal.StaticGoal.Type)
        {
            case QuestTaskType.Collect:
            case QuestTaskType.CollectCurrency:
                var goalColor = _goal.Progress >= _goal.StaticGoal.Goal.Value
                    ? _collectedColor.CovertToHex()
                    : _inProgressColor.CovertToHex();

                _icon.gameObject.SetActive(true);
                _title.text = ""; //" $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key)}:";
                _icon.sprite = _goal.StaticGoal.Type == QuestTaskType.CollectCurrency ? App.Instance.ReferencesTables.Sprites.GoldIcon : SpriteHelper.GetIcon(_goal.StaticGoal.Goal.Key);
                _quantity.text = $"(<color=#{goalColor}>{_goal.Progress}</color>/{_goal.StaticGoal.Goal.Value})";

                break;
            case QuestTaskType.Kill:
                var goalColor2 = _goal.Progress >= _goal.StaticGoal.Goal.Value
                    ? _collectedColor.CovertToHex()
                    : _inProgressColor.CovertToHex();

                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key)}:";
                _quantity.text = $"(<color=#{goalColor2}>{_goal.Progress}</color>/{_goal.StaticGoal.Goal.Value})";
                break;
        }
    }

    private void Clear()
    {
        _goal?.UnSubscribeUpdate(OnChange);

        _title.text = "";
        _quantity.text = "";
        _icon.gameObject.SetActive(false);
    }
}
