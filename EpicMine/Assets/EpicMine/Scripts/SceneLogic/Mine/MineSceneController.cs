using System.Collections;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneController : MineSceneBaseController
    {
        protected Core.Mine _selectedMine;

        private bool _isHardcoreMode;

        protected override void Start()
        {
            Initialize();

            _hero.Initialize(SectionProvider.Sections.FirstOrDefault());
        }

        protected override void Subscribe()
        {

            base.Subscribe();
            EventManager.Instance.Subscribe<MineSceneEndGameEvent>(OnGameEnd);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneEndGameEvent>(OnGameEnd);
            }
        }

        protected override void Initialize()
        {
            _selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            _isHardcoreMode = App.Instance
                .Services
                .RuntimeStorage
                .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

            CreateSectionProvider();

            SectionProvider.Sections.FirstOrDefault()?.SetReady();

        }

        private void OnGameEnd(MineSceneEndGameEvent end)
        {
            if (_isGameEnd) return;
            _isGameEnd = true;

            var energy = _hero.EnergySystem.Value.GetDecrypted();
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.Energy, energy);


            WindowManager.Instance.Show<WindowMineComplete>(withSound: false);
        }


        protected override void CreateSectionProvider()
        {
            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);


            var ghost = MineHelper.GetTierGhost(selectedTier,_selectedMine);

            if(ghost != null)
                App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.TierGhost, ghost);


            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.LearnMiningBasic))
            {
                SectionProvider = new TutorialBlacksmithSectionProvider(_hero);
                return;
            }

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FindChest))
            {
                if (_selectedMine.Number == MineLocalConfigs.TutorialFindChestMineNumber - 1)
                {
                    SectionProvider = new TutorialFindChestSectionProvider(_hero);
                    return;
                }
            }

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting))
            {
                if (_selectedMine.Number == MineLocalConfigs.TutorialFindEnchantedChestMineNumber - 1)
                {
                    SectionProvider = new TutorialFindEnchantedChestSectionProvider(_hero);
                    return;
                }
            }

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ForceOpenChest))
            {
                if (_isHardcoreMode)
                    SectionProvider = new EndlessSectionProvider(_hero, false);
                else
                    SectionProvider = new LimitedSectionProvider(MineLocalConfigs.BlacksmithSectionPositionNumber + _selectedMine.Number, _hero, false);
                return;
            }

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting) && _selectedMine.Number < 5)
            {
                if (_isHardcoreMode)
                    SectionProvider = new EndlessSectionProvider(_hero);
                else
                    SectionProvider = new LimitedSectionProvider(MineLocalConfigs.BlacksmithSectionPositionNumber + _selectedMine.Number, _hero);
                return;
            }

            if (_selectedMine.IsLast)
            {
                if (selectedTier.IsLast)
                {
                    SectionProvider = new GodSectionProvider(_hero);
                    return;
                }

                SectionProvider = new BossSectionProvider(_hero);
                return;
            }

            if (_isHardcoreMode)
            {
                SectionProvider = new EndlessSectionProvider(_hero);
                return;
            }

            SectionProvider = new LimitedSectionProvider(MineLocalConfigs.DefaultSectionsCount, _hero);
        }


        protected override void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            StartCoroutine(MoveHero(eventData.Delay, eventData.Section));
        }

        protected override IEnumerator MoveHero(float delay, MineSceneSection passedSection)
        {
            yield return new WaitForSeconds(delay);

            if (passedSection is MineSceneBlacksmithSection)
            {
                var rating = RatingHelper.GetRating(_hero.Pickaxe.Health);
                _selectedMine.Complete();
                _selectedMine.SetRating(rating);
                _hero.MoveForward(1);
                yield break;
            }

            if (passedSection is MineSceneDoorSection)
            {
                if (!_selectedMine.IsComplete)
                    _selectedMine.Complete();

                var rating = RatingHelper.GetRating(_hero.Pickaxe.Health);

                _selectedMine.SetRating(rating);
                _hero.MoveForward(1);
                yield break;
            }

            if (passedSection is MineSceneLastDoorSection)
            {
                if (!_selectedMine.IsComplete)
                    _selectedMine.Complete();

                _hero.MoveForward(1);
                yield break;
            }


            if (passedSection is MineSceneBossSection)
            {
                if (!_selectedMine.IsComplete)
                    _selectedMine.Complete();

              
                var rating = RatingHelper.GetRating(_hero.Pickaxe.Health);
                _selectedMine.SetRating(rating, _isHardcoreMode);


            }

            if (_isHardcoreMode)
            {
                if (!(passedSection is MineSceneBossSection) && !(passedSection is MineSceneEmptySection))
                {
                    var rating = RatingHelper.GetMineHardcoreRating(passedSection.Number + 1);
                    _selectedMine.SetRating(rating, true);
                }
            }

            _hero.MoveToNextNotEmptySection();
        }


        protected override void OnPickaxeDestroyed(MineScenePickaxeDestroyedEvent data)
        {
            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            selectedMine.SetHighscore((Hero.CurrentSection != null ? _hero.CurrentSection.Number : 0));

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.LearnMiningBasic))
            {
                WindowManager
                    .Instance
                    .Show<WindowPickaxeDestroyedCutDown>(withPause: true, withSound: false)
                    .Initialize(
                        (Hero.CurrentSection != null ? _hero.CurrentSection.Number : 0),
                        selectedMine,
                        OnClickRestart);
                return;
            }

            var isContinueAvailable = false;

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
                isContinueAvailable = IsContinueAvailable();
            else if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting))
            {
                var firstTimeDiedOnBossStep =
                    (FirstTimeDiedOnBossTutorialStep) App.Instance.Controllers.TutorialController
                        .Steps
                        .FirstOrDefault(s => s.Id == TutorialStepIds.FirstTimeDiedOnBoss);

                var secondTimeDiedOnBossStep =
                    (SecondTimeDiedOnBossTutorialStep) App.Instance.Controllers.TutorialController
                        .Steps
                        .FirstOrDefault(s => s.Id == TutorialStepIds.SecondTimeDiedOnBoss);

                var stepExists = firstTimeDiedOnBossStep != null && secondTimeDiedOnBossStep != null;
                if (!stepExists)
                    isContinueAvailable = IsContinueAvailable();
                else
                {
                    // minus 1 because window pickaxe destroyed are showed earlier than tutorial step
                    var isFirstTimeStepReady = firstTimeDiedOnBossStep.PickaxeDestroyedCount ==
                                               TutorialLocalConfigs.FirstTimeDiedOnBossCount - 1;
                    if (firstTimeDiedOnBossStep.IsComplete ||
                        !firstTimeDiedOnBossStep.IsComplete && !isFirstTimeStepReady)
                    {
                        var isSecondTimeStepReady = secondTimeDiedOnBossStep.PickaxeDestroyedCount ==
                                                    TutorialLocalConfigs.SecondTimeDiedOnBossCount - 1;
                        if (secondTimeDiedOnBossStep.IsComplete ||
                            !secondTimeDiedOnBossStep.IsComplete && !isSecondTimeStepReady)
                            isContinueAvailable = IsContinueAvailable();
                    }
                }
            }
            else
                isContinueAvailable = IsContinueAvailable();


            WindowManager
                .Instance
                .Show<WindowPickaxeDestroyed>(withPause: true, withSound: false, withCurrencies: true)
                .Initialize(
                    (Hero.CurrentSection != null ? _hero.CurrentSection.Number : 0),
                    selectedMine,
                    OnClickRestart,
                    OnClickContinue,
                    isContinueAvailable: isContinueAvailable);
        }

        private bool IsContinueAvailable()
        {
            if ((Hero.CurrentSection != null ? _hero.CurrentSection.Number : 0) >= App.Instance.StaticData.Configs.Windows.PickaxeDestroyed.ContinueAvailableSectionsCount)
                return true;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (selectedMine.IsLast)
            {
                var currentSection = SectionProvider.Sections.FirstOrDefault(s => s.Number == (_hero.CurrentSection != null ? _hero.CurrentSection.Number : 0));
                var wallSection = currentSection as MineSceneWallSection;
                if (wallSection != null)
                {
                    var percent = wallSection.Health / wallSection.HealthMax;
                    if (percent <= App.Instance.StaticData.Configs.Windows.PickaxeDestroyed.ContinueAvailableBossHealthPercent)
                        return true;
                }
            }

            return false;
        }

        private void OnClickRestart()
        {
            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            MineHelper.AddEndMiningEventToAnalytics(isAlreadyCompleted: selectedMine.IsComplete);
            SceneManager.Instance.LoadScene(ScenesNames.Mine);
        }

        private void OnClickContinue()
        {
            var healthRefillCount = App.Instance.Services.RuntimeStorage.Load<int>(RuntimeStorageKeys.HealthRefillCount);
            healthRefillCount++;
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.HealthRefillCount, healthRefillCount);

            Hero.Pickaxe.RefillHealth();
        }
    }
}