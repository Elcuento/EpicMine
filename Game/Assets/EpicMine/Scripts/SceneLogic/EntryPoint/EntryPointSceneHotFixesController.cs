using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using Newtonsoft.Json;
using UnityEngine;
using Pickaxe = BlackTemple.EpicMine.Core.Pickaxe;

namespace BlackTemple.EpicMine
{
    public class EntryPointSceneHotFixesController
    {

        public void Run(Action onCompleted)
        {

            // fixes from old pickaxes logic
            UnlockRecipes();
            ResetPickaxe();

            // fixes old tasks for skill, that can not achieve
            RemoveSkillCapTasks();

            // fix whats need to be sended
            ServerFixes(()=> { SecondRun(onCompleted); });
        }

        public void SecondRun(Action onCompleted)
        {
            // fix tutorial stuff
            CheckTutorial(onCompleted);
        }

        private void CheckTutorial(Action onCompleted)
        {
           
            if (App.Instance.Player.Dungeon.LastOpenedTier == null)
            { onCompleted?.Invoke();  return; }

            if (App.Instance.Controllers.TutorialController.IsComplete)
            { onCompleted?.Invoke();  return; }

            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number;

            if(tier >= 1 && !App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
            {
                var lastStep = (TutorialStepIds)Enum.GetValues(typeof(TutorialStepIds)).Length;
       
                var tutorial = App.Instance.Controllers.TutorialController.Steps.Find(x => x.Id == lastStep);

                if (tutorial == null)
                {
                    App.Instance.Services.LogService.Log("Wrong step id on entry point fix update");

                    onCompleted?.Invoke();
                    return;
                }

                tutorial.SetForceComplete();
                App.Instance.Controllers.TutorialController.ReInitialize((int)lastStep);

                onCompleted?.Invoke();
                return;
            }

            onCompleted?.Invoke();
            return;
        }


        private void ServerFixes(Action onCompleted)
        {
            var currentFix = App.Instance.VersionInfo.ServerVersion;

            /* if (App.Instance.Player.Versions.FixVersion != currentFix)
             {
                 App.Instance.Services.LogService.Log("Need fix update");

                 var request = new FixUpdateRequest(FirebaseInstances.Instance.Auth.CurrentUser.Metadata.CreationTimestamp.ToString(),
                     RegionInfo.CurrentRegion.TwoLetterISORegionName);
                 NetworkManager.Instance.Send<FixUpdateResponse>(request, (response) =>
                 {
                     switch (response.Version)
                     {
                         case 1:
                             ReachAbilityAndSkillLevelConsiderCurrentTier();
                             break;
                     }
                     App.Instance.Player.Versions.SetFix(response.Version);
                     App.Instance.UpdateFixVersion(response.Version, response.IsNeedRestart);

                     if (!response.IsNeedRestart)
                     {
                         onCompleted?.Invoke();
                     }

                 }, (a) =>
                 {
                     WindowManager.Instance
                         .Show<WindowInformation>()
                         .Initialize(
                             "Update Error",
                             $"Server update {currentFix} error, please contact with support",
                             "Restart", onClose: ()=> { App.Instance.Restart(); },
                             isNeedLocalizeDescription: false, isNeedLocalizeHeader: false, isNeedLocalizeButton: false);

                 });
             }
             else
             {
                 onCompleted?.Invoke();
             }*/
            onCompleted?.Invoke();
        }

        private void ReachAbilityAndSkillLevelConsiderCurrentTier()
        { /*
            var tier = App.Instance.Player.Dungeon.LastOpenedTier.Number;

            var damagePrice = Math.Round((double)App.Instance.StaticData.DamageLevels.Count /
                                         App.Instance.StaticData.Tiers.Count);

            var fortunePrice = Math.Round((double)App.Instance.StaticData.FortuneLevels.Count /
                                          App.Instance.StaticData.Tiers.Count);

            var criticalPrice = Math.Round((double)App.Instance.StaticData.CritLevels.Count /
                                          App.Instance.StaticData.Tiers.Count);

            var explosionPrice = Math.Round((double)App.Instance.StaticData.ExplosiveStrikeLevels.Count /
                                            App.Instance.StaticData.Tiers.Count);

            var freezePrice = Math.Round((double)App.Instance.StaticData.FreezingLevels.Count /
                                         App.Instance.StaticData.Tiers.Count);

            var acidPrice = Math.Round((double)App.Instance.StaticData.AcidLevels.Count /
                                       App.Instance.StaticData.Tiers.Count);

            // damage


           var damageLevel = tier * damagePrice;
            var fortuneLevel = tier * fortunePrice;
            var criticalLevel = tier * criticalPrice;

            var explosionLevel = tier * explosionPrice - (MineLocalConfigs.ExplosiveStrikeAbilityOpenedAtTier - 1) * explosionPrice;
            var freezeLevel = tier * freezePrice - (MineLocalConfigs.FreezingAbilityOpenedAtTier - 1) * freezePrice;
            var acidLevel = tier * acidPrice - (MineLocalConfigs.AcidAbilityOpenedAtTier - 1) * acidPrice;

            for (var i = App.Instance.Player.Skills.Damage.Number; i < damageLevel && i < App.Instance.StaticData.DamageLevels.Count; i++)
            {
                App.Instance.Player.Skills.Damage.Up(true);
            }

            for (var i = App.Instance.Player.Skills.Fortune.Number; i < fortuneLevel && i < App.Instance.StaticData.FortuneLevels.Count; i++)
            {
                App.Instance.Player.Skills.Fortune.Up(true);
            }

            for (var i = App.Instance.Player.Skills.Crit.Number; i < criticalLevel && i < App.Instance.StaticData.CritLevels.Count; i++)
            {
                App.Instance.Player.Skills.Crit.Up(true);
            }

            //

            for (var i = App.Instance.Player.Abilities.ExplosiveStrike.Number; i < explosionLevel && i < App.Instance.StaticData.ExplosiveStrikeLevels.Count; i++)
            {
                App.Instance.Player.Abilities.ExplosiveStrike.Up(true);
            }

            for (var i = App.Instance.Player.Abilities.Freezing.Number; i < freezeLevel && i < App.Instance.StaticData.FreezingLevels.Count; i++)
            {
                App.Instance.Player.Abilities.Freezing.Up(true);
            }

            for (var i = App.Instance.Player.Abilities.Acid.Number; i < acidLevel && i < App.Instance.StaticData.AcidLevels.Count; i++)
            {
                App.Instance.Player.Abilities.Acid.Up(true);
            }
            */
        }

        private void RemoveSkillCapTasks()
        {
            var tasks = App.Instance.Controllers.DailyTasksController.Tasks;
            var isFixed = false;
            for (var i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].StaticTask.Type == DailyTaskType.CritLevelUp 
                    && App.Instance.Player.Skills.Crit.IsLast)
                {
                    tasks[i].Unsubscribe();
                    tasks.Remove(tasks[i]);
                    i--;
                    isFixed = true;
                    continue;
                }

                if (tasks[i].StaticTask.Type == DailyTaskType.DamageLevelUp
                    && App.Instance.Player.Skills.Damage.IsLast)
                {
                    tasks[i].Unsubscribe();
                    tasks.Remove(tasks[i]);
                    i--;
                    isFixed = true;
                    continue;
                }

                if (tasks[i].StaticTask.Type == DailyTaskType.FortuneLevelUp 
                    && App.Instance.Player.Skills.Fortune.IsLast)
                {
                    tasks[i].Unsubscribe();
                    tasks.Remove(tasks[i]);
                    i--;
                    isFixed = true;
                }
            }

