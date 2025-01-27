
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player = BlackTemple.EpicMine.Core.Player;


namespace BlackTemple.EpicMine
{
    public class EntryPointSceneController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _loadingText;

        [SerializeField] private GameObject _loadingPanel;

        [SerializeField] private GameObject _buttonsPanel;

        [SerializeField] private GameObject _newsArrow;

        private bool _pressed;

        public void OnClickPlay()
        {
            if (_pressed)
                return;

            _pressed = true;
            _loadingPanel.SetActive(true);
            _buttonsPanel.SetActive(false);

            StartCoroutine(LoadProfileCoroutine(OnPlayerInitialized));
        }


        private IEnumerator LoadProfileCoroutine(Action<Player> onEndAction)
        {
            Player player = null;

            var path = Application.persistentDataPath;

            var thread = new Thread(() =>
            {
                player = LoadProfile(path);
            });

            thread.Start();

            yield return new WaitUntil(() => player != null);

            onEndAction?.Invoke(player);
        }
        private Player LoadProfile(string path)
        {
            var dtoPlayer = Player.Load(path);

            try
            {
               return  new Player(dtoPlayer);
            }
            catch (Exception e)
            {
               Debug.LogError(e);
               return  new Player(Player.CreateNew());
            }

        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;
        }

        private void Start()
        {
            App.Instance.Services.LogService.Log("Start initialize");

            App.Instance.Touch();
            TimeManager.Instance.Touch();

            var windowFade = WindowManager.Instance.Show<WindowFade>(withSound: false);
            windowFade.FadeIn(new WindowFadeSettings(0f, Color.black));

            var text = Application.version;
            _loadingText.text = text;
            windowFade.FadeOut(new WindowFadeSettings(1f, Color.black));

        }

        private void OnPlayerInitialized(Player player)
        {
            App.Instance.Initialize(player);

            App.Instance.Services.LogService.Log($"User id: {player.Id}");
            App.Instance.Services.AnalyticsService.SetUserId(player.Id);

          //  _loadingText.text = $"{player.Id}/{Application.version}";

            App.Instance.Controllers.TutorialController.Initialize(App.Instance.Player.TutorialStepId);
            App.Instance.Controllers.NotificationsController.Initialize();

            OnInitialize();
        }


        private void OnInitialize()
        {
            EventManager.Instance.Publish(new ProfileInitializeCompletedEvent());

            if (!App.Instance.Controllers.TutorialController.IsComplete)
            {
                var isLearnMiningBasicStepComplete = App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.LearnMiningBasic);

                if (!isLearnMiningBasicStepComplete)
                {
                    var tier = App.Instance.Player.Dungeon.Tiers.First();
                    App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, tier);
                    App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, tier.Mines.First());
                    SceneManager.Instance.LoadScene(ScenesNames.Mine);
                    return;
                }

                var isNowOnFirstTimeDiedStep = App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting) &&
                                               !App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FirstTimeDiedOnBoss);

                if (isNowOnFirstTimeDiedStep)
                {
                    var tier = App.Instance.Player.Dungeon.Tiers.First();
                    App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedTier, tier);
                    App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.SelectedMine, tier.Mines.Last());
                    SceneManager.Instance.LoadScene(ScenesNames.Mine);
                    return;
                }
            }

         

            SceneManager.Instance.LoadScene(ScenesNames.Village);

        }

    }
}
