using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine.Core
{
    public class QuestTaskGoal
    {
        public Action OnChange { get; private set; }

        public CommonDLL.Static.QuestTaskGoal StaticGoal { get; private set; }
        public QuestTask Task { get; private set; }

        public bool IsCompleted => Progress >= StaticGoal.Goal.Value;
        public long StartTime { get; private set; }

        public long Progress;
        public bool Initialized;

        private readonly Action _onGoalReached;

        public QuestTaskGoal(QuestTask task, CommonDLL.Static.QuestTaskGoal goal, CommonDLL.Dto.QuestTaskGoal goalDto, Action onComplete)
        {
            Task = task;
            StaticGoal = goal;
            Progress = goalDto?.progress ?? 0;
            _onGoalReached = onComplete;
            StartTime = goalDto?.startTime ?? 0;
        }

        public QuestTaskGoal(QuestTask task, CommonDLL.Static.QuestTaskGoal goal, bool isCompleted, Action onComplete)
        {
            Task = task;
            StaticGoal = goal;
            Progress = isCompleted ? StaticGoal.Goal.Value : 0;
            StartTime = 0;
            _onGoalReached = onComplete;
        }

        public void Initialize()
        {
            if (StartTime == 0)
                StartTime = TimeManager.Instance.NowUnixSeconds;

             Initialized = true;

            Check();

            if (!IsCompleted ||
                (StaticGoal.Type == QuestTaskType.Collect ||
                 StaticGoal.Type == QuestTaskType.CollectCurrency))
            {
                Subscribe();
            }

            CheckCompleted();
        }

        public void CheckCompleted()
        {
            if (IsCompleted)
                return;

            if (StaticGoal.Type == QuestTaskType.Speak)
            {

                if (Enum.TryParse(StaticGoal.Goal.Key, true, out CharacterType character))
                {
                    if (character == CharacterType.Lupis || character == CharacterType.Brokhard ||
                        character == CharacterType.Velmir)
                    {
                        var firstAppearPlace = App.Instance.StaticData.Ghosts.First(x => x.Id == character);

                        if (firstAppearPlace != null)
                        {
                            var lastOpenTier = App.Instance.Player.Dungeon.LastOpenedTier.Number + 1;

                            if (firstAppearPlace.Tier < lastOpenTier)
                                ReachGoal();
                        }
                    }
                }
            }

            if (StaticGoal.Type == QuestTaskType.TimeLeft)
            {
                Progress = TimeManager.Instance.NowUnixSeconds - StartTime;
                if(Progress > StaticGoal.Goal.Value)
                    ReachGoal();
            }
        }


        public void ReachGoal()
        {
            Progress = StaticGoal.Goal.Value;

            Task.Quest.Save();

            if (Task.IsCompleted || (StaticGoal.Type != QuestTaskType.Collect && StaticGoal.Type != QuestTaskType.CollectCurrency))
                UnSubscribe();

            EventManager.Instance.Publish(new QuestTaskGoalCompleteEvent(this));

            OnChange?.Invoke();
            _onGoalReached?.Invoke();
        }

        public void Complete()
        {
            Progress = StaticGoal.Goal.Value;

            if (StaticGoal.Type == QuestTaskType.Collect || StaticGoal.Type == QuestTaskType.CollectCurrency)
                QuestHelper.ExtractQuestNeeds(this);

            UnSubscribe();
        }

        private void UnSubscribe()
        {
            EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnItemChanged);
            EventManager.Instance.Unsubscribe<InventoryItemAddEvent>(OnItemAdd);
            EventManager.Instance.Unsubscribe<CurrencyChangeEvent>(OnCurrencyChanged);
            EventManager.Instance.Unsubscribe<GiftOpenEvent>(OnGiftOpened);
            EventManager.Instance.Unsubscribe<OpenChestEvent>(OnChestOpen);
            EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);
            EventManager.Instance.Unsubscribe<PickaxeCreateEvent>(OnPickaxeCreated);
            EventManager.Instance.Unsubscribe<SkillLevelChangeEvent>(OnSkillUpgrade);
            EventManager.Instance.Unsubscribe<TorchCreateEvent>(OnTorchCreated);
            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTimeTick);

            SceneManager.Instance.OnSceneChange -= OnSceneChanged;
        }

        private void Subscribe()
        {
            switch (StaticGoal.Type)
            {
                case QuestTaskType.Collect:
                    EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                    EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
                    EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnItemChanged);
                    break;
                case QuestTaskType.Speak:
                    break;
                case QuestTaskType.Kill:
                    EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                    EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
                    break;
                case QuestTaskType.TimeLeft:
                    EventManager.Instance.Subscribe<SecondsTickEvent>(OnTimeTick);
                    break;
                case QuestTaskType.CollectCurrency:
                    EventManager.Instance.Subscribe<CurrencyChangeEvent>(OnCurrencyChanged);
                    break;
                case QuestTaskType.OpenGift:
                    EventManager.Instance.Subscribe<GiftOpenEvent>(OnGiftOpened);
                    break;
                case QuestTaskType.OpenChests:
                    EventManager.Instance.Subscribe<OpenChestEvent>(OnChestOpen);
                    break;
                case QuestTaskType.EnterScene:
                    SceneManager.Instance.OnSceneChange += OnSceneChanged;
                    break;
                case QuestTaskType.OpenTier:
                    EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);
                    break;
                case QuestTaskType.ReachSection:
                    EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                    break;
                case QuestTaskType.CreateHilt:
                    EventManager.Instance.Subscribe<InventoryItemAddEvent>(OnItemAdd);
                    break;
                case QuestTaskType.CreatePickaxe:
                    EventManager.Instance.Subscribe<PickaxeCreateEvent>(OnPickaxeCreated);
                    break;
                case QuestTaskType.UpgradeSkill:
                    EventManager.Instance.Subscribe<SkillLevelChangeEvent>(OnSkillUpgrade);
                    break;
                case QuestTaskType.CreateTorch:
                    EventManager.Instance.Subscribe<TorchCreateEvent>(OnTorchCreated);
                    break;
                default:
                    Debug.LogError($"{StaticGoal.Type} has no subscription :( ");
                    break;
            }
        }



        public void SetProgress(long progress)
        {
            var current = Progress;

            if (current <= progress && current != progress)
            {
                Progress = progress;

                EventManager.Instance.Publish(new QuestTaskGoalChangeEvent(this));
                OnChange?.Invoke();

                if (Progress >= StaticGoal.Goal.Value)
                    ReachGoal();

                if (!IsCompleted)
                {
                    if(StaticGoal.Type == QuestTaskType.TimeLeft)
                        if (progress % 10 != 0)
                            return;

                    Task.Quest.Save();
                }
            }
        }

        private bool CheckPlacementRequirements(MineSceneSection section = null)
        {
            if (StaticGoal.PlacementRequire == null)
                return true;

            var selectedTier = App.Instance.Services.RuntimeStorage.Load<Tier>(RuntimeStorageKeys.SelectedTier);
            var selectedMine = App.Instance.Services.RuntimeStorage.Load<Mine>(RuntimeStorageKeys.SelectedMine);

            var triggers = StaticGoal.PlacementRequire;

            foreach (var req in triggers)
            {
                switch (req.Key)
                {
                    case QuestRequirementsType.Mine:
                        var mine = int.Parse(req.Value);

                        if (selectedMine == null)
                            return false;

                        if (triggers.TryGetValue(QuestRequirementsType.Tier,
                            out var tier))
                        {
                            if (selectedTier == null)
                                return false;
                            
                            var tierNum = int.Parse(tier);

                            if (selectedTier.Number + 1 != tierNum)
                             return false;
                        }

                        if (selectedMine.Number + 1 != mine)
                         return false;

                        break;
                    case QuestRequirementsType.Tier:
                        if (selectedTier == null)
                         return false;
                    
                        var tierNumber = int.Parse(req.Value);
                        if (selectedTier.Number + 1 != tierNumber)
                            return false;

                        break;

                    case QuestRequirementsType.Item:
                    {
                        if (!App.Instance.Player.Inventory.Has(new Item(req.Value, 1)))
                            return false;
                    }
                        break;
                    case QuestRequirementsType.Section:
 
                        if (section == null)
                        return false; 

                        if (section.Number + 1 != int.Parse(req.Value))
                        return false; 

                        break;
                    case QuestRequirementsType.SectionType:
                        if (section == null)
                            return false;

                        if (section.SectionType.ToString() != req.Value)
                            return false; 

                        break;
                }
            }

            return true;
        }


        public void Check()
        {
            var currentProgress = Progress;

            switch (StaticGoal.Type)
            {

                case QuestTaskType.CollectCurrency:
                    var gold = App.Instance.Player.Wallet.GetExistAmount(CurrencyType.Gold);                   
                    var progress = (int) (gold > StaticGoal.Goal.Value ? StaticGoal.Goal.Value : gold);
                    SetProgress(progress);
                    break;
                case QuestTaskType.CreateHilt:
                case QuestTaskType.Collect:
                    var amount = App.Instance.Player.Inventory.GetExistAmount(StaticGoal.Goal.Key);
                    progress = amount > StaticGoal.Goal.Value ? StaticGoal.Goal.Value : amount;
                    SetProgress(progress);
                    break;
                case QuestTaskType.OpenGift:
                case QuestTaskType.OpenChests:
                case QuestTaskType.Speak:
                    break;
                case QuestTaskType.EnterScene:
                    if(SceneManager.Instance.CurrentScene == StaticGoal.Goal.Key)
                        SetProgress(1);
                    break;
                case QuestTaskType.OpenTier:
                        SetProgress(1);
                    break;
                case QuestTaskType.ReachSection:
                    break;
                case QuestTaskType.CreatePickaxe:
                    foreach (var blacksmithPickax in App.Instance.Player.Blacksmith.Pickaxes)
                    {
                        if (blacksmithPickax.IsCreated && blacksmithPickax.StaticPickaxe.Id == StaticGoal.Goal.Key)
                            SetProgress(1);
                    }
                    break;
                case QuestTaskType.CreateTorch:
                    foreach (var torch in App.Instance.Player.TorchesMerchant.Torches)
                    {
                        if (torch.IsCreated && torch.StaticTorch.Id == StaticGoal.Goal.Key)
                            SetProgress(1);
                    }
                    break;
                case QuestTaskType.UpgradeSkill:
                    var skill = StaticGoal.Goal.Key == SkillType.Crit.ToString() 
                        ? App.Instance.Player.Skills.Crit : StaticGoal.Goal.Key == SkillType.Damage.ToString() 
                            ? App.Instance.Player.Skills.Damage : App.Instance.Player.Skills.Fortune;

                    progress = skill.Number + 1 > StaticGoal.Goal.Value ? StaticGoal.Goal.Value : skill.Number + 1;
                    SetProgress(progress);
                    break;

            }

            if (Progress >= StaticGoal.Goal.Value && Progress <= currentProgress && Progress != currentProgress)
                ReachGoal();
           
        }

        private void OnSceneChanged(string scenePrev, string sceneNew)
        {
            if (!CheckPlacementRequirements())
                return;

            Check();
        }

        public void OnSectionPassed(MineSceneSectionPassedEvent evenData)
        {
            switch (StaticGoal.Type)
            {
                case QuestTaskType.Collect:
                    break;
                case QuestTaskType.Speak:
                    break;
                case QuestTaskType.Kill:
                    var monster = evenData.Section as MineSceneMonsterSection;
                    if (monster != null)
                    {

                        if(StaticGoal.Goal.Key == monster.ItemId)
                            SetProgress(Progress + 1);
                    }

                    var boss = evenData.Section as MineSceneBossSection;
                    if (boss != null)
                    {
                        if (StaticGoal.Goal.Key == boss.ItemId)
                            SetProgress(Progress + 1);
                    }
                    break;
            }
            Check();
        }


        private void OnPickaxeCreated(PickaxeCreateEvent eventData)
        {
            if (!CheckPlacementRequirements(section:null))
                return;
            switch (StaticGoal.Type)
            {
                case QuestTaskType.CreatePickaxe:
                    if (eventData.Pickaxe.StaticPickaxe.Id == StaticGoal.Goal.Key)
                    {
                        SetProgress(1);
                        Check();
                    }
                    break;
            }
        }
        private void OnTorchCreated(TorchCreateEvent eventData)
        {
            if (!CheckPlacementRequirements(section: null))
                return;

            switch (StaticGoal.Type)
            {
                case QuestTaskType.CreateTorch:
                    if (eventData.Torch.StaticTorch.Id == StaticGoal.Goal.Key)
                    {
                        SetProgress(1);
                        Check();
                    }
                    break;
            }
        }

        private void OnSkillUpgrade(SkillLevelChangeEvent eventData)
        {
            if (!CheckPlacementRequirements(section: null))
                return;

            if (eventData.SkillLevel.Type.ToString() == StaticGoal.Goal.Key)
            {
                SetProgress(eventData.SkillLevel.Number + 1);
                Check();
            }
        }

        private void OnTimeTick(SecondsTickEvent data)
        {
            if (StaticGoal.Type == QuestTaskType.TimeLeft)
            {
                SetProgress(Progress  + 1);
                

                Check();
            }
        }

        public void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            if (!CheckPlacementRequirements(section: eventData.Section))
                return;

            switch (StaticGoal.Type)
            {
                case QuestTaskType.Collect:
                    var collectsItem =
                        App.Instance.StaticData.QuestTaskGoalsCollect.FindAll(x => x.Id == StaticGoal.Goal.Key);

                    foreach (var collectItem in collectsItem)
                    {
                        switch (collectItem.SourceType)
                        {
                            case QuestTaskGoalCollectSourceType.MonsterType:
                                if (eventData.Section is MineSceneMonsterSection monster)
                                {
                                    if (monster.StaticMonster.Type.ToString() == collectItem.Source)
                                    {
                                        monster.AddExtraDrop(collectItem.Id, collectItem.Chance);
                                    }
                                }

                                break;
                            case QuestTaskGoalCollectSourceType.MonsterId:
                                if (eventData.Section is MineSceneMonsterSection monster2)
                                {
                                    if (monster2.StaticMonster.Id == collectItem.Source)
                                    {
                                        monster2.AddExtraDrop(collectItem.Id, collectItem.Chance);
                                    }
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }                

                    break;
                case QuestTaskType.ReachSection:
                    SetProgress(1);
                    break;
            }

            Check();
        }

        public void OnCurrencyChanged(CurrencyChangeEvent eventData)
        {
            Check();
        }

        public void OnItemAdd(InventoryItemAddEvent eventData)
        {
            Check();
        }

        public void OnItemChanged(InventoryItemChangeEvent eventData)
        {
            Check();
        }

        public void OnGiftOpened(GiftOpenEvent eventData)
        {
            if (StaticGoal.Type == QuestTaskType.OpenGift)
            {
               SetProgress(Progress + 1);
               Check();

            }
        }

        public void OnTierOpen(TierOpenEvent eventData)
        {
            if (StaticGoal.Type == QuestTaskType.OpenTier)
            {
                if ((eventData.Tier.Number + 1) == StaticGoal.Goal.Value)
                {
                    SetProgress(1);
                    Check();
                }
            }
        }

        public void OnChestOpen(OpenChestEvent eventData)
        {
            if (StaticGoal.Type == QuestTaskType.OpenChests)
            {
                if (eventData.Type.ToString() == StaticGoal.Goal.Key)
                {
                    SetProgress(Progress + 1);
                    Check();
                }
            }
            
        }

        public void SubscribeUpdate(Action method)
        {
            OnChange += method;
        }

        public void UnSubscribeUpdate(Action method)
        {
            OnChange -= method;
        }

        public void Deactivate()
        {
            OnChange = null;
            UnSubscribe();
        }
    }
}