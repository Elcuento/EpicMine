using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine.Utils
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SimpleTextLocalizator : MonoBehaviour
    {
        [SerializeField] private string _key;

        [SerializeField] private string _beforeText;

        [SerializeField] private string _afterText;

        private TextMeshProUGUI _text;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _text.text = $"{_beforeText}{LocalizationHelper.GetLocale(_key)}{_afterText}";
        }

        public void SetText(string key, string beforeText = "", string afterText = "")
        {
            _key = key;
            _beforeText = beforeText;
            _afterText = afterText;
        }
    }
}