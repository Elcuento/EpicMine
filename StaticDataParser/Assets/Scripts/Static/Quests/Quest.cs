using System;
using System.Collections.Generic;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine.Static
{
    public class Quest
    {
        public string Id { get; }

        public string Owner { get; }

        public QuestType Type;

        public QuestTriggerType ActivateTriggerType;

        public Dictionary<QuestRequirementsType, string> ActivateTriggerRequirement { get; }

        public KeyValuePair<QuestInteractionMethodType, string> ActivateTriggerEnd { get; }

        public KeyValuePair<QuestTriggerExecuter, string> ActivateTrigger { get; }

        public QuestTriggerType StartTriggerType { get; }

        public Dictionary<QuestRequirementsType, string> StartTriggerRequirement { get; }

        public KeyValuePair<QuestTriggerExecuter, string> StartTrigger { get; }

        public KeyValuePair<QuestInteractionMethodType, string> StartTriggerEnd { get; }

        public List<string> StartDescription { get; }

        public List<string> EndDescription { get; }

        public Dictionary<string, int> RewardItems { get; }

        public List<FeaturesType> RewardFeatures { get; }

        public Dictionary<CurrencyType, int> RewardCurrency { get; }

        public List<string> Tasks { get; }

        public int Priority { get; }

        public string Filter { get; }

        public KeyValuePair<QuestInteractionMethodType, string> EndMethod { get; }

        public string Typ;

        public Quest(string id, string owner, QuestType? type, string filter, int? priority, string tasks, string rewardItems,
            string rewardFeatures,
            string rewardCurrency, string startDescription, string endDescription, string endMethod,
            QuestTriggerType? activateTriggerType, string activateTriggerRequirement, string activateTrigger,
            string activateTriggerEnd,
            QuestTriggerType? startTriggerType, string startTriggerRequirement, string startTrigger,
            string startTriggerEnd,
            string typ)
        {
            Id = id;
            Owner = owner;
            Filter = filter;
            Type = type ?? QuestType.Simple;
            Priority = priority ?? 0;
            
            ActivateTriggerType = activateTriggerType ?? QuestTriggerType.None;
            ActivateTriggerRequirement = Extensions.GetDictionaryBySplitKeyValuePair<QuestRequirementsType, string>(activateTriggerRequirement, '#');
            ActivateTrigger = Extensions.GetStringSplitKeyPair<QuestTriggerExecuter, string>(activateTrigger, ':');
            ActivateTriggerEnd = Extensions.GetStringSplitKeyPair<QuestInteractionMethodType, string>(activateTriggerEnd, ':');

            StartTriggerType = startTriggerType ?? QuestTriggerType.None;
            StartTriggerRequirement = Extensions.GetDictionaryBySplitKeyValuePair<QuestRequirementsType, string>(startTriggerRequirement,'#');
            StartTrigger = Extensions.GetStringSplitKeyPair<QuestTriggerExecuter, string>(startTrigger, ':');
            StartTriggerEnd = Extensions.GetStringSplitKeyPair<QuestInteractionMethodType, string>(startTriggerEnd, ':');

            Tasks = Extensions.SplitToList<string>(tasks, "#");

            RewardItems = Extensions.GetDictionaryBySplitKeyValuePair<string, int>(rewardItems, '#');
            RewardFeatures = Extensions.SplitToList<FeaturesType>(rewardFeatures, "#");
            RewardCurrency = Extensions.GetDictionaryBySplitKeyValuePair<CurrencyType, int>(rewardCurrency, '#');

            StartDescription = Extensions.SplitToList<string>(startDescription, "#");
            EndDescription = Extensions.SplitToList<string>(endDescription, "#");

            EndMethod = Extensions.GetStringSplitKeyPair<QuestInteractionMethodType, string>(endMethod, ':');

            Typ = typ;
        }
    }
}