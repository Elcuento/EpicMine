using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowTorchesMerchantTorchInfoBonusItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Transform _bonusList;
        [SerializeField] private WindowTorchesMerchantTorchInfoBonusItemBonus _bonusPrefab;

        public void Initialize(Sprite sprite, string text)
        {
            _icon.sprite = sprite;
            _text.text = text;
        }

        public void AddBonus(Sprite icon, string dict, string value)
        {
            var bon = Instantiate(_bonusPrefab, _bonusList, false);
            bon.Initialize(icon,dict, value);
        }
    }
}