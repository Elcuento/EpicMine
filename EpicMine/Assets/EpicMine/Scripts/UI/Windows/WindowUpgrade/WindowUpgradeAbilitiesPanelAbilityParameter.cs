using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowUpgradeAbilitiesPanelAbilityParameter : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        [SerializeField] private TextMeshProUGUI _title;

        [SerializeField] private TextMeshProUGUI _value;

        [SerializeField] private GameObject _greenBackground;


        public void Initialize(Sprite sprite, string title, string value, bool isIncreased)
        {
            _icon.sprite = sprite;
            _title.text = title;
            _value.text = value;

            _greenBackground.SetActive(isIncreased);
            StartCoroutine(UpdateCanvas());
        }


        private IEnumerator UpdateCanvas()
        {
            _value.text += " ";
            yield return new WaitForEndOfFrame();
            _value.text = _value.text.Trim();
        }
    }
}