using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class PvpSmallHeaderPanel : MonoBehaviour
    {
        [SerializeField] private Image _header;
        [SerializeField] private TextMeshProUGUI _label;

        public void SetText(string text)
        {
            _label.text = text;
        }
    }
}
