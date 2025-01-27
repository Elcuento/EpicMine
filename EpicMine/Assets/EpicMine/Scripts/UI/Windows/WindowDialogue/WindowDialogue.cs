using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using DragonBones;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable IdentifierTypo

namespace BlackTemple.EpicMine
{
    public class WindowDialogue : WindowBase
    {

        [Header("")]

        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private Image _characterNameBackground;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _nextButton;

        [Header("Characters")]

        [SerializeField] private UnityArmatureComponent _blacksmith;
        [SerializeField] private Color _blacksmithNameBackgroundColor;
        [SerializeField] private Color _blacksmithNameTextColor;

        [SerializeField] private UnityArmatureComponent _merchant;
        [SerializeField] private Color _merchantNameBackgroundColor;
        [SerializeField] private Color _merchantNameTextColor;

        [SerializeField] private UnityArmatureComponent _burglar;
        [SerializeField] private Color _burglarNameBackgroundColor;
        [SerializeField] private Color _burglarNameTextColor;

        [SerializeField] private UnityArmatureComponent _encelad;
        [SerializeField] private Color _enceladNameBackgroundColor;
        [SerializeField] private Color _enceladNameTextColor;

        [SerializeField] private UnityArmatureComponent _valeri;
        [SerializeField] private Color _valeriNameBackgroundColor;
        [SerializeField] private Color _valeridNameTextColor;

        [Header("Bubble")]

        [SerializeField] private RectTransform _windowBubble;
        [SerializeField] private GameObject _windowBubbleArrowLeft;
        [SerializeField] private GameObject _windowBubbleArrowRight;

        private List<Monologue> _monologues = new List<Monologue>();
        private int _currentMonologueIndex;
        private bool _isTyping;
        private string _dialogId;
        private Action _onClose;
        private Coroutine _dialogCoroutine;



        public void Initialize(Dialogue dialogue, Action onClose = null)
        {
            Debug.Log("init " + dialogue.Id);

            Clear();

            _monologues = App.Instance.StaticData.Monologues.Where(m => m.DialogueId == dialogue.Id).ToList();
            _onClose = onClose;



            if (_monologues.Count == 0)
            {
                App.Instance.Services.LogService.LogError("No monologues in " + dialogue.Id);
            }
            Next();

            EventManager.Instance.Publish(new DialogStartedEvent(dialogue.Id));
            _dialogId = dialogue.Id;
        }

        public void Initialize(List<Monologue> monologues, Action onClose = null)
        {
            Debug.Log("init " + monologues.Count);
            Clear();

            _monologues = monologues;
            _onClose = onClose;

            Next();
        }

        public void Initialize(string phraseLocaleKey, string characterId, string startAnimation, string cycleAnimation, string endAnimation, Action onClose = null)
        {
            Debug.Log("init " + phraseLocaleKey);
            Clear();

            _monologues = new List<Monologue>
            {
                new Monologue(phraseLocaleKey, string.Empty, characterId, startAnimation, endAnimation, cycleAnimation)
            };
            _onClose = onClose;

            Next();
        }


        public void Next()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            if (_currentMonologueIndex >= _monologues.Count)
            {
                _onClose?.Invoke();
                WindowManager.Instance.Close(this, withDestroy: true);
                return;
            }

            _nextButton.gameObject.SetActive(_currentMonologueIndex < _monologues.Count - 1);

            StopAllCoroutines();

            if (_isTyping)
            {
                ShowCurrentMonologue();
                return;
            }

            if(_dialogCoroutine != null)
                StopCoroutine(_dialogCoroutine);

            _dialogCoroutine = StartCoroutine(ShowNextMonologue());
        }


        public override void OnClose()
        {
            EventManager.Instance.Publish(new DialogEndEvent(_dialogId));
            base.OnClose();
            Clear();
        }


        private IEnumerator ShowNextMonologue()
        {
            _isTyping = true;

            var monologue = _monologues[_currentMonologueIndex];
            var locale = LocalizationHelper.GetLocale(monologue.Id);

            ShowCharacter();

            _text.maxVisibleCharacters = 0;
            _text.text = locale;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_text.GetComponent<RectTransform>());


            for (var i = 0; i <= locale.Length; i++)
            {
                _text.maxVisibleCharacters = i;
                _text.gameObject.SetActive(false);
                _text.gameObject.SetActive(true);

                if (i < locale.Length)
                    yield return new WaitForSecondsRealtime(0.025f);
            }



