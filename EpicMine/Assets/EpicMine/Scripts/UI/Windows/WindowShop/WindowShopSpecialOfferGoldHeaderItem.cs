using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

public class WindowShopSpecialOfferGoldHeaderItem : MonoBehaviour {

    [SerializeField] public List<WindowShopSpecialOfferHeaderResourceItem> _itemList;
    [SerializeField] public GameObject _enableButton;
    [SerializeField] public GameObject _disableButton;
    [SerializeField] public TextMeshProUGUI _timer;
    [SerializeField] public TextMeshProUGUI _description;
    [SerializeField] public TextMeshProUGUI _header;

    private ShopTriggerBuyGold _trigger;
    private ShopPack _pack;

    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        EventManager.Instance.Unsubscribe<ShopTriggerCompleteEvent>(OnShopTriggerComplete);
        EventManager.Instance.Unsubscribe<ShopTriggerStartEvent>(OnShopTriggerStart);
    }

    public void Start()
    {
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuyShopPack);
        EventManager.Instance.Subscribe<ShopTriggerCompleteEvent>(OnShopTriggerComplete);
        EventManager.Instance.Subscribe<ShopTriggerStartEvent>(OnShopTriggerStart);
    }

    public void OnShopTriggerComplete(ShopTriggerCompleteEvent eventData)
    {
        if (eventData.ShopTrigger.ShopPackId != _pack.Id)
            return;

        _enableButton.gameObject.SetActive(false);
        _disableButton.gameObject.SetActive(true);

        Initialize();
    }

    public void OnShopTriggerStart(ShopTriggerStartEvent eventData)
    {
        if (eventData.ShopTrigger.ShopPackId != _pack.Id)
            return;

        Initialize();
    }

    public void OnBuyShopPack(ShopBuyShopPackEvent eventData)
    {
        if (eventData.ShopPack.Type == ShopPackType.Gold)
        {
            CorrectTarget();
        } 
    }

    public void OnTickEvent(SecondsTickEvent data)
    {
        if (_trigger == null)
        {
            _enableButton.gameObject.SetActive(false);
            _disableButton.gameObject.SetActive(true);
            _timer.text = "";
            return;
        }

        if (!_trigger.IsCompleted)
        {
            _enableButton.gameObject.SetActive(false);
            _disableButton.gameObject.SetActive(true);
            _timer.text = "";
        }
        else
        {
            _enableButton.gameObject.SetActive(true);
            _disableButton.gameObject.SetActive(false);
        }

        var date = new DateTime();
  
        try
        {
            date = date.AddSeconds(_trigger.TimeLeft);
            _timer.text = TimeHelper.Format(date, dayAnnotation: true);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    public void OnClickBuy()
    {
        if (_trigger.IsCompleted)
        {
            App.Instance.Player.Shop.BuyShopPack(_pack);
        }
    }


    public void CorrectTarget()
    {
        if (_trigger != null && _pack != null)
        {
            var restGold = _trigger.RequireGoldSpent - _trigger.CurrentGoldSpent;
            _description.text = string.Format(LocalizationHelper.GetLocale("window_shop_gold_header_description"), restGold <= 0 ? 0 : restGold);
        }
    }

    public void Clear()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
    }

    public void Initialize()
    {
        Clear();

        _trigger = App.Instance.Controllers.ShopController.ShopTriggers
            .Find(x => x.ShopPackId == ShopLocalConfig.GoldHeaderShopPack) as ShopTriggerBuyGold;

        _pack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == ShopLocalConfig.GoldHeaderShopPack);

        if (_trigger == null)
        {
            App.Instance.Services.LogService.LogError("Gold collect trigger not exist");
            gameObject.SetActive(false);
            return;
        }

        _enableButton.gameObject.SetActive(_trigger != null && _trigger.IsCompleted);
        _disableButton.gameObject.SetActive(_trigger == null || !_trigger.IsCompleted);

        var restGold = _trigger.RequireGoldSpent - _trigger.CurrentGoldSpent;
        _description.text = string.Format(LocalizationHelper.GetLocale("window_shop_gold_header_description"), restGold <= 0 ? 0 : restGold);
        
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickEvent);


        var items = _pack.Items.ToList();

        for (var i = 0; i < _itemList.Count; i++)
        {
            if (i < items.Count)
            {
                _itemList[i].Initialize(items[i].Key, items[i].Value);
            }
            else
            {
                _itemList[i].gameObject.SetActive(false);
            }
        }

        OnTickEvent(new SecondsTickEvent());
    }
}
