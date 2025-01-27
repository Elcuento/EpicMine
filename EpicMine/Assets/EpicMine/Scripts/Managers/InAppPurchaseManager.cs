using System;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;


namespace BlackTemple.EpicMine
{
    public class InAppPurchaseManager : Singleton<InAppPurchaseManager>, IStoreListener
    {
        public bool IsInitialized => StoreController != null && _storeExtensionProvider != null;

        public IStoreController StoreController { get; private set; }

        private IExtensionProvider _storeExtensionProvider;

        public void Initialize(List<Iap> configs)
        {
            if (IsInitialized)
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());


            foreach (var iap in configs)
            {
                var productIds = new IDs {{iap.Id, AppleAppStore.Name, GooglePlay.Name}};
           
                if (iap.Platform == PlatformType.All || iap.Platform == App.Instance.CurrentPlatform || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    builder.AddProduct(iap.Id, (ProductType)iap.Type, productIds);
                }
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void Buy(string productId)
        {
            if (!IsInitialized)
                return;

            WindowManager.Instance.Show<WindowPreloader>(withSound: false);

            if (!NetworkManager.Instance.IsInternetAvailable)
            {
                OnPurchaseError(productId);
                return;
            }
            
            try
            {
                var product = StoreController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {

                    StoreController.InitiatePurchase(product);
                }
                else
                    OnPurchaseError(productId);
            }
            catch (Exception e)
            {
                App.Instance.Services.LogService.LogError($"Inapp buy error: { e }");
                OnPurchaseError(productId);
            }
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            StoreController = controller;
            _storeExtensionProvider = extensions;

            EventManager.Instance.Publish(new InitializedIapEvent());
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("initialized failed " + error);
            App.Instance.Services.LogService.LogError($"Inapp intialization error: { error }");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError("initialized failed " + error  +":" + message);
        }

        public bool ValidateReceipt(string receipt)
        {
            App.Instance.Services.LogService.LogError($"Try validate");
            
            var isValid = true;

/*#if (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR
            var validator =
 new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
App.Instance.Services.LogService.LogError($"validate process");
                validator.Validate(receipt);
            }
            catch (IAPSecurityException e)
            {
App.Instance.Services.LogService.LogError($"validate exaption " + e);
                isValid = false;
            }
              
#endif*/


            App.Instance.Services.LogService.LogError($"validate result : {isValid}");
          
            return isValid;
        }


        /*
              1. Purchase in store
              2. Purchase in server/save to list that purchase has completed
              3. Validate purchase check if server has that purchase or not. if not => 2.
           
              On start app get purchase list from store and from server and validate purchase that not in if its not in completed list

            server arch

            usersPurchase/id/uniqueId
            
            
         */
         

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs response)
        {
            App.Instance.Services.LogService.Log($"Inapp purchase process: { response.purchasedProduct.definition.id }");
            var isValid = true;

            App.Instance.Services.LogService.Log($"Purchase receipt: { response.purchasedProduct.receipt }");

#if (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR
       /*     var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                validator.Validate(response.purchasedProduct.receipt);
            }
            catch (IAPSecurityException)
            {
                isValid = false;
            }*/
#endif

            App.Instance.Services.LogService.Log($"Purchase valid: { isValid }");

            if (isValid)
            {
                WindowManager.Instance.Close<WindowPreloader>(withSound: false);
                EventManager.Instance.Publish(new IapPurchaseCompleteEvent(response.purchasedProduct));

                var transactionId = response.purchasedProduct != null
                    ? response.purchasedProduct.transactionID
                    : string.Empty;

                var localizedPrice = (float) (response.purchasedProduct?.metadata.localizedPrice ?? 0m);

                var isoCurrencyCode = response.purchasedProduct != null
                    ? response.purchasedProduct.metadata.isoCurrencyCode
                    : string.Empty;

                var id = response.purchasedProduct != null
                    ? response.purchasedProduct.definition.id
                    : string.Empty;

                App.Instance.Services.AnalyticsService
                    .RealPayment(
                        transactionId,
                        localizedPrice,
                        id,
                        isoCurrencyCode);

                OnProcessPurchase(response);

                /* if (App.Instance.Services.TenjinServiceAdapter != null)
                 {
                     try
                     {
                         OnProcessPurchase(response);
                     }
                     catch (Exception e)
                     {
                         Debug.LogError(e);
                     }
                 }*/

            }
            else
                OnPurchaseError(response.purchasedProduct.definition.id);
            
            return PurchaseProcessingResult.Complete;
        }

        public void OnProcessPurchase(PurchaseEventArgs purchaseEventArgs)
        {
            var price = purchaseEventArgs.purchasedProduct.metadata.localizedPrice;
            double lPrice = decimal.ToDouble(price);
            var currencyCode = purchaseEventArgs.purchasedProduct.metadata.isoCurrencyCode;

            Dictionary<string, object> wrapper;

            try
            {
                wrapper = purchaseEventArgs.purchasedProduct.receipt.FromJson<Dictionary<string, object>>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                wrapper = null;
            }
            //var wrapper =  // https://gist.github.com/darktable/1411710
            if (null == wrapper)
            {
                return;
            }

            var payload = (string)wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
            var productId = purchaseEventArgs.purchasedProduct.definition.id;

#if UNITY_ANDROID

            Dictionary<string, object> gpDetails;

            try
            {
                gpDetails = payload.FromJson<Dictionary<string, object>>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            var gpJson = (string)gpDetails["json"];
            var gpSig = (string)gpDetails["signature"];

            CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);

#elif UNITY_IOS

  var transactionId = purchaseEventArgs.purchasedProduct.transactionID;

  CompletedIosPurchase(productId, currencyCode, 1, lPrice , transactionId, payload);

#endif

        }

        private void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
        {
         /*   var instance = Tenjin.getInstance(App.Instance.Services.TenjinServiceAdapter.ApiKey);
            instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);*/
        }

        private void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
        {
         /*   var instance = Tenjin.getInstance(App.Instance.Services.TenjinServiceAdapter.ApiKey);
            instance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);*/
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            OnPurchaseError(product.definition.id);
        }


        private void OnPurchaseError(string productId)
        {
            WindowManager.Instance.Close<WindowPreloader>(withSound: false);
            EventManager.Instance.Publish(new IapPurchaseErrorEvent(productId));
        }


        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                    {
                        return true;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }
            return false;
        }


        public SubscriptionInfo CheckSubscription(Product item)
        {
            Dictionary<string, string> introductory_info_dict = null;

#if UNITY_ANDROID
            var subscriptionManager = new SubscriptionManager(item, null);
            return subscriptionManager.getSubscriptionInfo();
#elif UNITY_IOS || UNITY_IPHONE         
            var subscriptionManager = _storeExtensionProvider.GetExtension<IAppleExtensions>();
            introductory_info_dict = subscriptionManager.GetIntroductoryPriceDictionary();
#endif

            if (introductory_info_dict == null)
              return null;

            if (item.receipt != null)
            {
                    if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                    {
                        string intro_json =
                            (introductory_info_dict == null ||
                             !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
                                ? null
                                : introductory_info_dict[item.definition.storeSpecificId];
                        var p = new SubscriptionManager(item, intro_json);
                        var info = p.getSubscriptionInfo();
                         return info;
                    }
                    else
                    {
                        Debug.Log("Cant check it ");
                    }
            }

            return null;
        }
    }
}