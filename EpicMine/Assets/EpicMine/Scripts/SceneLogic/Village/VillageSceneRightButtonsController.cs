using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

public class VillageSceneRightButtonsController : MonoBehaviour {

    [SerializeField] private GameObject _adCrystalContainer;
    [SerializeField] private TextMeshProUGUI _adCrystalCountLabel;

    private ShopTimerPurchase _adCrystal;
    private ShopPack _adCrystalPack;

    public void Start()
    {
        Initialize();

        EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialStepCompleted);
    }

    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialStepCompleted);
    }

    public void OnTutorialStepCompleted(TutorialStepCompleteEvent eventData)
    {
        if(eventData.Step.Id >= TutorialStepIds.EnceladAppear)
            Initialize();
    }

    public void Initialize()
    {
        if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.EnceladAppear))
        {
            _adCrystalContainer.SetActive(false);
            return;
        }

        _adCrystalPack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == ShopLocalConfig.AdCrystalRewardShopPack);
        _adCrystal = App.Instance.Player.Shop.TimePurchase.Find(x => x.Id == ShopLocalConfig.AdCrystalRewardShopPack);

        var crystalValue = 0;

        if (_adCrystalPack  != null)
        {
            var crystalReward = _adCrystalPack.Currency.FirstOrDefault(x => x.Key == CurrencyType.Crystals);
            crystalValue = crystalReward.Value;
            _adCrystalCountLabel.text = crystalValue.ToString();
        }
        
        _adCrystalContainer.SetActive(_adCrystalPack != null && crystalValue > 0 && (_adCrystal == null || _adCrystal.Charge > 0));
    }

    public void OnClickAdCrystal()
    {
        WindowManager.Instance.Show<WindowGetAdReward>()
            .Initialize(_adCrystalPack, _adCrystal, OnClose);
    }

    public void OnClose()
    {
        Initialize();
    }
}
