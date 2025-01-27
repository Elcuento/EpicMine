using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class CurrenciesView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _crystalsAmountText;
        [SerializeField] private TextMeshProUGUI _moneyAmountText;


        public void CrystalsClick()
        {
            var windowShop = WindowManager.Instance.Show<WindowShop>(withCurrencies: true);
             windowShop.OpenCrystals();
        }

        public void GoldClick()
        {
            var windowShop = WindowManager.Instance.Show<WindowShop>(withCurrencies: true);
            windowShop.OpenGold();
        }


        private void Start()
        {
            _moneyAmountText.text = App.Instance.Player.Wallet.GetExistAmount(CurrencyType.Gold).ToString();
            _crystalsAmountText.text = App.Instance.Player.Wallet.GetExistAmount(CurrencyType.Crystals).ToString();

            EventManager.Instance.Subscribe<CurrencyChangeEvent>(OnCurrencyChange);
        }

        private void OnCurrencyChange(CurrencyChangeEvent eventData)
        {
            switch (eventData.Currency.Type)
            {
                case CurrencyType.Gold:
                    _moneyAmountText.text = eventData.Currency.Amount.ToString();
                    break;
                case CurrencyType.Crystals:
                    _crystalsAmountText.text = eventData.Currency.Amount.ToString();
                    break;
            }
        }


        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<CurrencyChangeEvent>(OnCurrencyChange);
        }
    }
}