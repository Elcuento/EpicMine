using System;
using System.Collections;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Currency = BlackTemple.EpicMine.Dto.Currency;


namespace BlackTemple.EpicMine
{
    public class WindowShopPopup : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _header;

        [SerializeField] private GameObject _pickaxePanel;
        [SerializeField] private Image _pickaxeBackgroundImage;
        [SerializeField] private Image _pickaxeIcon;
        [SerializeField] private TextMeshProUGUI _pickaxeDamage;
        [SerializeField] private TextMeshProUGUI _pickaxeDamageLevel;
        [SerializeField] private WindowBlacksmithPickaxeInfoBonusItem _bonusesPanelItemPrefab;
        [SerializeField] private Transform _bonusesPanelContainer;
        [SerializeField] private TextMeshProUGUI _pickaxeCost;


        [SerializeField] private GameObject _otherPanel;
        [SerializeField] private GameObject _firstTypeIconPanel;
        [SerializeField] private GameObject _secondTypeIconPanel;
        [SerializeField] private TextMeshProUGUI _firstTypeAmount;
        [SerializeField] private TextMeshProUGUI _secondTypeAmount;
        [SerializeField] private Image _firstTypeIcon;
        [SerializeField] private Image _secondTypeIcon;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private Image _costIcon;
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _imageRectTransform;
        [SerializeField] private TextMeshProUGUI _description;

        private ShopPack _shopPack;
        private Product _product;
        private Core.Pickaxe _pickaxe;


        public void Initialize(ShopPack shopPack)
        {
            Clear();
            _otherPanel.SetActive(true);

            _shopPack = shopPack;
            var primalAmount = 0;

            primalAmount = shopPack.Type == ShopPackType.Gold ?
                shopPack.Currency[CurrencyType.Gold] : shopPack.Type == ShopPackType.Crystals ?
                    shopPack.Currency[CurrencyType.Crystals] : 0;

            _header.text = string.Format(LocalizationHelper.GetLocale(_shopPack.Id), primalAmount == 0 ? "" : primalAmount.ToString() );

            var sprite = SpriteHelper.GetShopPackImage(_shopPack.Id);
            var pivotX = sprite.pivot.x / sprite.textureRect.width;
            var pivotY = sprite.pivot.y / sprite.textureRect.height;
            _imageRectTransform.pivot = new Vector2(pivotX, pivotY);
            _imageRectTransform.anchoredPosition = new Vector2(0, -40f);
            _image.sprite = sprite;
            _image.SetNativeSize();

            _cost.text = _shopPack.Cost.ToString();
            _costIcon.gameObject.SetActive(true);

            switch (_shopPack.Type)
            {
                case ShopPackType.Gold:
                    _secondTypeIconPanel.SetActive(true);
                    _secondTypeAmount.text = primalAmount.ToString();
                    var descriptionLocale = LocalizationHelper.GetLocale(_shopPack.Id + "_description");
                    _description.text = descriptionLocale;

                    var amount = ShopHelper.GetCurrentGoldShopPackAmount(primalAmount);
                    _header.text = string.Format(LocalizationHelper.GetLocale(_shopPack.Id), amount);
                    _secondTypeAmount.text = amount.ToString();
                    _secondTypeIcon.sprite = SpriteHelper.GetCurrencyIcon(CurrencyType.Gold);
                    break;
                case ShopPackType.Chest:
                case ShopPackType.Potion:
                case ShopPackType.Tnt:
                    ShowFirstTypePanel();
                    break;
            }
        }

        public void Initialize(Core.Pickaxe blPickaxe)
        {
            Clear();

            _pickaxePanel.SetActive(true);
            _pickaxe = blPickaxe;

            _header.text = LocalizationHelper.GetLocale(_pickaxe.StaticPickaxe.Id);

            _pickaxeDamage.text = $"{LocalizationHelper.GetLocale("damage")}: {_pickaxe.StaticPickaxe.Damage}";
            _pickaxeDamageLevel.text = $"{LocalizationHelper.GetLocale("window_blacksmith_required_damage_level")} {_pickaxe.StaticPickaxe.RequiredDamageLevel}";
            StartCoroutine(UpdatePickaxeCanvas());

            _pickaxeIcon.sprite = SpriteHelper.GetPickaxeImage(_pickaxe.StaticPickaxe.Id);
            _pickaxeCost.text = _pickaxe.StaticPickaxe.Cost.ToString();
            _pickaxeBackgroundImage.sprite = SpriteHelper.GetPickaxeLuxBackground(_pickaxe);

            SetPickaxeBonuses();
        }

