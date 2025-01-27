using System;
using System.Linq;
using BlackTemple.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowChooseRecipeAmount : WindowBase
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _craftTime;
        [SerializeField] private TextMeshProUGUI _summ;

        [SerializeField] private TextMeshProUGUI _minAmountText;
        [SerializeField] private TextMeshProUGUI _maxAmountText;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Slider _amountSlider;


        private Core.Recipe _recipe;
        private Action<int> _onChoose;
        private int _amount;
        private int _minAmount;
        private int _maxAmount;


        public void Initialize(Core.Recipe recipe, Action<int> onChoose)
        {
            _recipe = recipe;
            _onChoose = onChoose;

            _icon.sprite = SpriteHelper.GetIcon(_recipe.StaticRecipe.Id);
            _title.text = LocalizationHelper.GetLocale(_recipe.StaticRecipe.Id);

            CalculateMaxAmount();
            _minAmount = _maxAmount > 0 ? 1 : 0;

            _amountSlider.minValue = _minAmount;
            _amountSlider.maxValue = _maxAmount;

            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(h => h.Id == _recipe.StaticRecipe.Id);
            _amount = hilt != null
                ? _maxAmount >= 1 ? 1 : 0
                : _maxAmount;

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

        public void Create()
        {
            _onChoose(_amount);
            Close();
        }


        private void CalculateMaxAmount()
        {
            var ingredients = StaticHelper.GetIngredients(_recipe.StaticRecipe);
            _maxAmount = int.MaxValue;

            foreach (var ingredient in ingredients)
            {
                var exist = App.Instance.Player.Inventory.GetExistAmount(ingredient.Id);
                var maxAmount = exist / ingredient.Amount;
                if (maxAmount < _maxAmount)
                    _maxAmount = maxAmount;
            }
        }

        private void UpdateAmounts()
        {
            _minAmountText.text = _minAmount.ToString();
            _maxAmountText.text = _maxAmount.ToString();
            _amountText.text = (_amount * _recipe.StaticRecipe.Amount).ToString();

            _craftTime.text = TimeHelper.Format(_recipe.StaticRecipe.CraftTime * _amount, detailed: true);

            var resource = App.Instance.StaticData.Resources.FirstOrDefault(r => r.Id == _recipe.StaticRecipe.Id);
            if (resource != null)
            {
                _summ.text = (resource.Price * _amount).ToString();
                return;
            }

            var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(h => h.Id == _recipe.StaticRecipe.Id);
            if (hilt != null)
                _summ.text = (hilt.Price * _amount).ToString();
        }
    }
}