using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Buff = BlackTemple.EpicMine.Core.Buff;

public class VillageSceneLeftButtonsController : MonoBehaviour
{

    [Header("Offer")]
    [SerializeField] private GameObject _offerObject;
    [SerializeField] private TextMeshProUGUI _offerAmount;
    [SerializeField] private TextMeshProUGUI _offerTime;


    [Header("Crystals")]
    [SerializeField] private GameObject _crystalObject;
    [SerializeField] private Image  _crystalPicture;
    [SerializeField] private TextMeshProUGUI _crystalTime;
    [SerializeField] private Material _blackWhiteMat;

    private ShopOffer _offer;
    private Buff _crystalBuff;

    public void Awake()
    {
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickEvent);
    }

    public void Start()
    {
        RefreshOffer();
        RefreshCrystals();

        EventManager.Instance.Subscribe<ShopSpecialOfferEvent>(OnAddOffer);
        EventManager.Instance.Subscribe<EffectAddBuffEvent>(OnAddBuffEffect);
        EventManager.Instance.Subscribe<TutorialStepCompleteEvent>(OnTutorialCompleteStep);

        OnTickEvent(new SecondsTickEvent());
    }

   

    public void OnTutorialCompleteStep(TutorialStepCompleteEvent step)
    {
        if (step.Step.Id >= TutorialStepIds.EnceladAppear)
        {
            GetStoredOffers();
        }
    }


    public void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
            EventManager.Instance.Unsubscribe<ShopSpecialOfferEvent>(OnAddOffer);
            EventManager.Instance.Unsubscribe<EffectAddBuffEvent>(OnAddBuffEffect);
            EventManager.Instance.Unsubscribe<TutorialStepCompleteEvent>(OnTutorialCompleteStep);
        }
    }

    public void GetStoredOffers()
    {

        if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.EnceladAppear))
            return;

        if (!App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.ShopOfferNotifications))
            return;

        var shopOffers = App.Instance.Services.RuntimeStorage.
                Load<List<CommonDLL.Dto.ShopOffer>>(RuntimeStorageKeys.ShopOfferNotifications);

        if (!InAppPurchaseManager.Instance.IsInitialized)
            return;

        WindowManager.Instance.Show<WindowShopOffer>()
            .Initialize(shopOffers);
    }

    public void OnTickEvent(SecondsTickEvent data)
    {
        if (_offer == null || _offer.IsCompleted || !_offer.IsActive)
        {
            if(_offerObject.activeSelf)
            OfferTimerOff();
        }
        else
        {
            var offerDate = new DateTime();
            offerDate = offerDate.AddSeconds(_offer.TimeLeft);
            _offerTime.text = offerDate.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        if (_crystalBuff == null || !_crystalBuff.IsActive)
        {
            if (_crystalObject.activeSelf)
                CrystalTimerOff();
        }
        else
        {
            if (_crystalBuff.IsCheckTime)
            {
                _crystalTime.text = "Забрать 25 Кристаллов!";
                return;
            }
            var crystalDate = new DateTime();
            crystalDate = crystalDate.AddSeconds(_crystalBuff.TimeLeftToCheck);
            _crystalTime.text = crystalDate.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            _crystalPicture.material = _blackWhiteMat;
            _crystalPicture.color  = new Color(1,1,1,0.5f);
        }
    }

    public void TimerOff()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
    }


    public void OfferTimerOff()
    {
        _offerObject.SetActive(false);

        if (!_crystalObject.activeSelf)
        {
            TimerOff();
        }
    }

    public void CrystalTimerOff()
    {
        _crystalObject.SetActive(false);

        if (!_offerObject.activeSelf)
        {
            TimerOff();
        }
    }


    public void TimerStart()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickEvent);
    }


    public void OnOfferClick()
    {
        if (_offer != null)
        {
            WindowManager.Instance.Show<WindowShop>()
                .OpenOffer();
        }
    }

    public void OnClickCrystals()
    {
        if (_crystalBuff == null)
            return;

        if (_crystalBuff.IsCheckTime)
        {
            App.Instance.Player.Effect.CheckBuff(_crystalBuff.Id, () =>
            {
                OnTickEvent(new SecondsTickEvent());
            });
        }
        else
        {
            WindowManager.Instance.Show<WindowCrystalSubscription>()
                .Initialize(_crystalBuff);
        }
    }

    public void RefreshCrystals()
    {
        _crystalPicture.color = new Color(1, 1, 1, 1);
        _crystalPicture.material = null;

        _crystalBuff = App.Instance.Player.Effect.GetBuff(BuffType.Currency);

        _crystalObject.SetActive(_crystalBuff != null && 
                                 _crystalBuff.IsActive);

        if(_crystalBuff!=null && _crystalBuff.IsActive)
            TimerStart();
    }

    public void RefreshOffer()
    {
        var offer = App.Instance.Player.Shop.ShopOffer.Where(x => x.IsActive && !x.IsCompleted)
            .OrderByDescending(x => x.TimeLeft).ToList();

        if (offer.Count > 0)
        {
            _offer = offer[0];
            _offerAmount.text = offer.Count.ToString();
        }
        _offerObject.SetActive(offer.Count > 0);

        if (_offer!=null)
            TimerStart();
    }


    public void OnAddOffer(ShopSpecialOfferEvent data)
    {
        RefreshOffer();
        GetStoredOffers();
    }

    public void OnAddBuffEffect(EffectAddBuffEvent data)
    {
        RefreshCrystals();
    }
}
