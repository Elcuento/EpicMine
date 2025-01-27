using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class TutorialController
    {
        public List<TutorialStepBase> Steps { get; private set; }
        public List<Core.TutorialUnRowBase> UnRowSteps { get; private set; }

        private readonly IStorageService _storageService = new JsonDiskStorageService();

        public bool IsComplete { get; private set; }

        public string FileName = "tutorialData";

        public void ReInitialize(int lastCompleteStepId)
        {
            foreach (var tutorialStepBase in Steps)
            {
                tutorialStepBase.Clear();
            }

            Initialize(lastCompleteStepId);
        }
        public void Initialize(int lastCompleteStepId)
        {
            Clear();

            Load();

            var all = Enum.GetNames(typeof(TutorialStepIds));

            var lastCompleteStepNumber = Mathf.Clamp(lastCompleteStepId, 0, all.Length);

            if (lastCompleteStepNumber >= (int) TutorialStepIds.EnterSecondTierTutorialStep)
            {
                IsComplete = true;
                return;
            }

            Steps = new List<TutorialStepBase>
            {
                new ShowCinematicTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.ShowCinematic),
                new LearnMiningBasicTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.LearnMiningBasic),
                new BlacksmithGreetingTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.BlacksmithGreeting),
                new CreatePickaxeFirstPartTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.CreatePickaxeFirstPart),
                new CompleteSecondMineTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.CompleteSecondMine),
                new PickUpGiftedResourcesTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.PickUpGiftedResources),
                new CraftResourcesFirstPartTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.CraftResourcesFirstPart),
                new CraftResourcesSecondPartTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.CraftResourcesSecondPart),
                new CreatePickaxeSecondPartTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.CreatePickaxeSecondPart),
                new FindChestTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.FindChest),
                new StartChestBreakingTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.StartChestBreaking),
                new ForceOpenChestTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.ForceOpenChest),
                new PickUpGiftedArtefactsTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.PickUpGiftedArtefacts),
                new ShowCharacteristicsTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.ShowCharacteristics),
                new ShowDailyTasksTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.ShowDailyTasks),
                new FirstEnergyAbilityTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.FirstEnergyAbility),
                new BossMeetingTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.BossMeeting),
                new FirstTimeDiedOnBossTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.FirstTimeDiedOnBoss),
                new SecondTimeDiedOnBossTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.SecondTimeDiedOnBoss),
                new UnlockTierTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.UnlockTier),
                new EnceladAppearTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.EnceladAppear),
                new EnterSecondTierTutorialStep(lastCompleteStepNumber >= (int)TutorialStepIds.EnterSecondTierTutorialStep),
            };

            foreach (var step in Steps)
                step.CheckReady();
        }

        public bool IsStepComplete(TutorialUnRowStepIds step)
        {
            var exist = UnRowSteps.Find(x => x.Id == step);
            return exist != null;
        }

        public void SetStepComplete(TutorialUnRowStepIds step)
        {
            App.Instance.Player.SetTutorialStep((int)step);
  
            var exist = UnRowSteps.Find(x => x.Id == step);

            if (exist != null)
                return;

            UnRowSteps.Add(new Core.TutorialUnRowBase(step));

            Save();
        }

        public void Load()
        {
            if (_storageService.IsDataExists(FileName))
            {
                var data = _storageService.Load<List<TutorialUnRowBase>>(FileName);

                UnRowSteps = new List<Core.TutorialUnRowBase>();

                foreach (var tutorialUnRowBase in data)
                {
                    UnRowSteps.Add(new Core.TutorialUnRowBase(tutorialUnRowBase));
                }
            }
            else
            {
                UnRowSteps = new List<Core.TutorialUnRowBase>();
            }
        }


        public void Save()
        {
            var tutorList = new List<TutorialUnRowBase>();
            foreach (var tut in UnRowSteps)
            {
                tutorList.Add(new TutorialUnRowBase(tut));
            }

            _storageService.Save(FileName, tutorList);
        }

        public bool IsStepComplete(TutorialStepIds number)
        {
            if (IsComplete || Steps == null || Steps.Count <= 0)
                return true;

            var step = Steps.LastOrDefault(s => s.IsComplete);
            return step != null && (int)step.Id >= (int)number;
        }

        public void Clear(bool withFiles = false)
        {
            UnRowSteps = new List<Core.TutorialUnRowBase>();

            if (Steps != null)
            {
                foreach (var tutorialStepBase in Steps)
                    tutorialStepBase.Clear();

                Steps.Clear();
            }

            IsComplete = false;

            if(withFiles)
                Save();
        }

    }
}