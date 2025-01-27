using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using QuestTaskGoal = BlackTemple.EpicMine.Core.QuestTaskGoal;

// ReSharper disable IdentifierTypo

public class WindowQuestTracking : WindowBase
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Color _textColor;

    private List<string> _textList = new List<string>();

    private Coroutine _showCoroutine;
    private Vector2 _startPosition;

    private bool _initialized;
    private bool _enable;

    protected override void Awake()
    {
        base.Awake();

        _startPosition = _title.transform.localPosition;
        _title.color = _textColor;
        Clear();
    }

    private void Clear()
    {
        _textList.Clear();
        _title.text = "";
        _showCoroutine = null;
    }

    public void Initialize(QuestTaskGoal goal)
    {
        if (!_initialized)
        {
            _enable = App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.CreatePickaxeFirstPart);
            _initialized = true;

            if(!_enable)
            return;
        }

        if (goal.StaticGoal.Type == QuestTaskType.CollectCurrency)
        {
            if (_textList.Count == 0)
            {
                Close();
            }
            return;
        }

        var text = "";

        if (LocalizationHelper.IsLocaleExist(goal.StaticGoal.Id))
        {
            text = LocalizationHelper.GetLocale(goal.StaticGoal.Id);
        }
        else
        {
            text = $"{LocalizationHelper.GetLocale(goal.StaticGoal.Goal.Key)} ";

            if (goal.IsCompleted || goal.StaticGoal.Goal.Value == goal.Progress)
            {
                text += $"{LocalizationHelper.GetLocale("complete")}";
            }
            else
            {
                text += $"{goal.Progress}/{goal.StaticGoal.Goal.Value}";
            }
        }

        _textList.Add(text);

        if (_showCoroutine == null)
            _showCoroutine = StartCoroutine(ShowText());
    }

    public void Initialize(string goal)
    {
        _textList.Add(goal);

        if (_showCoroutine == null)
            _showCoroutine = StartCoroutine(ShowText());
    }



    public IEnumerator ShowText()
    {
        while (_textList.Count > 0)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.QuestTaskGoalChanged);

            var text = _textList[0];
            _title.text = text;

            var showed = false;

            _title.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).OnComplete(() =>
            {
                _title.transform.DOScale(new Vector3(1f, 1f, 1), 0.1f).OnComplete(() =>
                {
                    _title.transform.DOLocalMoveY(_startPosition.y + 80, 1.5f)
                        .SetEase(Ease.Linear)
                        .SetDelay(0.5f);

                    _title.DOFade(0, 1.5f)
                        .SetDelay(0.5f)
                        .OnComplete(() => { showed = true; });
                });
            });


            yield return new WaitUntil(()=> showed);

            _title.color = new Color(_textColor.r, _textColor.g, _textColor.b, 1);
            _title.transform.localPosition = _startPosition;
            _textList.Remove(text);
        }

        _showCoroutine = null;
        Close();
    }

    public override void OnClose()
    {
        base.OnClose();

        Clear();
    }
}