            if (isFixed)
            {
                App.Instance.Controllers.Save();
            }
        }

        private void UnlockRecipes()
        {
            var lockedHiltRecipes = App.Instance.Player.Workshop.Recipes.Where(r => r.IsUnlocked == false);

            foreach (var lockedHiltRecipe in lockedHiltRecipes)
            {
                var allIngredientsExist = true;

                foreach (var ingredient in lockedHiltRecipe.Ingredients)
                {
                    var item = new Item(ingredient.Id, 1);
                    if (!App.Instance.Player.Inventory.Has(item))
                    {
                        allIngredientsExist = false;
                        break;
                    }
                }

                if (allIngredientsExist)
                    lockedHiltRecipe.Unlock();
            }
        }

        private void ResetPickaxe()
        {
            var selectedPickaxe = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe;
            var isTierNumberValid = selectedPickaxe.RequiredTierNumber <= App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;
            var isDamageLevelValid = selectedPickaxe.RequiredDamageLevel <= App.Instance.Player.Skills.Damage.Number + 1;

            if ( (isTierNumberValid || App.Instance.Player.Prestige > 0) && isDamageLevelValid)
                return;

            Pickaxe lastAvailablePickaxe = null;

            foreach (var pickaxe in App.Instance.Player.Blacksmith.Pickaxes)
            {
                if (!pickaxe.IsCreated)
                    continue;

                if (pickaxe.StaticPickaxe.RequiredDamageLevel > App.Instance.Player.Skills.Damage.Number + 1)
                    continue;

                lastAvailablePickaxe = pickaxe;
            }

            if (lastAvailablePickaxe != null)
                App.Instance.Player.Blacksmith.Select(lastAvailablePickaxe);
        }
    }
}