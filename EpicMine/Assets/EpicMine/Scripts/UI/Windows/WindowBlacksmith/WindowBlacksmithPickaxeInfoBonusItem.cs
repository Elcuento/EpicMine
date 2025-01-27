using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowBlacksmithPickaxeInfoBonusItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(Sprite sprite, string text)
        {
            _icon.sprite = sprite;
            _text.text = text;
        }
    }
}