using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;

using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class RedDotsController
    {
        public event EventHandler<List<string>> OnBlacksmithChange;

        public event EventHandler<List<string>> OnDailyTasksChange;

        public event EventHandler<List<string>> OnRecipesChange;

        public event EventHandler<List<string>> OnShopChange;

        public event EventHandler<bool> OnQuestsChange;

        public event EventHandler<List<string>> OnInventoryChange;

        public event EventHandler<bool> OnLeaderBoardChange;

        public event EventHandler<RedDotSimple> OnSkillsAndAbilitiesChange;

        public event EventHandler<bool> OnSkillsChange;

        public event EventHandler<bool> OnAbilitiesChange;

        public event EventHandler<bool> OnPvpWindowChange;

        public event EventHandler<bool> OnTorchesWindowChange;


        public List<string> NewPickaxes { get; private set; } = new List<string>();

        public List<string> ViewedDailyTasks { get; private set; } = new List<string>();

        public List<string> NewRecipes { get; private set; } = new List<string>();

        public List<string> NewShopPacks { get; private set; } = new List<string>();

        public List<string> ViewedItems { get; private set; } = new List<string>();

        public List<RedDotState> AbilitiesUpgradeAble { get; private set; } = new List<RedDotState>();

        public List<RedDotState> SkillsUpgradeAble { get; private set; } = new List<RedDotState>();

        public RedDotSimple SkillAbilitiesAble { get; private set; } = new RedDotSimple();

        public bool IsNewLeaderBoard { get; private set; }

        public bool IsPvpWindowShowed { get; private set; }

        public bool IsQuestsChangeShowed { get; private set; }

        public bool IsTorchesWindowShowed { get; private set; }

        private const string FileName = "redDotsData";

        private readonly IStorageService _storageService = new JsonDiskStorageService();


        public RedDotsController()
        {

            Subscribe();

            if (_storageService.IsDataExists(FileName))
            {
                var data = _storageService.Load<Dto.RedDots>(FileName);
                NewPickaxes = data.NewPickaxes ?? new List<string>();
                ViewedDailyTasks = data.ViewedDailyTasks ?? new List<string>();
                NewRecipes = data.NewRecipes ?? new List<string>();
                NewShopPacks = data.NewShopPacks ?? new List<string>();
                ViewedItems = data.ViewedItems ?? new List<string>();
                IsNewLeaderBoard = data.IsNewLeaderBoard;
                IsPvpWindowShowed = data.IsPvpWindowShowed;
                IsTorchesWindowShowed = data.IsTorchesWindowShowed;
                AbilitiesUpgradeAble = data.AbilitiesDot ?? new List<RedDotState>();
                SkillsUpgradeAble = data.SkillsDot ?? new List<RedDotState>();
                SkillAbilitiesAble = data.AbilitiesSkillsDot ?? new RedDotSimple();
                IsQuestsChangeShowed = data.IsQuestsChangeShowed;
            }
        }

        public void Save()
        {
            var data = new Dto.RedDots
            {
                NewPickaxes = NewPickaxes,
                ViewedDailyTasks = ViewedDailyTasks,
                NewRecipes = NewRecipes,
                NewShopPacks = NewShopPacks,
                ViewedItems = ViewedItems,
                IsNewLeaderBoard = IsNewLeaderBoard,
                IsPvpWindowShowed = IsPvpWindowShowed,
                IsTorchesWindowShowed = IsTorchesWindowShowed,
                AbilitiesDot = AbilitiesUpgradeAble,
                SkillsDot = SkillsUpgradeAble,
                IsQuestsChangeShowed = IsQuestsChangeShowed,
                AbilitiesSkillsDot = SkillAbilitiesAble
            };

            _storageService.Save(FileName, data);
        }

        public void Clear()
        {
            NewPickaxes?.Clear();
            ViewedDailyTasks?.Clear();
            NewRecipes?.Clear();
            NewShopPacks?.Clear();
            ViewedItems?.Clear();
            IsNewLeaderBoard = false;
            IsPvpWindowShowed = false;
            IsTorchesWindowShowed = false;
            IsQuestsChangeShowed = false;
            AbilitiesUpgradeAble = new List<RedDotState>();
            SkillsUpgradeAble = new List<RedDotState>();
            SkillAbilitiesAble = new RedDotSimple();

            _storageService.Remove(FileName);
        }


        public void ViewPickaxe(string id)
        {
            if (NewPickaxes.Contains(id))
            {
                NewPickaxes.Remove(id);
                OnBlacksmithChange?.Invoke(NewPickaxes);
            }
        }

        public void AddCustomPickaxe(string id)
        {
            if (NewPickaxes.Contains(id))
                return;

            NewPickaxes.Add(id);
            OnBlacksmithChange?.Invoke(NewPickaxes);
        }

        public void ViewShopPack(string id)
        {
            if (NewShopPacks.Contains(id))
            {
                NewShopPacks.Remove(id);
                OnShopChange?.Invoke(NewShopPacks);
            }
        }

        public void ViewItem(string id)
        {
            if (!ViewedItems.Contains(id))
            {
                ViewedItems.Add(id);
                OnInventoryChange?.Invoke(ViewedItems);
            }
        }

        public void ViewDailyTasks()
        {
            foreach (var dailyTask in App.Instance.Controllers.DailyTasksController.Tasks)
            {
                if (dailyTask.IsCompleted)
                    continue;

                if (!ViewedDailyTasks.Contains(dailyTask.StaticTask.Id))
                    ViewedDailyTasks.Add(dailyTask.StaticTask.Id);
            }

            // remove old tasks
            var viewedTasks = new List<string>();
            foreach (var sawDailyTask in ViewedDailyTasks)
            {
                if (App.Instance.Controllers.DailyTasksController.Tasks.FirstOrDefault(t => t.StaticTask.Id == sawDailyTask) != null)
                    viewedTasks.Add(sawDailyTask);
            }

            ViewedDailyTasks = viewedTasks;
            OnDailyTasksChange?.Invoke(ViewedDailyTasks);
        }

        public void ViewRecipe(string id)
        {
            if (NewRecipes.Contains(id))
            {
                NewRecipes.Remove(id);
                OnRecipesChange?.Invoke(NewRecipes);
            }
        }

        public void ViewPvpWindow()
        {
            IsPvpWindowShowed = true;
            OnPvpWindowChange?.Invoke(IsPvpWindowShowed);
        }

        public void ViewTorchesWindow()
        {
            IsTorchesWindowShowed = true;
            OnTorchesWindowChange?.Invoke(IsTorchesWindowShowed);
        }

        public void ViewLeaderBoard()
        {
            IsNewLeaderBoard = false;
            OnLeaderBoardChange?.Invoke(IsNewLeaderBoard);
        }

        public void ViewQuestsWindow()
        {
            var unCompletedQuests = App.Instance.Player.Quests.QuestList
                .Where(x => x.Status == QuestStatusType.Started && x.IsReady).ToList();

            IsQuestsChangeShowed = unCompletedQuests.Count == 0;
            OnQuestsChange?.Invoke(IsQuestsChangeShowed);
        }

        // skills
        public void SetSkillsUpgradeState(Dictionary<SkillType, bool> skills)
        {
            var isViewed = true;
            foreach (var skill in skills)
            {
                var exist = SkillsUpgradeAble.Find(x => x.ValueName == skill.Key.ToString());
                if (exist == null)
                {
                    SkillsUpgradeAble.Add(exist = new RedDotState(skill.Key.ToString(), false, false));
                }

                if (skill.Value)
                {
                    if (skill.Value && skill.Value != exist.State && exist.Viewed)
                    {
                        exist.Viewed = false;
                        isViewed = false;
                    }
                }

                exist.State = skill.Value;
            }

            OnSkillsChange?.Invoke(isViewed);
            SetSkillsAbilitiesViewed(AbilitiesUpgradeAble.All(x=> x.Viewed) && isViewed);
        }

        public void SetSkillsUpgradeViewed(bool viewed)
        {
            foreach (var redDotState in SkillsUpgradeAble)
            {
                redDotState.Viewed = viewed;
            }

            OnSkillsChange?.Invoke(viewed);
            SetSkillsAbilitiesViewed(AbilitiesUpgradeAble.All(x => x.Viewed) && SkillsUpgradeAble.All(x => x.Viewed));
        }

        public void SetAbilitiesUpgradeState(Dictionary<AbilityType, bool> abilities)
        {
            var isViewed = true;

            if (App.Instance.Player.Dungeon.LastOpenedTier.Number + 1 >=
                MineLocalConfigs.ExplosiveStrikeAbilityOpenedAtTier)
            {
                foreach (var ability in abilities)
                {
                    var exist = AbilitiesUpgradeAble.Find(x => x.ValueName == ability.Key.ToString());
                    if (exist == null)
                    {
                        AbilitiesUpgradeAble.Add(exist = new RedDotState(ability.Key.ToString(), false, false));
                    }

                    if (ability.Value && ability.Value != exist.State && exist.Viewed)
                    {
                        exist.Viewed = false;
                        isViewed = false;
                    }

                    exist.State = ability.Value;
                }
            }

            OnAbilitiesChange?.Invoke(isViewed);
            SetSkillsAbilitiesViewed(isViewed && SkillsUpgradeAble.All(x => x.Viewed));

        }
        public void SetAbilitiesUpgradeViewed(bool viewed)
        {
            foreach (var redDotState in AbilitiesUpgradeAble)
            {
                redDotState.Viewed = viewed;
            }

            OnAbilitiesChange?.Invoke(viewed);
            SetSkillsAbilitiesViewed(AbilitiesUpgradeAble.All(x => x.Viewed) && SkillsUpgradeAble.All(x => x.Viewed));

        }

        // all

        public void SetSkillsAbilitiesViewed(bool viewed = false)
        {
            SkillAbilitiesAble.SetView(viewed);

            OnSkillsAndAbilitiesChange?.Invoke(SkillAbilitiesAble);
        }

        private void Subscribe()
        {
            EventManager.Instance.Subscribe<PickaxeHiltFindEvent>(OnPickaxeHiltFound);
            EventManager.Instance.Subscribe<DailyTaskCompleteEvent>(OnDailyTaskComplete);
            EventManager.Instance.Subscribe<DailyTaskTakeEvent>(OnDailyTaskTakeReward);
            EventManager.Instance.Subscribe<RecipeUnlockEvent>(OnRecipeUnlocked);
            EventManager.Instance.Subscribe<InventoryNewItemAddEvent>(OnInventoryNewItemAdd);
            EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnInventoryItemRemove);
            EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);

            EventManager.Instance.Subscribe<QuestStartEvent>(OnQuestsStart);
            EventManager.Instance.Subscribe<QuestTaskCompleteEvent>(OnQuestTaskComplete);
            EventManager.Instance.Subscribe<QuestTaskGoalCompleteEvent>(OnQuestGoalComplete);
        }

        private void OnQuestGoalComplete(QuestTaskGoalCompleteEvent eventData)
        {
            IsQuestsChangeShowed = false;
            OnQuestsChange?.Invoke(IsQuestsChangeShowed);
        }

        private void OnQuestTaskComplete(QuestTaskCompleteEvent eventData)
        {
            IsQuestsChangeShowed = false;
            OnQuestsChange?.Invoke(IsQuestsChangeShowed);
        }

        private void OnQuestsStart(QuestStartEvent eventData)
        {
           IsQuestsChangeShowed = false;
           OnQuestsChange?.Invoke(IsQuestsChangeShowed);
        }

        private void OnInventoryNewItemAdd(InventoryNewItemAddEvent eventData)
        {
            OnInventoryChange?.Invoke(ViewedItems);
        }

        private void OnInventoryItemRemove(InventoryItemRemoveExistEvent eventData)
        {
            OnInventoryChange?.Invoke(ViewedItems);
        }

        private void OnDailyTaskTakeReward(DailyTaskTakeEvent eventData)
        {
            OnDailyTasksChange?.Invoke(ViewedDailyTasks);
        }

        private void OnPickaxeHiltFound(PickaxeHiltFindEvent eventData)
        {
            if (eventData.Pickaxe.StaticPickaxe.Type == PickaxeType.Mythical)
            {
                NewPickaxes.Add(eventData.Pickaxe.StaticPickaxe.Id);
                OnBlacksmithChange?.Invoke(NewPickaxes);
            }
        }

        private void OnDailyTaskComplete(DailyTaskCompleteEvent eventData)
        {
            OnDailyTasksChange?.Invoke(ViewedDailyTasks);
        }

        private void OnRecipeUnlocked(RecipeUnlockEvent eventData)
        {
            NewRecipes.Add(eventData.Recipe.StaticRecipe.Id);
            OnRecipesChange?.Invoke(NewRecipes);
        }

        private void OnTierOpen(TierOpenEvent eventData)
        {
            if (eventData.Tier.Number <= 1)
            {
                foreach (var shopPack in App.Instance.StaticData.ShopPacks)
                {
                    if (shopPack.Type == ShopPackType.Gold && !NewShopPacks.Contains(shopPack.Id))
                        NewShopPacks.Add(shopPack.Id);
                }

                OnShopChange?.Invoke(NewShopPacks);

                IsNewLeaderBoard = true;
                OnLeaderBoardChange?.Invoke(IsNewLeaderBoard);
            }

            foreach (var staticDataPickaxe in App.Instance.StaticData.Pickaxes)
            {
                if (staticDataPickaxe.RequiredTierNumber == eventData.Tier.Number + 1)
                    NewPickaxes.Add(staticDataPickaxe.Id);
            }

            OnBlacksmithChange?.Invoke(NewPickaxes);
        }
    }
}