using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using System;
using System.Collections.Generic;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace BlackTemple.EpicMine.Core
{
    public class Quest
    {
        public Action OnUpdate { get; private set; }

        public CommonDLL.Static.Quest StaticQuest { get; private set; }

        public List<QuestTask> TaskList { get; private set; }
        
        public bool IsReady
        {
            get
            {
                if (!_isReady)
                {
                    _isReady = App.Instance.Player.Quests.IsAllTasksCompleted(StaticQuest.Tasks);
                    return _isReady;
                }

                return true;
            }
        }

        public QuestStatusType Status { get; private set; }

        public bool IsTracking { get; private set; }

        private bool _isReady;

        public Quest(CommonDLL.Static.Quest staticQuest, CommonDLL.Dto.Quest quest)
        {
            StaticQuest = staticQuest;
            TaskList = new List<QuestTask>();
            Status = quest.status;
            IsTracking = quest.isTracking;

            for (var index = 0; index < staticQuest.Tasks.Count; index++)
            {
                var questTask = staticQuest.Tasks[index];
                var staticTask = App.Instance.StaticData.QuestTasks.Find(x => x.Id == questTask);

                if (staticTask == null)
                {
                    Debug.LogError("Static error on quest task " + staticQuest.Id + ":" + questTask);
                    continue;
                }

                var task = quest.tasks.Find(x => x.id == questTask);

                TaskList.Add(task != null ? new QuestTask(this, staticTask, task) : new QuestTask(this, staticTask));
            }
        }

        public Quest(CommonDLL.Static.Quest staticQuest)
        {
            StaticQuest = staticQuest;

            TaskList = new List<QuestTask>();

            for (var index = 0; index < staticQuest.Tasks.Count; index++)
            {
                var questTask = staticQuest.Tasks[index];
                var staticTask = App.Instance.StaticData.QuestTasks.Find(x => x.Id == questTask);

                if (staticTask == null)
                    continue;

                TaskList.Add(new QuestTask(this, staticTask));
            }
        }

        private void SubscribeTriggers()
        {
            UnSubscribeTriggers();

            var status = Status == QuestStatusType.Activated
                ? StaticQuest.StartTriggerType
                : StaticQuest.ActivateTriggerType;

            switch (status)
            {
                case QuestTriggerType.SectionPassed:
                    EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPass);
                    break;
                case QuestTriggerType.SectionReady:
                    EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
                    break;
                case QuestTriggerType.SectionAppear:
                    EventManager.Instance.Subscribe<MineSceneSectionAppearEvent>(OnSectionAppear);
                    break;
                case QuestTriggerType.SectionExit:
                    EventManager.Instance.Subscribe<MineSceneSectionExitEvent>(OnSectionExit);
                    break;
                case QuestTriggerType.AddItem:
                    EventManager.Instance.Subscribe<InventoryItemChangeEvent>(OnItemAdd);
                    break;
                case QuestTriggerType.EnterScene:
                    SceneManager.Instance.OnSceneChange += OnEnterScene;
                    break;
                case QuestTriggerType.None:
                    break;
            }

        }

        private void UnSubscribeTriggers()
        {
            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPass);
            EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Unsubscribe<MineSceneSectionAppearEvent>(OnSectionAppear);
            EventManager.Instance.Unsubscribe<MineSceneSectionExitEvent>(OnSectionExit);
            EventManager.Instance.Unsubscribe<InventoryItemChangeEvent>(OnItemAdd);
            SceneManager.Instance.OnSceneChange -= OnEnterScene;
        }


        private bool CheckRequirements(MineSceneSection section = null)
        {
            var lastTier = App.Instance.Player.Dungeon.LastOpenedTier;
            var lastMine = lastTier.LastOpenedMine;

            
            var triggers = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTriggerRequirement
                : StaticQuest.StartTriggerRequirement;

            foreach (var req in triggers)
            {
                switch (req.Key)
                {
                    case QuestRequirementsType.Mine:
                        var mineNum = int.Parse(req.Value);

                        if (StaticQuest.ActivateTriggerRequirement.TryGetValue(QuestRequirementsType.Tier,
                            out var tier))
                        {
                            var tierNum = int.Parse(tier);

                            if (lastTier.Number + 1 < tierNum)
                            {
                                return false;
                            }
                            else
                            {
                                if(lastTier.Number + 1 > tierNum)
                                   return true;
                            }


                            if (lastMine.Number + 1 < mineNum)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (lastMine.Number + 1 < mineNum)
                            {
                                return false;
                            }
                        }

                        break;
                    case QuestRequirementsType.Tier:
                        if (lastTier.Number + 1 < int.Parse(req.Value))
                        {
                            return false;
                        }
                        break;

                    case QuestRequirementsType.Item:
                        {
                            if (!App.Instance.Player.Inventory.Has(new Item(req.Value, 1)))
                                return false;
                        }
                        break;
                    case QuestRequirementsType.Section:
                        if (section != null && section.Number < int.Parse(req.Value))
                            return false;
                        break;
                    case QuestRequirementsType.Scene:
                        if (SceneManager.Instance.CurrentScene != req.Value)
                            return false;
                        break;
                    case QuestRequirementsType.SectionType:

                        if (section != null && section.SectionType.ToString() != req.Value)
                            return false;
                        break;
                }
            }

            return true;
        }

        private void OnEnterScene(string from, string to)
        {
            if (!CheckRequirements())
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            if (trigger.Key == QuestTriggerExecuter.Scene)
            {
                // Debug.LogError("on tner scene " + trigger.Value +":" + from + ":" + to);
                if (trigger.Value != to)
                    return;
            }

            if (Status == QuestStatusType.UnActivated)
                SetActivate();
            else if (Status == QuestStatusType.Activated)
                SetStart();
        }


        private void OnItemAdd(InventoryItemChangeEvent eventData)
        {
            if (!CheckRequirements())
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            if (trigger.Key == QuestTriggerExecuter.Item)
            {
                if (trigger.Value != eventData.Item.Id)
                    return;
            }

            if (Status == QuestStatusType.UnActivated)
                SetActivate();
            else if (Status == QuestStatusType.Activated)
                SetStart();
        }

        private void OnStartSpeak(SpeakVillageCharacterStartEvent eventData)
        {
            if (!CheckRequirements())
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            switch (trigger.Key)
            {
                case QuestTriggerExecuter.None:
                    break;
                case QuestTriggerExecuter.Character:
                    if (trigger.Value == eventData.Type.ToString())
                    {
                        if (Status == QuestStatusType.UnActivated)
                            SetActivate();
                        else if (Status == QuestStatusType.Activated)
                            SetStart();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void OnSectionPass(MineSceneSectionPassedEvent eventData)
        {
            if (!CheckRequirements(section: eventData.Section))
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            if (trigger.Key == QuestTriggerExecuter.Monster)
            {
                var monsterSection = eventData.Section as MineSceneMonsterSection;
                if (monsterSection != null)
                {
                    if (trigger.Value != monsterSection.ItemId)
                        return;
                }
            }

            if (Status == QuestStatusType.UnActivated)
                SetActivate();
            else if (Status == QuestStatusType.Activated)
                SetStart();
        }

        private void OnSectionExit(MineSceneSectionExitEvent eventData)
        {
            if (!CheckRequirements(section: eventData.Section))
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            if (trigger.Key == QuestTriggerExecuter.Monster)
            {
                var monsterSection = eventData.Section as MineSceneMonsterSection;
                if (monsterSection != null)
                {
                    if (trigger.Value != monsterSection.ItemId)
                        return;
                }
                else return;
            }

            if (Status == QuestStatusType.UnActivated)
                SetActivate();
            else if (Status == QuestStatusType.Activated)
                SetStart();
        }


        private void OnSectionAppear(MineSceneSectionAppearEvent eventData)
        {
            if (!CheckRequirements(section: eventData.Section))
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            if (trigger.Key == QuestTriggerExecuter.Monster)
            {
                var monsterSection = eventData.Section as MineSceneMonsterSection;
                if (monsterSection != null)
                {
                    if (trigger.Value != monsterSection.ItemId)
                        return;
                }
                else return;
            }

            if (Status == QuestStatusType.UnActivated)
                SetActivate();
            else if (Status == QuestStatusType.Activated)
                SetStart();
        }

        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            if (!CheckRequirements(section: eventData.Section))
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            if (trigger.Key == QuestTriggerExecuter.Monster)
            {
                var monsterSection = eventData.Section as MineSceneMonsterSection;
                if (monsterSection != null)
                {
                    if (trigger.Value != monsterSection.ItemId)
                        return;
                }
                else return;
            }

            if (Status == QuestStatusType.UnActivated)
                SetActivate();
            else if (Status == QuestStatusType.Activated)
                SetStart();
        }

        public void Deactivate()
        {
            UnSubscribeTriggers();

            foreach (var questTask in TaskList)
            {
                //if(questTask.Initialized)
                //    continue;

                questTask.Deactivate();
            }
        }

        public void Remove()
        {
            Deactivate();

            var staticData = App.Instance.StaticData;

            var staticQuest = staticData.Quests.Find(x => x.Id == StaticQuest.Id);

            if (staticQuest == null)
                return ;

            App.Instance.Player.Quests.QuestList.Remove(this);

        }

        public bool AfterInitializeCheck()
        {
            switch (Status)
            {
                case QuestStatusType.UnActivated:
                    if (StaticQuest.ActivateTrigger.Key == QuestTriggerExecuter.Item)
                    {
                        if (App.Instance.Player.Inventory.GetExistAmount(StaticQuest.ActivateTrigger.Value) > 0)
                        {
                            SetActivate();
                            return true;
                        }
                    }
                    break;
                case QuestStatusType.Activated:
                    break;
                case QuestStatusType.Started:
                    break;
                case QuestStatusType.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        return false;

    }

        public void Initialize()
        {
            if (Status == QuestStatusType.Completed)
                return;


            UnSubscribeTriggers();

            switch (Status)
            {
                case QuestStatusType.UnActivated:
                    SubscribeTriggers();

                    return;
                case QuestStatusType.Activated:
                    if (StaticQuest.StartTrigger.Key == QuestTriggerExecuter.None)
                    {
                        SetStart();
                    }
                    else
                    {
                        SubscribeTriggers();
                    }
                    break;
                case QuestStatusType.Started:
                    foreach (var questTask in TaskList)
                    {
                        if (!questTask.IsOpen)
                            continue;

                       // if (questTask.Initialized)
                       //    continue;

                        questTask.Initialize();
                    }
                    break;
                case QuestStatusType.Completed:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

       
        public void Save()
        {
            EventManager.Instance.Publish(new QuestUpdateEvent(this));

            App.Instance.Player.Save();

            OnUpdate?.Invoke();
        }

        public void SetTracking(bool enable)
        {
            IsTracking = enable;

            Save();

            EventManager.Instance.Publish(new QuestUpdateTrackingEvent(this));
        }

        public void SetActivate()
        {
            if (Status >= QuestStatusType.Activated)
                return;

            if (SceneManager.Instance.CurrentScene == ScenesNames.EntryPoint)
                return;

          //  Debug.Log(StaticQuest.ActivateTriggerType +":" + StaticQuest.ActivateTrigger.Key +":" + StaticQuest.ActivateTrigger.Value);
            switch (StaticQuest.ActivateTriggerEnd.Key)
            {
                case QuestInteractionMethodType.DiscoverWindow:
                    WindowManager.Instance.Show<WindowNewDiscover>(withPause: true)
                        .Initialize(StaticQuest.Id, StaticQuest.ActivateTriggerEnd.Value,
                            () =>
                            {
                                Status = QuestStatusType.Activated;
                                Save();

                               // Initialize();
                                EventManager.Instance.Publish(new QuestActivateEvent(this));
                            });

                    break;

                case QuestInteractionMethodType.DialogueWindow:
                    var dialogue =
                        App.Instance.StaticData.Dialogues.Find(x => x.Id == StaticQuest.ActivateTriggerEnd.Value);

                    if (dialogue == null)
                    {
                        App.Instance.Services.LogService.LogError($"No dialog {StaticQuest.Id} : {StaticQuest.ActivateTriggerEnd.Value}");
                        Status = QuestStatusType.Activated;
                        Save();

                        //   Initialize();
                        EventManager.Instance.Publish(new QuestActivateEvent(this));
                        return;
                    }

                    WindowManager.Instance.Show<WindowDialogue>(withPause: true)
                        .Initialize(dialogue, () =>
                        {
                            Status = QuestStatusType.Activated;
                            Save();

                           // Initialize();
                            EventManager.Instance.Publish(new QuestActivateEvent(this));
                        });

                    break;
                default:
                    Status = QuestStatusType.Activated;
                    Save();

                  //  Initialize();
                    EventManager.Instance.Publish(new QuestActivateEvent(this));
                    break;
            }

            App.Instance.Services.AnalyticsService.ActivateQuest(StaticQuest.Id);
        }

        public void SetStart()
        {
            if (Status >= QuestStatusType.Started)
                return;

            if (SceneManager.Instance.CurrentScene == ScenesNames.EntryPoint)
                return;

            switch (StaticQuest.StartTriggerEnd.Key)
            {
                case QuestInteractionMethodType.DiscoverWindow:
                    WindowManager.Instance.Show<WindowNewDiscover>(withPause: true)
                        .Initialize(StaticQuest.Id, StaticQuest.StartTriggerEnd.Value, () =>
                        {
                            Status = QuestStatusType.Started;
                            Save();

                            //Initialize();
                            EventManager.Instance.Publish(new QuestStartEvent(this));
                        });
                    break;

                case QuestInteractionMethodType.DialogueWindow:
                    var dialogue =
                        App.Instance.StaticData.Dialogues.Find(x => x.Id == StaticQuest.StartTriggerEnd.Value);

                    if (dialogue == null)
                    {
                        App.Instance.Services.LogService.LogError($"No dialog {StaticQuest.Id} : {StaticQuest.StartTriggerEnd.Value}");
                        Status = QuestStatusType.Started;
                        Save();

                        //   Initialize();
                        EventManager.Instance.Publish(new QuestStartEvent(this));
                        return;
                    }

                    WindowManager.Instance.Show<WindowDialogue>(withPause: true)
                        .Initialize(dialogue, () =>
                        {
                            Status = QuestStatusType.Started;
                            Save();

                            //   Initialize();
                            EventManager.Instance.Publish(new QuestStartEvent(this));
                        });

                    break;
                default:

                    Status = QuestStatusType.Started;
                    Save();

                  //  Initialize();
                    EventManager.Instance.Publish(new QuestStartEvent(this));
                    break;
            }

            App.Instance.Services.AnalyticsService.StartQuest(StaticQuest.Id);

        }

        public void SetReset()
        {
            Status = QuestStatusType.UnActivated;
        }

        public void SetComplete()
        {
            if (Status >= QuestStatusType.Completed)
                return;

            if (StaticQuest.RewardFeatures != null && StaticQuest.RewardFeatures.Count > 0)
            {
                foreach (var staticQuestRewardFeature in StaticQuest.RewardFeatures)
                {
                    if (!App.Instance.Player.Features.FeaturesList.Contains(staticQuestRewardFeature))
                        App.Instance.Player.Features.FeaturesList.Add(staticQuestRewardFeature);
                }
            }


            //Deactivate();
            IsTracking = false;
            Status = QuestStatusType.Completed;
            Save();

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.MineComplete);


            QuestHelper.ExtractQuestReward(StaticQuest);

            Debug.Log(StaticQuest.Id);
            EventManager.Instance.Publish(new QuestCompleteEvent(this));


            switch (StaticQuest.EndMethod.Key)
            {
                case QuestInteractionMethodType.DiscoverWindow:
                    WindowManager.Instance.Show<WindowNewDiscover>(withPause: true)
                        .Initialize(StaticQuest.Id, StaticQuest.EndMethod.Value);
                    break;

                case QuestInteractionMethodType.DialogueWindow:
                    var dialogue =
                        App.Instance.StaticData.Dialogues.Find(x => x.Id == StaticQuest.EndMethod.Value);

                    if (dialogue == null)
                    {
                        App.Instance.Services.LogService.Log("No dialog for EndMethod" + StaticQuest.Id);
                        return;
                    }

                    WindowManager.Instance.Show<WindowDialogue>(withPause: true)
                        .Initialize(dialogue);

                    break;
                case QuestInteractionMethodType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            App.Instance.Services.AnalyticsService.CompleteQuest(StaticQuest.Id);
        }

        public void Speak(CharacterType type)
        {
            var triggerType = Status == QuestStatusType.Activated
                ? StaticQuest.StartTriggerType
                : StaticQuest.ActivateTriggerType;

            if (triggerType != QuestTriggerType.Speak)
                return;

            if (!CheckRequirements())
                return;

            var trigger = Status == QuestStatusType.UnActivated
                ? StaticQuest.ActivateTrigger
                : StaticQuest.StartTrigger;

            switch (trigger.Key)
            {
                case QuestTriggerExecuter.None:
                    break;
                case QuestTriggerExecuter.Character:
                    if (trigger.Value == type.ToString())
                    {
                        if (Status == QuestStatusType.UnActivated)
                            SetActivate();
                        else if (Status == QuestStatusType.Activated)
                            SetStart();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SubscribeUpdate(Action method)
        {
            OnUpdate += method;
        }

        public void UnSubscribeUpdate(Action method)
        {
            OnUpdate -= method;
        }
    }
}