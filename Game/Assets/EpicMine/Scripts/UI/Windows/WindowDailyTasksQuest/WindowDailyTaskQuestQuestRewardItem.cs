using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Dto;
using UnityEngine;
using UnityEngine.UI;
using Quest = BlackTemple.EpicMine.Core.Quest;

public class WindowDailyTaskQuestQuestRewardItem : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private ItemView _itemPrefab;
    [Space]
    [SerializeField] private Image _backGround;
    [SerializeField] private Button _completeButton;
    [SerializeField] private Color _uncompletedColor;

    private Quest _quest;

    public void Initialize(Quest quest)
    {
        _quest = quest;

        var isAllCompleted = quest.IsReady;
        _completeButton.image.sprite = isAllCompleted
            ? App.Instance.ReferencesTables.Sprites.ButtonGrown
            : App.Instance.ReferencesTables.Sprites.ButtonGrey;
        _backGround.color = isAllCompleted ? Color.white : _uncompletedColor;

        foreach (var currency in quest.StaticQuest.RewardCurrency)
        {
            var item = Instantiate(_itemPrefab, _content, false);
            item.Initialize(new Currency(currency.Key, currency.Value));
        }

        foreach (var items in quest.StaticQuest.RewardItems)
        {
            var item = Instantiate(_itemPrefab, _content, false);
            item.Initialize(items.Key, items.Value);
        }

        foreach (var features in quest.StaticQuest.RewardFeatures)
        {
            var item = Instantiate(_itemPrefab, _content, false);
            item.Initialize(features);
        }
    }

    public void OnClickComplete()
    {
        if (!_quest.IsReady)
            return;

        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

        _quest.SetComplete();
    }
}
