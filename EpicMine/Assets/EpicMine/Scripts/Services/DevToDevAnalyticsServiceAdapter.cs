using System.Collections.Generic;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    /*  public class DevToDevAnalyticsServiceAdapter : IAnalyticsService
    {
      
        private const string AndroidAppId = "f88bbf93-95b9-090c-8d63-28e62c1b47c2";

        private const string IosAppId = "ac4f9b6b-1600-0acd-b814-838c68cc753a";

        private const string AndroidAppSecret = "6F9130Ksz4EJ8IMjpgZXlmBCoYyfdDe5";

        private const string IosAppSecret = "BnzSbCPGy78qAeou0pO3Nmv96gYUFZxD";

            
        public DevToDevAnalyticsServiceAdapter()
        {
            /*   DevToDev.Analytics.ApplicationVersion = Application.version;


   #if UNITY_ANDROID
               var appId = AndroidAppId;
               var appSecret = AndroidAppSecret;
   #elif UNITY_IOS
               var appId = IosAppId;
               var appSecret = IosAppSecret;
   #else
               var appId = string.Empty;
               var appSecret = string.Empty;
   #endif
               DevToDev.Analytics.SetActiveLog(Debug.isDebugBuild);
               DevToDev.Analytics.Initialize(appId, appSecret);

            GameAnalytics.Initialize();
        }


        public void SetUserId(string id)
        {
            GameAnalytics.SetCustomId(id);
        }

        public void SetUserNickname(string nickname)
        {
           // DevToDev.Analytics.ActiveUser.Name = nickname;
        }

        public void UserLevelUp(int level)
        {
            GameAnalytics.NewDesignEvent("level_up_" + level);
        }

        public void StartTutorial()
        {
            GameAnalytics.NewDesignEvent("tutorial_start");
        }

        public void SetTutorialStepComplete(int id)
        {
            GameAnalytics.NewDesignEvent("tutorial_completed_" + id);
        }

        public void CurrencyChange(int was, int become, IncomeSourceType source, string annotations = "")
        {
          /*  var eventName = $"currency_change";
            var parameters = new CustomEventParameters
            {
                Int = new Dictionary<string, int>()
                {
                    {"Was", was },
                    {"Become", become },
                },
                String = new Dictionary<string, string>()
                {
                    {"Source", source.ToString() },
                    {"Annotations", annotations },
                }
            };

            CustomEvent(eventName, parameters);
        }

        public void SetCustomData(string key, object value)
        {
           // DevToDev.Analytics.ActiveUser.SetUserData(key, value);
        }

        public void CreatePickaxe(string id, PickaxeType type)
        {
            GameAnalytics.NewDesignEvent("pickaxe_create_" + id);
        }

        public void CreateTorch(string id, TorchType type)
        {
            GameAnalytics.NewDesignEvent("torch_create_" + id);
        }

        public void CompleteTutorial()
        {
            GameAnalytics.NewDesignEvent("tutorial_completed");
        }

        public void CompleteQuest(string questName)
        {
            GameAnalytics.NewDesignEvent("quest_completed_" + questName);
        }

        public void StartQuest(string questName)
        {
            GameAnalytics.NewDesignEvent("quest_start_" + questName);
        }


        public void ActivateQuest(string questName)
        {
            GameAnalytics.NewDesignEvent("quest_activate" + questName);
        }

        public void CompleteTask(string taskName)
        {
            GameAnalytics.NewDesignEvent("quest_task_completed_" + taskName);
        }

        public void InAppPurchase(string purchaseId, string purchaseType, long purchaseAmount, long purchasePrice,
            string purchaseCurrency)
        {
            GameAnalytics.NewDesignEvent("shop_pack_buy_" + purchaseId);
        }

        public void RealPayment(string paymentId, float inAppPrice, string inAppName, string inAppCurrencyIsoCode)
        {
            Debug.Log(paymentId+ ":" + inAppPrice + ":" + inAppCurrencyIsoCode);

            return;
            GameAnalytics.NewBusinessEvent(inAppCurrencyIsoCode, (int)inAppPrice, paymentId, paymentId, "");
        }

        public void CurrencyAccrual(int amount, string name, CurrencyAccrualType accrualType)
        {
           /* var devToDevAccrualType = accrualType == CurrencyAccrualType.Earned
                ? DevToDev.AccrualType.Earned
                : DevToDev.AccrualType.Purchased;

            DevToDev.Analytics.CurrencyAccrual(amount, name, devToDevAccrualType);
        }

        public void CustomEvent(string eventName, CustomEventParameters parameters)
        {
          /*  var devToDevParams = new DevToDev.CustomEventParams();

            if (parameters.Int != null)
            {
                foreach (var parameter in parameters.Int)
                    devToDevParams.AddParam(parameter.Key, parameter.Value);
            }

            if (parameters.String != null)
            {
                foreach (var parameter in parameters.String)
                    devToDevParams.AddParam(parameter.Key, parameter.Value);
            }

            if (parameters.Float != null)
            {
                foreach (var parameter in parameters.Float)
                    devToDevParams.AddParam(parameter.Key, parameter.Value);
            }

            DevToDev.Analytics.CustomEvent(eventName, devToDevParams);
        }


        public void StartMining(int tierNumber, int mineNumber, bool isAlreadyCompleted = false)
        {
           GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,tierNumber + "_" + mineNumber + "_" + isAlreadyCompleted);
        }

        public void EndMining(int tierNumber, int mineNumber, bool isComplete,
            Dictionary<string, int> earned, Dictionary<string, int> spent, bool isAlreadyCompleted = false)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, tierNumber + "_" + mineNumber + "_" + isAlreadyCompleted);
        }

        public void EndPvpMatch(int tierNumber, int prestige, int rating, int timeSpend, bool withBot,
            PvpArenaGameResoultType result, int wallCrashed)
        {
           GameAnalytics.NewDesignEvent("pvp_match_start_" + tierNumber + "_" + prestige + "_" + result);

        }

    }*/
}