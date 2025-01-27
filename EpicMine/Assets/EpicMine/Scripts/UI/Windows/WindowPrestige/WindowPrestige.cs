
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Burglar = BlackTemple.EpicMine.Core.Burglar;
using DailyTasks = BlackTemple.EpicMine.Core.DailyTasks;
using Debug = System.Diagnostics.Debug;
using Gifts = BlackTemple.EpicMine.Core.Gifts;
using Inventory = BlackTemple.EpicMine.Core.Inventory;
using Quests = BlackTemple.EpicMine.Core.Quests;
using Tier = CommonDLL.Static.Tier;
using Workshop = BlackTemple.EpicMine.Core.Workshop;

namespace BlackTemple.EpicMine
{
    public class WindowPrestige : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _monologueText;
        [SerializeField] private GameObject _warningPanel;
        [SerializeField] private GameObject _choicePanel;
        [SerializeField] private GameObject _completePanel;

        [SerializeField] private Image[] _levelIcons;
        [SerializeField] private Image _pickaxeIcon;
        [SerializeField] private Image _pickaxeBackground;

        private bool _isWaitingForClick;
        private bool _isReceivedReward;

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            _warningPanel.SetActive(false);
            _choicePanel.SetActive(false);
            _completePanel.SetActive(false);

            Initialize(IsRewardTaken());
        }

        public bool IsRewardTaken()
        {
            var allPickaxes = App.Instance.StaticData.Pickaxes.Where(x => x.Type == PickaxeType.God).ToList();
            if (App.Instance.Player.Prestige >= allPickaxes.Count)
            {
                return true;
            }
            else
            {
                var staticPickaxe = allPickaxes[App.Instance.Player.Prestige];
                var pickaxe = App.Instance.Player.Blacksmith.Pickaxes.Find(x => x.StaticPickaxe.Id == staticPickaxe.Id);

                return pickaxe!=null && pickaxe.IsCreated;
            }

        }

        public void Initialize(bool isReceivedReward)
        {
            _isReceivedReward = isReceivedReward;

            _monologueText.gameObject.SetActive(true);

            if (App.Instance.Player.Prestige < 5)
            {
                var levelSprite = SpriteHelper.GetPrestigeIcon(App.Instance.Player.Prestige + 1);
                foreach (var icon in _levelIcons)
                    icon.sprite = levelSprite;

                StartCoroutine(ShowEntranceMonologue());
            }
            else
                StartCoroutine(ShowLastMonologue());

            EventManager.Instance.Publish(new DialogStartedEvent());
        }

        public void GetPrestigeReward(Action onComplete, Action onEnd)
        {

            var staticData = App.Instance.StaticData;
            var playerData = App.Instance.Player;

            var staticTiersCount = staticData.Tiers.Count;
            var lastOpenedTierNumber = playerData.Dungeon.Tiers.Count - 1;


            if (lastOpenedTierNumber < staticTiersCount - 1)
            {
                UnityEngine.Debug.LogError("Tiers does not completed");
                return;
            }

            var isLastBossComplete = playerData.Dungeon.Tiers[lastOpenedTierNumber].Mines[5].IsComplete;
            if (!isLastBossComplete)
            {
                SceneManager.Instance.LoadScene(ScenesNames.Village);
                UnityEngine.Debug.LogError("Last mine does not completed");
                return ;
            }

            var userPrestigeLevel = playerData.Prestige;

            var giftPickaxes = new List<CommonDLL.Static.Pickaxe>();
            foreach (var pic in staticData.Pickaxes)
            {
                if (pic.Type == PickaxeType.God)
                    giftPickaxes.Add(pic);
            }

            userPrestigeLevel++;
            if (userPrestigeLevel > giftPickaxes.Count)
            {
                UnityEngine.Debug.LogError("Invalid prestige level");
                return;
            }

            var giftedPickaxe = giftPickaxes[userPrestigeLevel - 1];

            var existPickaxe = playerData.Blacksmith.Pickaxes.Find(x => x.StaticPickaxe.Id == giftedPickaxe.Id);
            if (existPickaxe != null && existPickaxe.IsCreated)
            {
                onComplete?.Invoke();
                UnityEngine.Debug.LogError("reward already taken");
                return ;
            }

            var sprite = SpriteHelper.GetPickaxeImage(giftedPickaxe.Id);
            _pickaxeIcon.sprite = sprite;

            var staticPickaxe = App.Instance.StaticData.Pickaxes.FirstOrDefault(p => p.Id == giftedPickaxe.Id);
            if (staticPickaxe != null)
            {
                _pickaxeBackground.sprite = SpriteHelper.GetPickaxeRarityBackground(staticPickaxe.Rarity);
                App.Instance.Controllers.RedDotsController.AddCustomPickaxe(staticPickaxe.Id);

                var pickaxe = App.Instance.Player.Blacksmith.Pickaxes.FirstOrDefault(p => p.StaticPickaxe.Id == staticPickaxe.Id);
                pickaxe?.Create(force: true);
            }
            onComplete?.Invoke();
        }

        public void OnGetReward()
        {
            _choicePanel.SetActive(false);
            _completePanel.SetActive(true);
        }

        public void RewardReceived()
        {
            StartCoroutine(ShowChoiceMonologue());
        }

        public void Click()
        {
            if (_isWaitingForClick)
                _isWaitingForClick = false;
        }


        public void WarningYesClick()
        {

            var staticData = App.Instance.StaticData;
            var playerData = App.Instance.Player;

            var staticTiersCount = staticData.Tiers.Count;
            var lastOpenedTierNumber = playerData.Dungeon.Tiers.Count - 1;

            if (lastOpenedTierNumber < staticTiersCount - 1)
            {
                UnityEngine.Debug.LogError("Tiers does not completed");
                return ;
            }

            var isLastBossComplete = playerData.Dungeon.Tiers[lastOpenedTierNumber].Mines[5].IsComplete;
            if (!isLastBossComplete)
            {
                UnityEngine.Debug.LogError("Last mine does not completed");
                return;
            }

            var userPrestigeLevel = playerData.Prestige;


            var pickaxes = 0;
            foreach (var pickaxe in staticData.Pickaxes)
            {
                if (pickaxe.Type == PickaxeType.God)
                    pickaxes++;
            }

            userPrestigeLevel++;
            if (userPrestigeLevel > pickaxes)
            {
                UnityEngine.Debug.LogError("Invalid prestige level");
                return ;
            }

            playerData.SetPrestige(userPrestigeLevel);
            playerData.Artefacts.Set(0);


            App.Instance.Player.SetUpgradePrestige(true);

            _choicePanel.SetActive(false);
            _warningPanel.SetActive(false);

            App.Instance.Controllers.AttackPointProbabilityController.Clear();
            App.Instance.Controllers.DailyTasksController.Clear();
            App.Instance.Controllers.RedDotsController.Clear();
            App.Instance.Controllers.RatingsController.Clear();


            App.Instance.Restart();

        }

        public void WarningNoClick()
        {
            _warningPanel.SetActive(false);
            _choicePanel.SetActive(false);

            StartCoroutine(ShowBuyMonologue());
        }

        public void ChoiceYesClick()
        {
            _choicePanel.SetActive(false);
            _warningPanel.SetActive(true);
        }

        public void ChoiceNoClick()
        {
            _warningPanel.SetActive(false);
            _choicePanel.SetActive(false);

            StartCoroutine(ShowBuyMonologue());
        }

        public void ThanksClick()
        {
            _completePanel.SetActive(false);

            StartCoroutine(ShowChoiceMonologue());
        }

        private IEnumerator ShowEntranceMonologue()
        {
            var monologueTypeNumber = App.Instance.Player.Prestige < 4 ? 1 : 2;
            var dialogCount = (_isReceivedReward ? 4 : 5);

            for (var i = 0; i < dialogCount; i++)
            {
                yield return ShowPhrase($"window_prestige_entrance_monologue_{monologueTypeNumber}_{i + 1}");
                _isWaitingForClick = true;
                yield return new WaitWhile(() => _isWaitingForClick);
            }

            if (_isReceivedReward)
                RewardReceived();
             else GetPrestigeReward(OnGetReward, RewardReceived);

            _monologueText.gameObject.SetActive(false);

        }

        private IEnumerator ShowLastMonologue()
        {
            yield return ShowPhrase("window_prestige_last_monologue");

            _isWaitingForClick = true;
            yield return new WaitWhile(() => _isWaitingForClick);

            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
        }

        private IEnumerator ShowChoiceMonologue()
        {
            var monologueTypeNumber = App.Instance.Player.Prestige < 4 ? 1 : 2;

            for (var i = 0; i < 3; i++)
            {
                yield return ShowPhrase($"window_prestige_choice_monologue_{i + 1}{ (monologueTypeNumber == 2 && i == 1 ? "_1" : "") }");
                _isWaitingForClick = i < 2;
                yield return new WaitWhile(() => _isWaitingForClick);
            }

            _choicePanel.SetActive(true);
        }

        private IEnumerator ShowBuyMonologue()
        {
            yield return ShowPhrase("window_prestige_bye_monologue");

            _isWaitingForClick = true;
            yield return new WaitWhile(() => _isWaitingForClick);

            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
        }

        private IEnumerator ShowPhrase(string key)
        {
            var locale = LocalizationHelper.GetLocale(key);
            _monologueText.text = locale;

            for (var i = 0; i <= locale.Length; i++)
            {
                _monologueText.maxVisibleCharacters = i;
                _monologueText.gameObject.SetActive(false);
                _monologueText.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}