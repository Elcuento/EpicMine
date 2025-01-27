using System.Collections;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player = BlackTemple.EpicMine.Core.Player;

namespace BlackTemple.EpicMine
{
    public class MinePvpSceneUiProgressPanel : MonoBehaviour
    {
        [SerializeField] private MinePvpSceneController _sceneController;
        [SerializeField] private Slider _leftPlayerProgressSlider;
        [SerializeField] private Slider _rightPlayerProgressSlider;

        [SerializeField] private TextMeshProUGUI _leftPlayerName;
        [SerializeField] private TextMeshProUGUI _rightPlayerName;

        [SerializeField] private Transform _rotatorChestOne;
        [SerializeField] private Transform _rotatorChestTwo;


        private void Awake()
        {
            _rotatorChestOne.transform.DORotate(new Vector3(0, 0, 360), 5, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            _rotatorChestTwo.transform.DORotate(new Vector3(0, 0, -360), 5, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            Subscribe();
        }
        private void OnDestroy()
        {
            Unsubscribe();
        }


        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            var progress = GetProgress(_sceneController.Hero.CurrentSection != null ? _sceneController.Hero.CurrentSection.Number : 0);

            _leftPlayerProgressSlider.DOKill();
            _leftPlayerProgressSlider.DOValue(progress, 0f);
        }

        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            StartCoroutine(UpdateProgress(eventData.Delay));
        }

        private void UpdateOpponentProgress(int currentSection)
        {
            var progress = GetProgress(currentSection);
            var moveTime = MineLocalConfigs.SectionMoveTime;

            _rightPlayerProgressSlider.DOKill();
            _rightPlayerProgressSlider.DOValue(progress, moveTime).SetEase(Ease.Linear);
        }

        private IEnumerator UpdateProgress(float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            var nextSectionNumber = 0;

            var sections = _sceneController.SectionProvider.Sections.Where(s => s.Number > (_sceneController.Hero.CurrentSection != null ? _sceneController.Hero.CurrentSection.Number : 0)).ToList();
            if (sections.Count > 0)
            {
                nextSectionNumber = sections[0].Number;
            }

            var progress = GetProgress(nextSectionNumber);
            var moveTime = MineLocalConfigs.SectionMoveTime;

            _leftPlayerProgressSlider.DOKill();
            _leftPlayerProgressSlider.DOValue(progress, moveTime).SetEase(Ease.Linear);
        }

        private float GetProgress(int sectionNumber)
        {
            return sectionNumber / (float) (PvpLocalConfig.DefaultPvpMineSectionCount);
        }

        public void OnGameStart(PvpArenaStartGameEvent data)
        {

            var matchData = PvpArenaNetworkController.GetMatchData();

            var player = matchData?.Players?.FirstOrDefault(x => x.Id != App.Instance.Player.Id);

            _leftPlayerName.text = App.Instance.Player.Nickname;

            var nname = "";

            if (player != null)
            {
                nname = player.Name;
                nname = nname.Contains(PvpLocalConfig.BotNamePrefix)
                    ? nname.Replace(PvpLocalConfig.BotNamePrefix, "")
                    : nname;
            }
            else
            {
                Debug.LogError("No enemy");
                return;
            }


#if UNITY_EDITOR
            _rightPlayerName.text =
                $"{nname}(Bot = {player.IsBot})";
#else
            _rightPlayerName.text = nname;
#endif
        }

        public void OnSectionPassedOpponent(PvpArenaOpponentSectionPassedEvent data)
        {
            UpdateOpponentProgress(data.Number);
        }

        private void Subscribe()
        {
            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<PvpArenaStartGameEvent>(OnGameStart);
            EventManager.Instance.Subscribe<PvpArenaOpponentSectionPassedEvent>(OnSectionPassedOpponent);
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                EventManager.Instance.Unsubscribe<PvpArenaStartGameEvent>(OnGameStart);
                EventManager.Instance.Unsubscribe<PvpArenaOpponentSectionPassedEvent>(OnSectionPassedOpponent);
            }
        }
    }
}