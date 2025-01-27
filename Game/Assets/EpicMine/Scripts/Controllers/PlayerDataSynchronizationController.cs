using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Currency = CommonDLL.Dto.Currency;
using QuestTask = CommonDLL.Dto.QuestTask;
using QuestTaskGoal = CommonDLL.Dto.QuestTaskGoal;


namespace BlackTemple.EpicMine
{
    public class PlayerDataSynchronizationController
    {
        /* private const string LocalFileName = "playerDataUpdatePack";

         private readonly int _period;

         private int _secondsFromLastSync;

         private List<Item> _items = new List<Item>();

         private List<Currency> _currencies = new List<Currency>();

         private Dictionary<SkillType, int> _skills = new Dictionary<SkillType, int>();

         private Dictionary<AbilityType, int> _abilities = new Dictionary<AbilityType, int>();

         private Dictionary<AutoMinerUpgradeType, int> _autoMinerUpgrades = new Dictionary<AutoMinerUpgradeType, int>();

         private List<CommonDLL.Dto.Pickaxe> _pickaxes = new List< CommonDLL.Dto.Pickaxe>();

         private List<CommonDLL.Dto.Torch> _torches = new List<CommonDLL.Dto.Torch>();

         private Dictionary<string, int> _adPickaxes = new Dictionary<string, int>();

         private Dictionary<string, int> _adTorches = new Dictionary<string, int>();

         private string _selectedPickaxe = string.Empty;

         private string _selectedTorch = string.Empty;

         // Pvp
         private int _pvpInviteDisable = -1;
         //

         private List<CommonDLL.Dto.Quest> _quests = new List<CommonDLL.Dto.Quest>();

         private List<CommonDLL.Dto.Tier> _tiers = new List<CommonDLL.Dto.Tier>();

         private List<CommonDLL.Dto.Recipe> _recipes = new List<CommonDLL.Dto.Recipe>();

         private TutorialStepIds? _tutorialStepId;

         private readonly IStorageService _storageService = new JsonDiskStorageService();


         public PlayerDataSynchronizationController(int period = 0)
         {
             _period = period;
         }


         public void Subscribe()
         {
             EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnInventoryItemChange);
             EventManager.Instance.Subscribe<InventoryNewItemAddEvent>(OnInventoryItemAdd);
             EventManager.Instance.Subscribe<InventoryItemRemoveExistEvent>(OnInventoryItemRemove);
             EventManager.Instance.Subscribe<CurrencyChangeEvent>(OnCurrencyChange);
             EventManager.Instance.Subscribe<SkillLevelChangeEvent>(OnSkillLevelChange);
             EventManager.Instance.Subscribe<AbilityLevelChangeEvent>(OnAbilityLevelChange);
             EventManager.Instance.Subscribe<AutoMinerChangeCapacityLevelEvent>(OnAutoMinerCapacityLevelChange);
             EventManager.Instance.Subscribe<AutoMinerChangeSpeedLevelEvent>(OnAutoMinerSpeedLevelChange);
             EventManager.Instance.Subscribe<PickaxeHiltFindEvent>(OnPickaxeHiltFound);
             EventManager.Instance.Subscribe<PickaxeCreateEvent>(OnPickaxeCreate);
             EventManager.Instance.Subscribe<PickaxeSelectEvent>(OnPickaxeSelect);
             EventManager.Instance.Subscribe<TorchCreateEvent>(OnTorchCreate);
             EventManager.Instance.Subscribe<TorchSelectEvent>(OnTorchSelect);
             EventManager.Instance.Subscribe<AdPickaxesChangeEvent>(OnAdPickaxesChange);
             EventManager.Instance.Subscribe<AdTorchesChangeEvent>(OnAdTorchesChange);
             EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
             EventManager.Instance.Subscribe<TierUnlockDropItemEvent>(OnTierUnlockDropItem);
             EventManager.Instance.Subscribe<QuestUpdateEvent>(OnQuestUpdate);
             EventManager.Instance.Subscribe<MineChangeEvent>(OnMineChange);
             EventManager.Instance.Subscribe<RecipeChangeEvent>(OnRecipeChanged);
             EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialStepComplete);
             EventManager.Instance.Subscribe<UnscaledSecondsTickEvent>(OnSecondTick);
             EventManager.Instance.Subscribe<PvpInviteEnableEvent>(OnPvpInviteEnable);
         }


         public void Send()
         {
             var sent = false;

             // Pvp
             if (_pvpInviteDisable != -1)
             {
                 var request = new RequestDataUpdatePvpInvite(_pvpInviteDisable == 1);

                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(request, CommandType.UpdatePvpInvite,
                     data => { _pvpInviteDisable = -1; }, withPreLoader: false);
                 sent = true;
             }

             if (_items.Count > 0)
             {

                 var requestAmt =  new RequestDataUpdateInventory(_items);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateInventory,
                     data => { _items.Clear(); }, withPreLoader: false);
                 sent = true;
             }

             if (_currencies.Count > 0)
             {

                 var request = new RequestDataUpdateCurrency(_currencies);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(request, CommandType.UpdateCurrency,
                     data => { _currencies.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_skills.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateSkills(_skills);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateSkills,
                     data => { _skills.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_abilities.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateAbilities(_abilities);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateAbilities,
                     data => { _abilities.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_autoMinerUpgrades.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateMinerLevels(_autoMinerUpgrades);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateAutoMinerLevels,
                     data => { _autoMinerUpgrades.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_pickaxes.Count > 0)
             {
                 var requestAmt = new RequestDataUpdatePickaxe(_pickaxes);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdatePickaxe,
                     data => { _pickaxes.Clear(); }, withPreLoader: false);
                 sent = true;
             }

             if (_torches.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateTorches(_torches);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateTorches,
                     data => { _torches.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_adPickaxes.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateAdPickaxe(_adPickaxes);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateAdPickaxe,
                     data => { _adPickaxes.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_adTorches.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateAdTorches(_adTorches);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateAdTorch,
                     data => { _adTorches.Clear(); }, withPreLoader: false);
                 sent = true;
             }

             if (!string.IsNullOrEmpty(_selectedPickaxe))
             {
                 var requestAmt = new RequestDataUpdateSelectedPickaxe(_selectedPickaxe);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateSelectedPickaxe,
                     data => { _selectedPickaxe = string.Empty; }, withPreLoader: false);

                 sent = true;
             }

             if (!string.IsNullOrEmpty(_selectedTorch))
             {
                 var requestAmt = new RequestDataUpdateSelectedTorch(_selectedTorch);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateSelectedTorch,
                     data => { _selectedTorch = string.Empty; }, withPreLoader: false);

                 sent = true;
             }

             if (_tiers.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateTiers(_tiers);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateTiers,
                     data => { _tiers.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_quests.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateQuests(_quests);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateQuests,
                     data => { _quests.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_recipes.Count > 0)
             {
                 var requestAmt = new RequestDataUpdateRecipes(_recipes);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateRecipes,
                     data => { _recipes.Clear(); }, withPreLoader: false);

                 sent = true;
             }

             if (_tutorialStepId > 0)
             {
                 var requestAmt = new RequestDataUpdateTutorialStepId(_tutorialStepId.Value);
                 AmtNetworkController.Instance.SendNetworkMessage<SendData>(requestAmt, CommandType.UpdateTutorialStepId,
                     data => { _tutorialStepId = null; },withPreLoader:false);

                 sent = true;
             }

             if (!sent)
                 return;

             _secondsFromLastSync = 0;
             App.Instance.Services.LogService.Log("Player data synced");
         }

         public void Save()
         {
             var updatePack = PackData();
             _storageService.Save(LocalFileName, updatePack);
         }

         public void Load()
         {
             if (_storageService.IsDataExists(LocalFileName))
             {
                 var updatePack = _storageService.Load<PlayerDataUpdatePack>(LocalFileName);

                 // Pvp
                 if (updatePack.PvpInviteDisable != -1)
                     _pvpInviteDisable = updatePack.PvpInviteDisable;

                 if (updatePack.Items != null)
                     _items = updatePack.Items;

                 if (updatePack.Currencies != null)
                     _currencies = updatePack.Currencies;

                 if (updatePack.AutoMinerUpgrades!= null)
                     _autoMinerUpgrades = updatePack.AutoMinerUpgrades;

                 if (updatePack.Skills != null)
                     _skills = updatePack.Skills;

                 if (updatePack.Abilities != null)
                     _abilities = updatePack.Abilities;

                 if (updatePack.Pickaxes != null)
                     _pickaxes = updatePack.Pickaxes;

                 if (updatePack.Torches != null)
                     _torches = updatePack.Torches;

                 if (updatePack.AdPickaxes != null)
                     _adPickaxes = updatePack.AdPickaxes;

                 if (updatePack.AdTorches != null)
                     _adTorches = updatePack.AdTorches;

                 if (updatePack.Tiers != null)
                     _tiers = updatePack.Tiers;

                 if (updatePack.Quests!= null)
                     _quests = updatePack.Quests;

                 if (updatePack.Recipes != null)
                     _recipes = updatePack.Recipes;

                 if (!string.IsNullOrEmpty(updatePack.SelectedPickaxe))
                     _selectedPickaxe = updatePack.SelectedPickaxe;

                 if (!string.IsNullOrEmpty(updatePack.SelectedTorch))
                 _selectedTorch = updatePack.SelectedTorch;

                 if (updatePack.TutorialStepId != null)
                     _tutorialStepId = updatePack.TutorialStepId;

                 _storageService.Remove(LocalFileName);
             }
         }

         public void Clear()
         {
             _items = new List<Item>();
             _torches.Clear();
             _currencies.Clear();
             _skills.Clear();
             _abilities.Clear();
             _pickaxes.Clear();
             _adPickaxes.Clear();
             _adTorches.Clear();
             _autoMinerUpgrades.Clear();
             _selectedPickaxe = string.Empty;
             _selectedTorch = string.Empty;
             _tiers.Clear();
             _quests.Clear();
             _recipes.Clear();
             _tutorialStepId = null;
             _secondsFromLastSync = 0;
             _pvpInviteDisable = -1;

             _storageService.Remove(LocalFileName);
         }


         private void OnInventoryItemAdd(InventoryNewItemAddEvent eventData)
         {
             UpdateItem(eventData.Item.Id, eventData.Item.Amount);
         }

         private void OnInventoryItemChange(InventoryItemChangeEvent eventData)
         {
             UpdateItem(eventData.Item.Id, eventData.Item.Amount);
         }

         private void OnInventoryItemRemove(InventoryItemRemoveExistEvent existEventData)
         {
             UpdateItem(existEventData.ItemId);
         }

         private void OnCurrencyChange(CurrencyChangeEvent eventData)
         {
             if (eventData.Currency.Type == CurrencyType.Crystals)
                 return;

             var existCurrencyIndex = _currencies.FindIndex(i => i.Type == eventData.Currency.Type);
             var newCurrency = new Currency(eventData.Currency.Type, eventData.Currency.Amount);

             if (existCurrencyIndex >= 0)
                 _currencies[existCurrencyIndex] = newCurrency;
             else
                 _currencies.Add(newCurrency);
         }

         private void OnSkillLevelChange(SkillLevelChangeEvent eventData)
         {
             _skills[eventData.SkillLevel.Type] = eventData.SkillLevel.Number;
         }

         private void OnAutoMinerCapacityLevelChange(AutoMinerChangeCapacityLevelEvent eventData)
         {
             _autoMinerUpgrades[AutoMinerUpgradeType.Capacity] = eventData.Level.Number;
         }

         private void OnAutoMinerSpeedLevelChange(AutoMinerChangeSpeedLevelEvent eventData)
         {

             _autoMinerUpgrades[AutoMinerUpgradeType.Speed] = eventData.Level.Number;
         }

         private void OnAbilityLevelChange(AbilityLevelChangeEvent eventData)
         {
             _abilities[eventData.AbilityLevel.Type] = eventData.AbilityLevel.Number;
         }

         private void OnPickaxeHiltFound(PickaxeHiltFindEvent eventData)
         {
             UpdatePickaxe(eventData.Pickaxe);
         }

         private void OnPickaxeCreate(PickaxeCreateEvent eventData)
         {
             UpdatePickaxe(eventData.Pickaxe);
         }

         private void OnPickaxeSelect(PickaxeSelectEvent eventData)
         {
             _selectedPickaxe = eventData.Pickaxe.StaticPickaxe.Id;
         }

         private void OnTorchSelect(TorchSelectEvent eventData)
         {
             _selectedTorch = eventData.Torch.StaticTorch.Id;
         }

         private void OnTorchCreate(TorchCreateEvent eventData)
         {
            UpdateTorch(eventData.Torch);
         }

         private void OnAdPickaxesChange(AdPickaxesChangeEvent eventData)
         {
             _adPickaxes = eventData.AdPickaxes.ToDictionary(x => x.Key, x => x.Value);
         }

         private void OnAdTorchesChange(AdTorchesChangeEvent eventData)
         {
             _adTorches = eventData.AdTorches.ToDictionary(x => x.Key, x => x.Value);
         }

         private void OnTierOpen(TierOpenEvent eventData)
         {
             UpdateTier(eventData.Tier);
         }

         private void OnQuestUpdate(QuestUpdateEvent eventData)
         {
             UpdateQuests(eventData.Quest);
         }


         private void OnTierUnlockDropItem(TierUnlockDropItemEvent eventData)
         {
             UpdateTier(eventData.Tier);
         }

         private void OnMineChange(MineChangeEvent eventData)
         {
             UpdateTier(eventData.Mine.Tier);
         }

         private void OnRecipeChanged(RecipeChangeEvent eventData)
         {
             var dtoRecipe = new CommonDLL.Dto.Recipe(eventData.Recipe.StaticRecipe.Id, eventData.Recipe.FoundResources);

             var existItemIndex = _recipes.FindIndex(i => i.Id == dtoRecipe.Id);
             if (existItemIndex >= 0)
                 _recipes[existItemIndex] = dtoRecipe;
             else
                 _recipes.Add(dtoRecipe);
         }

         private void OnTutorialStepComplete(TutorialStepCompleteEvent eventData)
         {
             _tutorialStepId = eventData.Step.Id;
         }


         private void OnSecondTick(UnscaledSecondsTickEvent eventData)
         {
             _secondsFromLastSync++;
             if (_secondsFromLastSync < _period)
                 return;

             Send();
         }


         private void UpdateItem(string itemId, int amount = 0)
         {
             var existItemIndex = _items.FindIndex(i => i.Id == itemId);
             var newItem = new Item(itemId, amount);
             if (existItemIndex >= 0)
                 _items[existItemIndex] = newItem;
             else
                 _items.Add(newItem);
         }

         // pvp
         private void OnPvpInviteEnable(PvpInviteEnableEvent eventData)
         {
             _pvpInviteDisable = eventData.IsEnable ? 1 : 0;
         }

         private void UpdateTier(Core.Tier tier)
         {
            // Debug.Log("Up tier");

             var mines = new List<CommonDLL.Dto.Mine>();
             for (var index = 0; index < tier.Mines.Count; index++)
             {
                 var tierMine = tier.Mines[index];
                 mines.Add(
                     new CommonDLL.Dto.Mine(index,
                         tierMine.IsComplete,
                         tierMine.Rating,
                         tierMine.HardcoreRating,
                         tierMine.Highscore,
                         tierMine.IsHardcoreOn,
                         tierMine.IsGhostAppear));
             }

             _tiers.Remove(_tiers.Find(x => x.Number == tier.Number));
             _tiers.Add(new CommonDLL.Dto.Tier(tier.Number, tier.IsOpen, mines, tier.UnlockedDropItems));
         }

         private void UpdateQuests(Core.Quest quest)
         {
             _quests.Remove(_quests.Find(x => x.id == quest.StaticQuest.Id));

             var tasksData = new List<QuestTask>();
             var questData = new CommonDLL.Dto.Quest(quest.StaticQuest.Id, tasksData, quest.Status, quest.IsTracking);

             foreach (var task in quest.TaskList)
             {
                 var goals = new List<QuestTaskGoal>();
                 var taskData  = new CommonDLL.Dto.QuestTask
                 {
                     goals = goals, id = task.StaticTask.Id, isCompleted = task.IsCompleted
                 };

                 foreach (var a in task.GoalsList)
                 {
                     goals.Add(new QuestTaskGoal
                     {
                         id = a.StaticGoal.Id, isCompleted = a.IsCompleted, progress = a.Progress, startTime = a.StartTime
                     });
                 }

                 tasksData.Add(taskData);
             }
             _quests.Add(questData);
         }

         private void UpdatePickaxe(Core.Pickaxe pickaxe)
         {
             _pickaxes.Remove(_pickaxes.Find(x => x.Id == pickaxe.StaticPickaxe.Id));

             _pickaxes.Add(new CommonDLL.Dto.Pickaxe(pickaxe.StaticPickaxe.Id, pickaxe.IsCreated, pickaxe.IsHiltFound));
         }

         private void UpdateTorch(Core.Torch torch)
         {
             _torches.Remove(_torches.Find(x => x.Id == torch.StaticTorch.Id));

             _torches.Add(new CommonDLL.Dto.Torch(torch.StaticTorch.Id, torch.IsCreated));
         }

         private PlayerDataUpdatePack PackData()
         {
             return new PlayerDataUpdatePack
             {
                 Items = _items,
                 Currencies = _currencies,
                 Skills = _skills,
                 Abilities = _abilities,
                 Pickaxes = _pickaxes,
                 Torches = _torches,
                 AdPickaxes = _adPickaxes,
                 AdTorches = _adTorches,
                 SelectedPickaxe = _selectedPickaxe,
                 SelectedTorch = _selectedTorch,
                 Recipes = _recipes,
                 Tiers = _tiers,
                 TutorialStepId = _tutorialStepId,
                 PvpInviteDisable = _pvpInviteDisable,
                 AutoMinerUpgrades = _autoMinerUpgrades,
                 Quests = _quests
             };
         }*/
    }
}