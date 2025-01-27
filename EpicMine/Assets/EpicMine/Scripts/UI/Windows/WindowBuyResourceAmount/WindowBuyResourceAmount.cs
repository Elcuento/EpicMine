using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowBuyResourceAmount : WindowBase
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _sale;
        [SerializeField] private TextMeshProUGUI _summ;

        [SerializeField] private TextMeshProUGUI _minAmountText;
        [SerializeField] private TextMeshProUGUI _maxAmountText;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Slider _amountSlider;


        private ShopResources _resource;
        private Action<ShopResources, int, float> _onChoose;
        private int _amount;
        private int _minAmount;
        private int _maxAmount;
        private float _saleAmount;



        public void Initialize(ShopResources res, Action<ShopResources, int, float> onChoose)
        {
            _resource = res;
            _onChoose = onChoose;

            _icon.sprite = SpriteHelper.GetIcon(_resource.Id);
            _title.text = LocalizationHelper.GetLocale(_resource.Id);

            CalculateMaxAmount();
            _minAmount = 1;
             _amount = 1;

            _amountSlider.minValue = _minAmount;
            _amountSlider.maxValue = _maxAmount;

            _amountSlider.value = _amount;
            _amountSlider.direction = _maxAmount <= 1 ? Slider.Direction.RightToLeft : Slider.Direction.LeftToRight;

            UpdateAmounts();
        }

        public void IncreaseAmount()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _amount++;

            if (_amount > _maxAmount)
                _amount = _maxAmount;

            UpdateAmounts();
            _amountSlider.value = _amount;
        }

        public void DecreaseAmount()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);
            _amount--;

            if (_amount < _minAmount)
                _amount = _minAmount;

            UpdateAmounts();
            _amountSlider.value = _amount;
        }

        public void SetAmount(float amount)
        {
            _amount = Mathf.Clamp((int)amount, _minAmount, _maxAmount);
            UpdateAmounts();
        }

        public void OnClickBuy()
        {
            _onChoose(_resource, _amount, _saleAmount);
            Close();
        }


        private void CalculateMaxAmount()
        {
            var currency = App.Instance.Player.Wallet.GetExistAmount(CurrencyType.Crystals);

            var max = currency / _resource.CrystalCost;
            _maxAmount = (int) (max > 500 ? 500 : max);
        }

        private void UpdateAmounts()
        {
            _saleAmount = ShopHelper.GetResourceSalePercent(_amount);
            var sum = _amount * _resource.CrystalCost;
            var finalSum = Math.Round(sum - sum * _saleAmount);


            _minAmountText.text = (_minAmount * _resource.MinCount).ToString();
            _maxAmountText.text = (_maxAmount * _resource.MinCount).ToString();
            _amountText.text = (_amount * _resource.MinCount).ToString();


            _sale.text = $"{ (int)(_saleAmount * 100) } %";
            _summ.text = $"{LocalizationHelper.GetLocale("window_currency_spend_confirm_ok_shop")} {finalSum}";

        }
    }
}