using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Buff = BlackTemple.EpicMine.Core.Buff;

public class WindowShopSpecialOfferCrystalsHeaderItem : MonoBehaviour
{

    public const string MonthShopPack = "shop_pack_stock_crystals0";

    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _totalGet;
    [SerializeField] private TextMeshProUGUI _instantGet;
    [SerializeField] private TextMeshProUGUI _everyDay;

    [Header("Buy/Get Button Label")]
    [SerializeField] private TextMeshProUGUI _butGetLabel;

    [Header("Timer ")]
    [SerializeField] private GameObject _timerContainer;
    [SerializeField] private TextMeshProUGUI _timerValue;

    private Buff _buff;
    private ShopPack _pack;
    private Product _product;


    private void Start()
    {
        EventManager.Instance.Subscribe<ShopBuyShopPackEvent>(OnBuy);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);
        EventManager.Instance.Unsubscribe<ShopBuyShopPackEvent>(OnBuy);
    }

    public void OnBuy(ShopBuyShopPackEvent eventData)
    {
        if (eventData.ShopPack == null)
            return;

        if (eventData.ShopPack.Id != _product.definition.id)
            return;

        _buff = App.Instance.Player.Effect.GetBuff(BuffType.Currency);

        if (_buff == null)
        {
            return;
        }

        TimerStart();
    }

    public void OnTickEvent(SecondsTickEvent data)
    {
        if (_buff == null || !_buff.IsActive)
        {
           TimerOff();
            return;
        }

        if (_buff.IsCheckTime)
        {
            TimerGet();
            return;
        }

        var date = new DateTime();
        date = date.AddSeconds(_buff.TimeLeftToCheck);
        _timerValue.text = TimeHelper.Format(date, true);

    }

    public void Initialize()
    {
        _pack = App.Instance.StaticData.ShopPacks.Find(x => x.Id == MonthShopPack);
        _product = ShopHelper.GetProductByPackId(_pack.Id);

        if (_product == null || _pack == null || _pack.Buffs.Count <= 0)
        {
            gameObject.SetActive(false);
            App.Instance.Services.LogService.LogError($"crystals purchase not exist");
            return;
        }

        var staticBuff = App.Instance.StaticData.Buffs.Find(x => x.Id == _pack.Buffs[0]);

        var everyDay = staticBuff.Value.FirstOrDefault(x => x.Key == BuffValueType.CrystalsByDay).Value;
        var monthDays = (DateTime.UtcNow.AddMonths(1) - DateTime.UtcNow).Days;
        var total = _pack.Currency.FirstOrDefault(x => x.Key == CurrencyType.Crystals)
                        .Value + monthDays * everyDay;

        var instant = _pack.Currency.FirstOrDefault(x => x.Key == CurrencyType.Crystals)
            .Value;

        _totalGet.text = total.ToString();
        _everyDay.text = everyDay.ToString();
        _instantGet.text = instant.ToString();


        _buff = App.Instance.Player.Effect.GetBuff(BuffType.Currency, BuffValueType.CrystalsByDay);

        if (_buff != null && _buff.IsActive)
        {
            if (_buff.IsCheckTime)
            {
                TimerGet();
            }
            else
            {
                TimerStart();
            }
        }
        else
        {
            TimerOff(true);
        }

    }

    public void TimerGet()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);

        _timerContainer.SetActive(false);
        _buyButton.image.sprite = App.Instance.ReferencesTables.Sprites.ButtonYellow;
        _butGetLabel.gameObject.SetActive(true);
      //  _butGetLabel.text = LocalizationHelper.GetLocale("get");
    }

    public void TimerStart()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);

        _timerContainer.SetActive(true);
        _buyButton.image.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
        _butGetLabel.gameObject.SetActive(false);

        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickEvent);

        OnTickEvent(new SecondsTickEvent());
    }

    public void TimerOff(bool initialize = false)
    {
        if (_product == null)
        {
            gameObject.SetActive(false);
            return;
        }
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickEvent);

     //   _butGetLabel.text = $"{_product.metadata.localizedPrice:0.##} {_product.metadata.isoCurrencyCode}";
        _timerContainer.SetActive(false);
        _buyButton.image.sprite = App.Instance.ReferencesTables.Sprites.ButtonYellow;
        _butGetLabel.gameObject.SetActive(true);

    }


    public void OnClick()
    {
        if (_buff == null || !_buff.IsActive)
        {
            WindowManager.Instance.Show<WindowCrystalSubscriptionOffer>()
                .Initialize();
        }
        else
        if (_buff != null && _buff.IsCheckTime)
        {
            App.Instance.Player.Effect.CheckBuff(_pack.Buffs[0], TimerStart);
        }       
    }
}
