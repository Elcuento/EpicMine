using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public interface IAnalyticsService
    {
        void SetUserId(string id);

        void SetUserNickname(string nickname);

        void UserLevelUp(int level);

        void StartTutorial();

        void SetTutorialStepComplete(int id);

        void CompleteTutorial();

        void CompleteQuest(string questName);

        void StartQuest(string questName);

        void ActivateQuest(string staticQuestId);

        void CompleteTask(string taskName);

        void InAppPurchase(string purchaseId, string purchaseType, long purchaseAmount, long purchasePrice, string purchaseCurrency);

        void RealPayment(string paymentId, float inAppPrice, string inAppName, string inAppCurrencyIsoCode);

        void CurrencyAccrual(int amount, string name, CurrencyAccrualType accrualType);

        void CustomEvent(string eventName, CustomEventParameters parameters);

        void StartMining(int tierNumber, int mineNumber, bool isAlreadyCompleted = false);

        void EndMining(int tierNumber, int mineNumber, bool isComplete, Dictionary<string, int> earned, Dictionary<string, int> spent, bool isAlreadyCompleted = false);

        void EndPvpMatch(int tierNumber, int prestige, int rating, int timeSpend, bool withBot,
            PvpArenaGameResoultType result, int wallCrashed);

        void CurrencyChange(int was, int become, IncomeSourceType source, string annotations = "");

        void SetCustomData(string key, object value);

        void CreatePickaxe(string id, PickaxeType type);

        void CreateTorch(string id, TorchType type);

    }

    public enum CurrencyAccrualType
    {
        Earned,
        Purchased
    }

    public struct CustomEventParameters
    {
        public Dictionary<string, int> Int;
        public Dictionary<string, string> String;
        public Dictionary<string, float> Float;
    }
}