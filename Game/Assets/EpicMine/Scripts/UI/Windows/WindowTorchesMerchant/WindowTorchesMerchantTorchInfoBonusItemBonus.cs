using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowTorchesMerchantTorchInfoBonusItemBonus : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _value;

        public void Initialize(Sprite sprite, string text, string value)
        {
            _icon.sprite = sprite;
            _text.text = text;
            _value.text = value;
        }
    }
}