using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class EmptyAnalyticsService : IAnalyticsService
    {
        public void SetUserId(string id) { }

        public void SetUserNickname(string nickname) { }

        public void UserLevelUp(int level) { }

        public void StartTutorial() { }

        public void SetTutorialStepComplete(int id) { }

        public void CompleteTutorial() { }

        public void CompleteQuest(string questName)  { }

        public void StartQuest(string questName) { }

        public void ActivateQuest(string staticQuestId) { }

        public void CompleteTask(string questName) { }

        public void InAppPurchase(string purchaseId, string purchaseType, long purchaseAmount, long purchasePrice, string purchaseCurrency) { }

        public void RealPayment(string paymentId, float inAppPrice, string inAppName, string inAppCurrencyIsoCode) { }

        public void CurrencyAccrual(int amount, string name, CurrencyAccrualType accrualType) { }

        public void CustomEvent(string eventName, CustomEventParameters parameters) { }

        public void StartMining(int tierNumber, int mineNumber, bool isAlreadyCompleted = false) { }

        public void EndMining(int tierNumber, int mineNumber, bool isComplete, Dictionary<string, int> earned, Dictionary<string, int> spent, bool isAlreadyCompleted = false) { }

        public void EndPvpMatch(int tierNumber, int prestige, int rating, int timeSpend, bool withBot, PvpArenaGameResoultType result,int wallCrashed) {}

        public void CurrencyChange(int was, int become, IncomeSourceType source, string annotations = "") { }

        public void SetCustomData(string key, object value) { }

        public void CreatePickaxe(string id, PickaxeType type) { }

        public void CreateTorch(string id, TorchType type) { }
    }
}