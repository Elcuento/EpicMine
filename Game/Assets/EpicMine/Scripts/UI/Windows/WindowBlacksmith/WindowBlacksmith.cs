using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class WindowBlacksmith : WindowBase
    {
        public List<WindowBlacksmithPickaxe> Pickaxes { get; private set; }

        [SerializeField] private WindowBlacksmithPickaxe _pickaxePrefab;
        [SerializeField] private Transform _pickaxeContainer;
        [SerializeField] private WindowBlacksmithPickaxeInfo _pickaxeInfo;

        [Space]
        [SerializeField] private ScrollRect _pickaxesScrollRect;

        [Space]
        [SerializeField] private Toggle _blacksmith;
        [SerializeField] private Toggle _mythical;

        [Space]
        [SerializeField] private RedDotBaseView _blacksmithRedDot;
        [SerializeField] private RedDotBaseView _mythicalRedDot;

        [Space]
        [SerializeField] private TextMeshProUGUI _blacksmithText;
        [SerializeField] private TextMeshProUGUI _mythicalText;

        [Space]
        [SerializeField] private Color _activeFilterTabColor;
        [SerializeField] private Color _inactiveFilterTabColor;

        private float _soundTime;


        public void Filter()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            foreach (var pickaxe in Pickaxes)
            {
                switch (pickaxe.Pickaxe.StaticPickaxe.Type)
                {
                    case PickaxeType.Blacksmith:
                        pickaxe.gameObject.SetActive(_blacksmith.isOn);
                        break;
                    case PickaxeType.Ad:
                        pickaxe.gameObject.SetActive(_blacksmith.isOn);
                        break;
                    case PickaxeType.Donate:
                        pickaxe.gameObject.SetActive(_blacksmith.isOn);
                        break;
                    case PickaxeType.Mythical:
                        pickaxe.gameObject.SetActive(_mythical.isOn);
                        break;
                    case PickaxeType.God:
                        pickaxe.gameObject.SetActive(_mythical.isOn);
                        break;
                    case PickaxeType.Reward:
                        pickaxe.gameObject.SetActive(_mythical.isOn);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _pickaxesScrollRect.verticalNormalizedPosition = 1;
            SetFilterTabsColors();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            Pickaxes = new List<WindowBlacksmithPickaxe>();

            var orderedPickaxes = App.Instance.Player.Blacksmith.Pickaxes.OrderBy(p => p.StaticPickaxe.RequiredTierNumber);
            foreach (var pickaxe in orderedPickaxes)
            {
                if ( (pickaxe.StaticPickaxe.Type == PickaxeType.God || pickaxe.StaticPickaxe.Type == PickaxeType.Reward)
                     && !pickaxe.IsCreated)
                    continue;

                var pickaxeItem = Instantiate(_pickaxePrefab, _pickaxeContainer, false);
                pickaxeItem.Initialize(pickaxe, OnPickaxeClick);
                Pickaxes.Add(pickaxeItem);
            }

            Filter();

            var selected = Pickaxes.FirstOrDefault(p => p.Pickaxe == App.Instance.Player.Blacksmith.SelectedPickaxe);
            if (selected)
            {
                OnPickaxeClick(selected);
            }
            else
            {
                var firstUnlocked = Pickaxes.FirstOrDefault(p => p.isActiveAndEnabled);
                if (firstUnlocked != null)
                    OnPickaxeClick(firstUnlocked);
            }
     

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
            {
                OnRedDotBlacksmithChange(App.Instance.Controllers.RedDotsController.NewPickaxes);
                App.Instance.Controllers.RedDotsController.OnBlacksmithChange += OnRedDotBlacksmithChange;
            }

            EventManager.Instance.Subscribe<PickaxeHiltFindEvent>(OnHiltFound);
            EventManager.Instance.Subscribe<PickaxeCreateEvent>(OnPickaxeCreated);
            EventManager.Instance.Subscribe<PickaxeSelectEvent>(OnSelectPickaxe);
            EventManager.Instance.Subscribe<AdPickaxesChangeEvent>(OnAdPickaxesChange);
        }

        private void OnAdPickaxesChange(AdPickaxesChangeEvent eventData)
        {
            foreach (var i in eventData.AdPickaxes)
            {
                foreach (var windowBlacksmithPickaxe in Pickaxes)
                {
                    if (windowBlacksmithPickaxe.Pickaxe.StaticPickaxe.Type != PickaxeType.Ad)
                        continue;

                    if (windowBlacksmithPickaxe.Pickaxe.StaticPickaxe.Id != i.Key)
                        continue;

                    windowBlacksmithPickaxe.UpdateAdVal(i.Value);
                }
            }

            _pickaxeInfo.ReInitialize();
        }

        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
            _pickaxeContainer.ClearChildObjects();
            Pickaxes.Clear();
        }


        private void Update()
        {
            if (_soundTime > 0)
            {
                _soundTime -= Time.deltaTime;
                return;
            }

            _soundTime = Random.Range(3f, 8f);
            var randomSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.BlacksmithSounds.Length);
            var randomSound = App.Instance.ReferencesTables.Sounds.BlacksmithSounds[randomSoundIndex];
            AudioManager.Instance.PlaySound(randomSound);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PickaxeHiltFindEvent>(OnHiltFound);
                EventManager.Instance.Unsubscribe<PickaxeCreateEvent>(OnPickaxeCreated);
                EventManager.Instance.Unsubscribe<PickaxeSelectEvent>(OnSelectPickaxe);
                EventManager.Instance.Unsubscribe<AdPickaxesChangeEvent>(OnAdPickaxesChange);
            }

            if (App.Instance != null)
            {
                App.Instance.Controllers.RedDotsController.OnBlacksmithChange -= OnRedDotBlacksmithChange;
            }
        }

        private void OnPickaxeClick(WindowBlacksmithPickaxe pickaxe)
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            foreach (var pickax in Pickaxes)
                pickax.RemoveActive();

            pickaxe.SetActive();
            pickaxe.HideRedDot();
            _pickaxeInfo.Initialize(pickaxe.Pickaxe);

            App.Instance.Controllers.RedDotsController.ViewPickaxe(pickaxe.Pickaxe.StaticPickaxe.Id);
        }

        private void OnHiltFound(PickaxeHiltFindEvent eventData)
        {
            if (!eventData.Pickaxe.IsUnlocked)
                return;

            foreach (var pickaxe in Pickaxes)
            {
                if (pickaxe.Pickaxe != eventData.Pickaxe)
                    continue;
                
                pickaxe.RemoveLocked();
                break;
            }
        }

        private void OnRedDotBlacksmithChange(List<string> unlockedPickaxes)
        {
            var blacksmithCount = 0;
            var mythicalCount = 0;

            foreach (var pickaxe in Pickaxes)
            {
                pickaxe.HideRedDot();
                if (unlockedPickaxes.Contains(pickaxe.Pickaxe.StaticPickaxe.Id))
                {
                    pickaxe.ShowRedDot();
                    switch (pickaxe.Pickaxe.StaticPickaxe.Type)
                    {
                        case PickaxeType.Blacksmith:
                            blacksmithCount++;
                            break;
                        case PickaxeType.Mythical:
                            mythicalCount++;
                            break;
                        case PickaxeType.Donate:
                            blacksmithCount++;
                            break;
                        case PickaxeType.God:
                            mythicalCount++;
                            break;
                        case PickaxeType.Ad:
                            blacksmithCount++;
                            break;
                    }
                }

                _blacksmithRedDot.Show(blacksmithCount);
                _mythicalRedDot.Show(mythicalCount);
            }
        }

        private void OnPickaxeCreated(PickaxeCreateEvent eventData)
        {
            foreach (var pickaxe in Pickaxes)
            {
                if (pickaxe.Pickaxe != eventData.Pickaxe)
                    continue;

                pickaxe.SetCreated();
                break;
            }
        }

        private void OnSelectPickaxe(PickaxeSelectEvent eventData)
        {
            foreach (var pickaxe in Pickaxes)
            {
                if (pickaxe.Pickaxe == eventData.Pickaxe)
                    pickaxe.SetSelected();
                else
                    pickaxe.RemoveSelected();
            }
        }


        private void SetFilterTabsColors()
        {
            _blacksmithText.color = _blacksmith.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _mythicalText.color = _mythical.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
        }
    }
}