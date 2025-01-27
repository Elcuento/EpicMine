using System;
using System.Globalization;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using QuestTaskGoal = BlackTemple.EpicMine.Core.QuestTaskGoal;

public class WindowDailyTaskQuestQuestTaskGoalItem : MonoBehaviour
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
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnSecondTick);
    }

    public void Initialize(QuestTaskGoal goal)
    {
        Clear();

        _goal = goal;
        _completeCheck.SetActive(_goal.IsCompleted);

        if (LocalizationHelper.IsLocaleExist(_goal.StaticGoal.Id))
        {
            _title.text = "- " + LocalizationHelper.GetLocale(_goal.StaticGoal.Id);
            return;
        }

        switch (_goal.StaticGoal.Type)
        {
            case QuestTaskType.Collect:
            case QuestTaskType.CollectCurrency:
                var goalColor = _goal.Progress >= _goal.StaticGoal.Goal.Value
                    ? _collectedColor.CovertToHex()
                    : _inProgressColor.CovertToHex();

                _icon.gameObject.SetActive(true);
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key)}:";
                _icon.sprite = _goal.StaticGoal.Type == QuestTaskType.CollectCurrency ? App.Instance.ReferencesTables.Sprites.GoldIcon : SpriteHelper.GetIcon(_goal.StaticGoal.Goal.Key);
                _quantity.text = $"(<color=#{goalColor}>{_goal.Progress}</color>/{_goal.StaticGoal.Goal.Value})";

                break;
            case QuestTaskType.Kill:
            case QuestTaskType.OpenChests:
            case QuestTaskType.OpenGift:
                var goalColor2 = _goal.Progress >= _goal.StaticGoal.Goal.Value
                    ? _collectedColor.CovertToHex()
                    : _inProgressColor.CovertToHex();

                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key)}:";
                _quantity.text = $"(<color=#{goalColor2}>{_goal.Progress}</color>/{_goal.StaticGoal.Goal.Value})";
                break;
            case QuestTaskType.Speak:
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" :"-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key.ToLower())}";
                break;
            case QuestTaskType.OpenTier:
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key.ToLower())}";
                break;
            case QuestTaskType.EnterScene:
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key.ToLower())}";
                break;
            case QuestTaskType.ReachSection:
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal?.Goal.Key.ToLower())}";
                break;
            case QuestTaskType.CreatePickaxe:
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key.ToLower())}";
                _icon.gameObject.SetActive(true);
                _icon.sprite = SpriteHelper.GetIcon(_goal.StaticGoal.Goal.Key);
                break;
            case QuestTaskType.CreateTorch:
                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key.ToLower())}";
                _icon.gameObject.SetActive(true);
                _icon.sprite = SpriteHelper.GetIcon(_goal.StaticGoal.Goal.Key);
                break;
            case QuestTaskType.CreateHilt:
                var goalColor3 = _goal.Progress >= _goal.StaticGoal.Goal.Value
                    ? _collectedColor.CovertToHex()
                    : _inProgressColor.CovertToHex();

                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal.Goal.Key.ToLower())}";
                _quantity.text = $"(<color=#{goalColor3}>{_goal.Progress}</color>/{_goal.StaticGoal.Goal.Value})";
                _icon.gameObject.SetActive(true);
                _icon.sprite = SpriteHelper.GetIcon(_goal.StaticGoal.Goal.Key);

                break;
            case QuestTaskType.UpgradeSkill:

                var goalColor4 = _goal.Progress >= _goal.StaticGoal.Goal.Value
                    ? _collectedColor.CovertToHex()
                    : _inProgressColor.CovertToHex();

                var skillType = (SkillType)Enum.Parse(typeof(SkillType), _goal.StaticGoal.Goal.Key);

                _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>{LocalizationHelper.GetLocale(_goal.StaticGoal?.Goal.Key.ToLower())}";
                _quantity.text = $"(<color=#{goalColor4}>{_goal.Progress}</color>/{_goal.StaticGoal.Goal.Value})";
                _icon.gameObject.SetActive(true);
                _icon.sprite = SpriteHelper.GetSkillIcon(skillType);
                break;
            case QuestTaskType.TimeLeft:

                var date = new DateTime();
                var endTime = _goal.StartTime + _goal.StaticGoal.Goal.Value - TimeManager.Instance.NowUnixSeconds;
                endTime = endTime < 0 ? 0 : endTime;
                date = date.AddSeconds(endTime);

                _quantity.text = date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                _title.text =  $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>";

                _quantity.text = _goal.IsCompleted ? $"<color=#ffffff>00:00:00" :  $" <color=#ffffff>{date.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}</color>";
                _icon.gameObject.SetActive(true);
                _icon.sprite = App.Instance.ReferencesTables.Sprites.AbilityCooldownIcon;

                EventManager.Instance.Subscribe<SecondsTickEvent>(OnSecondTick);

                break;
        }
    }

    private void Clear()
    {
        _title.text = "";
        _quantity.text = "";
        _icon.gameObject.SetActive(false);

    }

    private void OnSecondTick(SecondsTickEvent data)
    {
        if (_goal == null || _goal.StaticGoal.Type != QuestTaskType.TimeLeft)
            return;

        var date = new DateTime();
        var endTime = _goal.StartTime + _goal.StaticGoal.Goal.Value - TimeManager.Instance.NowUnixSeconds;
        endTime = endTime < 0 ? 0 : endTime;
        date = date.AddSeconds(endTime);

        _quantity.text = date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

        _title.text = $"<color=#2A2A2A>{(_goal.IsCompleted ? "" : "-  ")}</color>";

        _quantity.text = _goal.IsCompleted ? $"<color=#ffffff>00:00:00" : $" <color=#ffffff>{date.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}</color>";
        _icon.gameObject.SetActive(true);
        _icon.sprite = App.Instance.ReferencesTables.Sprites.AbilityCooldownIcon;

    }
}