            _isTyping = false;
            _currentMonologueIndex++;
        }

        private void ShowCharacter()
        {
            var monologue = _monologues[_currentMonologueIndex];

            var isAppeared = false;
            var isHeAlsoThePrevious = false;
            if (_currentMonologueIndex > 0)
            {
                for (var i = 0; i < _currentMonologueIndex; i++)
                {
                    var previousMonologue = _monologues[i];
                    if (previousMonologue.CharacterId == monologue.CharacterId)
                        isAppeared = true;

                    if (i == _currentMonologueIndex - 1 && previousMonologue.CharacterId == monologue.CharacterId)
                        isHeAlsoThePrevious = true;
                }
            }

            if (!isHeAlsoThePrevious)
                ClearCharacters();

            var locale = LocalizationHelper.GetLocale(monologue.CharacterId);
            _characterNameText.text = $"{ locale }:";

            switch (monologue.CharacterId)
            {
                case "blacksmith":
                    _blacksmith.gameObject.SetActive(true);
                    _characterNameBackground.color = _blacksmithNameBackgroundColor;
                    _characterNameText.color = _blacksmithNameTextColor;
                    StartCoroutine(PlayAnimations(_blacksmith, monologue.StartAnimation, monologue.CycleAnimation, monologue.EndAnimation, isAppeared, isHeAlsoThePrevious));
                    SetBubblePosition(isLeft: true);
                    break;
                case "merchant":
                    _merchant.gameObject.SetActive(true);
                    _characterNameBackground.color = _merchantNameBackgroundColor;
                    _characterNameText.color = _merchantNameTextColor;
                    StartCoroutine(PlayAnimations(_merchant, monologue.StartAnimation, monologue.CycleAnimation, monologue.EndAnimation, isAppeared, isHeAlsoThePrevious));
                    SetBubblePosition(isLeft: true);
                    break;
                case "burglar":
                    _burglar.gameObject.SetActive(true);
                    _characterNameBackground.color = _burglarNameBackgroundColor;
                    _characterNameText.color = _burglarNameTextColor;
                    StartCoroutine(PlayAnimations(_burglar, monologue.StartAnimation, monologue.CycleAnimation, monologue.EndAnimation, isAppeared, isHeAlsoThePrevious));
                    SetBubblePosition(isLeft: false);
                    break;
                case "encelad":
                    _encelad.gameObject.SetActive(true);
                    _characterNameBackground.color = _enceladNameBackgroundColor;
                    _characterNameText.color = _enceladNameTextColor;
                    StartCoroutine(PlayAnimations(_encelad, monologue.StartAnimation, monologue.CycleAnimation, monologue.EndAnimation, isAppeared, isHeAlsoThePrevious));
                    SetBubblePosition(isLeft: false);
                    break;
                case "valeri":
                    _valeri.gameObject.SetActive(true);
                    _characterNameBackground.color = _valeriNameBackgroundColor;
                    _characterNameText.color = _valeridNameTextColor;
                    StartCoroutine(PlayAnimations(_valeri, monologue.StartAnimation, monologue.CycleAnimation, monologue.EndAnimation, isAppeared, isHeAlsoThePrevious));
                    SetBubblePosition(isLeft: true);
                    break;
            }
        }

        private void ShowCurrentMonologue()
        {
            _text.maxVisibleCharacters = _text.text.Length;
            _isTyping = false;
            _currentMonologueIndex++;
        }


        private IEnumerator PlayAnimations(UnityArmatureComponent character, string startAnimation, string cycleAnimation, string endAnimation, bool isAppeared, bool isHeAlsoThePrevious)
        {
            if (!isAppeared)
            {
                character.animation.Play("Release", 1);
                yield return new WaitForSecondsRealtime(character.animation.animations["Release"].duration);
            }

            if (isHeAlsoThePrevious)
            {
                character.animation.FadeIn("Tap", 0.1f, 1);
                yield return new WaitForSecondsRealtime(character.animation.animations["Tap"].duration);
                if (!string.IsNullOrEmpty(endAnimation))
                {
                    character.animation.FadeIn(endAnimation, 0.1f, 1);
                    yield return new WaitForSecondsRealtime(character.animation.animations[endAnimation].duration);
                }
            }

            if (!string.IsNullOrEmpty(startAnimation))
            {
                character.animation.FadeIn(startAnimation, 0.1f, 1);
                yield return new WaitForSecondsRealtime(character.animation.animations[startAnimation].duration);
            }

            character.animation.FadeIn(cycleAnimation, 0.1f);
        }

        private void SetBubblePosition(bool isLeft = true)
        {
            _windowBubble.anchoredPosition = new Vector2(isLeft ? -310f : 310f, 0f);
            _windowBubbleArrowLeft.SetActive(!isLeft);
            _windowBubbleArrowRight.SetActive(isLeft);
            _nextButton.anchoredPosition = new Vector2(isLeft ? 0f : 520f, 0f);
        }

        private void Clear()
        {
            _currentMonologueIndex = 0;
            _monologues.Clear();
            _text.text = string.Empty;
            _dialogId = string.Empty;
            ClearCharacters();
        }

        private void ClearCharacters()
        {
            _characterNameText.text = string.Empty;

            _blacksmith.gameObject.SetActive(false);
            _merchant.gameObject.SetActive(false);
            _burglar.gameObject.SetActive(false);
            _encelad.gameObject.SetActive(false);
            _valeri.gameObject.SetActive(false);

            _blacksmith.animation?.Stop();
            _burglar.animation?.Stop();
            _merchant.animation?.Stop();
            _encelad.animation?.Stop();
            _valeri.animation?.Stop();

            _blacksmith.animation?.Reset();
            _burglar.animation?.Reset();
            _merchant.animation?.Reset();
            _encelad.animation?.Reset();
            _valeri.animation?.Reset();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}