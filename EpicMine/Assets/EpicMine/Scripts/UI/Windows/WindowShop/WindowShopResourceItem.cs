using System;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Currency = BlackTemple.EpicMine.Dto.Currency;

public class WindowShopResourceItem : MonoBehaviour
{
    [SerializeField] private GameObject _open;
    [SerializeField] private GameObject _close;

    [SerializeField] private Button _button;
    [SerializeField] private Image _picture;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private TextMeshProUGUI _price;

    [SerializeField] private GameObject _lock;

    private ShopResources _shopPack;

    public void Initialize(ShopResources res, bool isLock)
    {
        _shopPack = res;

        _button.interactable = !isLock;

        _open.SetActive(!isLock);
        _close.SetActive(isLock);

        _price.text = isLock ? "" : res.CrystalCost.ToString();
        _picture.color = isLock ? new Color(0, 0, 0, 0.4f) : new Color(1, 1, 1, 1);

        _picture.sprite = SpriteHelper.GetIcon(res.Id);
        _count.text = res.MinCount.ToString();

    }

    public void OnClick()
    {
        WindowManager.Instance.Show<WindowBuyResourceAmount>()
            .Initialize(_shopPack, TryBuy);
    }

    public void TryBuy(ShopResources res, int amount, float sale)
    {
        long price = res.CrystalCost * amount;
        var cost = Math.Round(price - price * (sale > 0 ? sale : 0));

        if (App.Instance.Player.Wallet.Has(new Currency(CurrencyType.Crystals, (int)cost)))
        {

            var staticData = App.Instance.StaticData;

            var shopPack = staticData.ShopResources.Find(x => x.Id == res.Id);
            if (shopPack == null)
            {
                UnityEngine.Debug.LogError("Pack not exist");
                return;
            }

            if (sale > staticData.Configs.Shop.ResourceMaxSale)
            {
                if (amount >= 10)
                {
                    sale = (amount / 300f) * staticData.Configs.Shop.ResourceMaxSale;

                    if (sale > staticData.Configs.Shop.ResourceMaxSale)
                    {
                        sale = staticData.Configs.Shop.ResourceMaxSale;
                    }
                }
                else
                {
                    sale = 0;
                }
            }

            price = amount * shopPack.CrystalCost;

            if (sale > 0)
            {
                price = (long)(price - Math.Round(price * sale));
            }


            if (!App.Instance.Player.Wallet.SubsTractCurrency(CurrencyType.Crystals, price))
            {
                UnityEngine.Debug.LogError("No enough currency");
                return;
            }

            var item = new Item(res.Id, res.MinCount * amount);

            App.Instance.Player.Inventory.Add(item, IncomeSourceType.FromShopBuy);

            WindowManager.Instance.Show<WindowShopPackReward>()
                .Initialize(new List<Item> { item }, new List<Currency>());

            App.Instance.Services.AnalyticsService.InAppPurchase(res.Id, "ShopResource", res.MinCount * amount,
                price, "Crystals");

        }
        else
        {
            WindowManager.Instance.Show<WindowNotEnoughCurrency>()
                .Initialize("window_not_enough_currency_crystals_discription", "window_not_enough_currency_buy",
                    WindowManager.Instance.Show<WindowShop>()
                        .OpenCrystals);
        }
    }

}
