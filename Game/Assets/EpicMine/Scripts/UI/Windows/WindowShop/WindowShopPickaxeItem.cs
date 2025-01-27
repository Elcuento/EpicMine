using System;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Currency = BlackTemple.EpicMine.Dto.Currency;

namespace BlackTemple.EpicMine
{
    public class WindowShopPickaxeItem : MonoBehaviour
    {

        private Action<Core.Pickaxe> _onClick;

        [SerializeField] private Image _iconBackground;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _locked;
        [SerializeField] private TextMeshProUGUI _lockedText;
        [SerializeField] private TextMeshProUGUI _buyedText;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _costText;

        private Core.Pickaxe _pickaxe;


        public void Initialize(Core.Pickaxe pickaxe)
        {
            _pickaxe = pickaxe;

            _iconBackground.sprite = SpriteHelper.GetPickaxeLuxBackground(_pickaxe);
            _icon.sprite = SpriteHelper.GetPickaxeImage(_pickaxe.StaticPickaxe.Id);

            if (App.Instance.Player.Dungeon.LastOpenedTier.Number + 1 < _pickaxe.StaticPickaxe.RequiredTierNumber)
            {
                _icon.color = Color.black;
                _locked.SetActive(true);
                _lockedText.text = string.Format(LocalizationHelper.GetLocale("window_shop_pickaxe_locked"), _pickaxe.StaticPickaxe.RequiredTierNumber);
            }
            else
            {
                _icon.color = Color.white;
                _locked.SetActive(false);
                _buyedText.text = _pickaxe.IsCreated ? LocalizationHelper.GetLocale("window_shop_pickaxe_buyed") : " ";
            }

            _title.text = string.Format(LocalizationHelper.GetLocale(_pickaxe.StaticPickaxe.Id));
            _costText.text = _pickaxe.StaticPickaxe.Cost.ToString();

            EventManager.Instance.Subscribe<PickaxeCreateEvent>(OnPickaxeCreate);
        }
        

        public void Click()
        {
            
            if (App.Instance.Player.Dungeon.LastOpenedTier.Number + 1 < _pickaxe.StaticPickaxe.RequiredTierNumber)
                return;
            
            if (_pickaxe.IsCreated)
                return;

            var cost = new Dto.Currency(CurrencyType.Crystals, _pickaxe.StaticPickaxe.Cost);

            if (!App.Instance.Player.Wallet.Has(cost))
            {
                WindowManager.Instance.Show<WindowNotEnoughCurrency>()
                    .Initialize("window_not_enough_currency_crystals_discription", "window_not_enough_currency_buy",
                        WindowManager.Instance.Show<WindowShop>().OpenCrystals);

                return;
            }

            if (!_pickaxe.CanCreateDonate())
                return;


            WindowManager
                .Instance
                .Show<WindowCurrencySpendConfirm>()
                .Initialize(
                    cost,
                    () =>
                    {
                        _pickaxe.Create(success =>
                        {
                            if (success)
                            {
                                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Pay);
                                WindowManager.Instance.Show<WindowShopPackReward>()
                                    .Initialize(new List<Item>() {new Item(_pickaxe.StaticPickaxe.Id,1)}, new List<Currency>());
                            }
                        });
                    },
                    "window_currency_spend_confirm_description_shop",
                    "window_currency_spend_confirm_ok_shop");
        }


        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<PickaxeCreateEvent>(OnPickaxeCreate);
        }

        private void OnPickaxeCreate(PickaxeCreateEvent eventData)
        {
            if (_pickaxe != eventData.Pickaxe)
                return;

            _buyedText.text = LocalizationHelper.GetLocale("window_shop_pickaxe_buyed");
            Unsubscribe();
        }
    }
}