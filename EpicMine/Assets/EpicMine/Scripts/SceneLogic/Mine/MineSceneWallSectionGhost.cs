using System;
using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

public class MineSceneWallSectionGhost : MonoBehaviour
{
    [SerializeField] private MineSceneSection _section;

    [SerializeField] private Transform _ghostContainer;
    [SerializeField] private ClickHandler _clickHandler;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _ghostSpeech;

    private Ghost _staticGhost;

    private BlackTemple.EpicMine.Core.Tier _tier;
    private BlackTemple.EpicMine.Core.Mine _mine;

    private bool _isSpeaking;
    private GhostActionType _action;

    public void OnDestroy()
    {
        _clickHandler.PointClickEvent -= OnClick;

        if (EventManager.Instance == null)
            return;
    }


    public void Initialize(Ghost ghost, MineSceneSection section)
    {
        _canvas.gameObject.SetActive(false);

        _section = section;
        _staticGhost = ghost;

        _tier = App.Instance
            .Services
            .RuntimeStorage
            .Load<BlackTemple.EpicMine.Core.Tier>(RuntimeStorageKeys.SelectedTier);

        _mine = App.Instance
            .Services
            .RuntimeStorage
            .Load<BlackTemple.EpicMine.Core.Mine>(RuntimeStorageKeys.SelectedMine);

        _clickHandler.PointClickEvent += OnClick;
    }


    public bool Show()
    {
        var currentActionCount = _tier.GhostActionsCount;
        currentActionCount++;

        if (currentActionCount > _staticGhost.Actions.Count)
            return false;
        
        _canvas.gameObject.SetActive(true);

        _action = (GhostActionType)_staticGhost.Actions[currentActionCount - 1];

      //  _action = GhostActionType.Speak;

        var prefab = Resources.Load<MineSceneGhost>(Paths.ResourcesPrefabsGhostsPath + _staticGhost.Id.ToString().ToLower());
        Instantiate(prefab, _ghostContainer, false)
            .Initiate(_clickHandler, OnGhostLeave, OnGhostSpeak, _staticGhost, _action);

        EventManager.Instance.Publish(new TierGhostAppearEvent(_action == GhostActionType.Speak));
        return true;
    }

    public void OnGhostSpeak(string str, Action readEnd)
    {
        StartCoroutine(ShowSpeaking(str, readEnd));
    }

    public void OnGhostLeave()
    {
        _ghostSpeech.text = "";

        _mine.SetGhost();
         
        _clickHandler.gameObject.SetActive(false);

        EventManager.Instance.Publish(new TierGhostDisappearEvent(_action));
    }

    public void OnClick()
    {
        _isSpeaking = false;
    }


    private IEnumerator ShowSpeaking(string text, Action readEnd)
    {
        _ghostSpeech.text = "";

        _isSpeaking = true;

        _ghostSpeech.maxVisibleCharacters = 0;
        _ghostSpeech.text = text;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_ghostSpeech.GetComponent<RectTransform>());

        for (var i = 0; i <= text.Length; i++)
        {
            _ghostSpeech.maxVisibleCharacters = i;
            _ghostSpeech.gameObject.SetActive(false);
            _ghostSpeech.gameObject.SetActive(true);

            if (i < text.Length)
                yield return new WaitForSecondsRealtime(0.025f);

            if (!_isSpeaking)
            {
                _ghostSpeech.maxVisibleCharacters = _ghostSpeech.text.Length;
                _ghostSpeech.gameObject.SetActive(true);
                break;
            }
        }

        readEnd?.Invoke();
        _isSpeaking = false;
    }

}