        public void PickaxeBuyClick()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            _pickaxe.Create(success =>
            {
                if (success)
                {
                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Pay);
                    Close();
                }
            });
        }

        public void ShopPackBuyClick()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (_product != null)
            {
                InAppPurchaseManager.Instance.Buy(_product.definition.id);
                return;
            }

            if (_shopPack != null && _shopPack.Type != ShopPackType.Crystals)
            {
                var cost = new Currency(CurrencyType.Crystals, _shopPack.Cost);
                if (!App.Instance.Player.Wallet.Has(cost))
                {
                    WindowManager.Instance.Show<WindowShop>()
                        .OpenCrystals();

                    return;
                }

                WindowManager
                    .Instance
                    .Show<WindowCurrencySpendConfirm>()
                    .Initialize(
                        cost,
                        () =>
                        {
                          //  var buyRequest = new BuyShopPackRequest(_shopPack.Id);
                           // NetworkManager.Instance.Send<ResponseBase>(buyRequest, OnBuyShopPackSuccess);
                        },
                        "window_currency_spend_confirm_description_shop",
                        "window_currency_spend_confirm_ok_shop");
            }
        }



        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
            Unsubscribe();
        }

        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
        }


        private void SetPickaxeBonuses()
        {
            _bonusesPanelContainer.ClearChildObjects();

            if (_pickaxe.IsUnlocked)
            {
                if (_pickaxe.StaticPickaxe.BonusGoldPercent != null && _pickaxe.StaticPickaxe.BonusGoldPercent > 0)
                {
                    var goldLocale = LocalizationHelper.GetLocale("window_blacksmith_bonus_gold");
                    AddBonusItem(SpriteHelper.GetCurrencyIcon(CurrencyType.Gold), string.Format(goldLocale, _pickaxe.StaticPickaxe.BonusGoldPercent));
                }

                if (_pickaxe.StaticPickaxe.BonusDamagePercent != null && _pickaxe.StaticPickaxe.BonusDamagePercent > 0)
                {
                    var powerLocale = LocalizationHelper.GetLocale("power_description");
                    AddBonusItem(App.Instance.ReferencesTables.Sprites.DamageIcon, $"{powerLocale} +{_pickaxe.StaticPickaxe.BonusDamagePercent}%");
                }

                if (_pickaxe.StaticPickaxe.BonusFortunePercent != null && _pickaxe.StaticPickaxe.BonusFortunePercent > 0)
                {
                    var fortuneLocale = LocalizationHelper.GetLocale("fortune_description");
                    AddBonusItem(App.Instance.ReferencesTables.Sprites.FortuneIcon, $"{fortuneLocale} +{_pickaxe.StaticPickaxe.BonusFortunePercent}%");
                }

                if (_pickaxe.StaticPickaxe.BonusCritPercent != null && _pickaxe.StaticPickaxe.BonusCritPercent > 0)
                {
                    var critLocale = LocalizationHelper.GetLocale("crit_description");
                    AddBonusItem(App.Instance.ReferencesTables.Sprites.CritIcon, $"{critLocale} +{_pickaxe.StaticPickaxe.BonusCritPercent}%");
                }

                if (!string.IsNullOrEmpty(_pickaxe.StaticPickaxe.BonusDropItemId) && _pickaxe.StaticPickaxe.BonusDropItemPercent != null && _pickaxe.StaticPickaxe.BonusDropItemPercent > 0)
                {
                    var locale = LocalizationHelper.GetLocale("window_blacksmith_bonus_resource_chance");
                    var itemLocale = LocalizationHelper.GetLocale(_pickaxe.StaticPickaxe.BonusDropItemId).ToLower();
                    AddBonusItem(SpriteHelper.GetIcon(_pickaxe.StaticPickaxe.BonusDropItemId), string.Format(locale, itemLocale, _pickaxe.StaticPickaxe.BonusDropItemPercent));
                }
            }
        }

        private void AddBonusItem(Sprite sprite, string text)
        {
            var item = Instantiate(_bonusesPanelItemPrefab, _bonusesPanelContainer, false);
            item.Initialize(sprite, text);
        }

        private IEnumerator UpdatePickaxeCanvas()
        {
            _pickaxeDamageLevel.text += " ";
            yield return new WaitForEndOfFrame();
            _pickaxeDamageLevel.text = _pickaxeDamageLevel.text.Trim();
        }


        private void ShowFirstTypePanel()
        {
            _firstTypeIconPanel.SetActive(true);

            var firstItem = _shopPack.Items.First();
            var iconId = firstItem.Key;
            _firstTypeIcon.sprite = SpriteHelper.GetIcon(iconId);
            _firstTypeAmount.text = firstItem.Value > 1 ? firstItem.Value.ToString() : "";

            var descriptionLocale = LocalizationHelper.GetLocale(_shopPack.Id + "_description");
            switch (_shopPack.Type)
            {
                case ShopPackType.Chest:
                    _description.text = descriptionLocale;
                    break;
                case ShopPackType.Potion:
                    var staticPotion = App.Instance.StaticData.Potions.FirstOrDefault(p => p.Id == firstItem.Key);
                    if (staticPotion != null)
                    {
                        switch (staticPotion.Type)
                        {
                            case PotionType.Damage:
                                _description.text = string.Format(descriptionLocale, staticPotion.Value, staticPotion.Time);
                                break;
                            case PotionType.Health:
                            case PotionType.Energy:
                                _description.text = string.Format(descriptionLocale, staticPotion.Value);
                                break;
                        }
                    }

                    break;
                case ShopPackType.Tnt:
                    var tnt = App.Instance.StaticData.Tnt.First();
                    _description.text = string.Format(descriptionLocale, tnt.DamagePercent);
                    break;
            }
        }

        private void Clear()
        {
            _pickaxePanel.SetActive(false);
            _otherPanel.SetActive(false);
            _secondTypeIconPanel.SetActive(false);
            _secondTypeIconPanel.SetActive(false);
            _shopPack = null;
            _product = null;
            _pickaxe = null;
        }

        private void Unsubscribe()
        {
        
        }

        private void OnDestroy()
        {
            Clear();
            Unsubscribe();
        }
    }
}