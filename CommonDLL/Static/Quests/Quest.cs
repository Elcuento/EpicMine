using System.Collections.Generic;

namespace CommonDLL.Static
{
    public class Quest
    {
        public string Id ;

        public string Owner ;

        public QuestType Type;

        public QuestTriggerType ActivateTriggerType;

        public Dictionary<QuestRequirementsType, string> ActivateTriggerRequirement ;

        public KeyValuePair<QuestInteractionMethodType, string> ActivateTriggerEnd ;

        public KeyValuePair<QuestTriggerExecuter, string> ActivateTrigger ;

        public QuestTriggerType StartTriggerType ;

        public Dictionary<QuestRequirementsType, string> StartTriggerRequirement ;

        public KeyValuePair<QuestTriggerExecuter, string> StartTrigger ;

        public KeyValuePair<QuestInteractionMethodType, string> StartTriggerEnd ;

        public List<string> StartDescription ;

        public List<string> EndDescription ;

        public Dictionary<string, int> RewardItems ;

        public List<FeaturesType> RewardFeatures ;

        public Dictionary<CurrencyType, int> RewardCurrency ;

        public List<string> Tasks ;

        public int Priority ;

        public string Filter ;

        public KeyValuePair<QuestInteractionMethodType, string> EndMethod ;

        public string Typ;

    }
}