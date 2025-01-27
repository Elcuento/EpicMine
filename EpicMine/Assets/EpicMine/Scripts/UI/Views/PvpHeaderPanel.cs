using BlackTemple.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class PvpHeaderPanel : MonoBehaviour
    {
        [SerializeField] private Image _header;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Transform _extraAttributeTransform;


        public void SetColors(int league)
        {
            _header.color = PvpHelper.GetLeagueFlagColor(league);
            _label.color = PvpHelper.GetLeagueTextColor(league);
        }

        public void Initialize(int league, string label)
        {
            SetColors(league);
            _label.text = label;

            LoadLeagueAttribute(league);
        }

        public void LoadLeagueAttribute(int league)
        {
            _extraAttributeTransform.ClearChildObjects();
            var attribute = Instantiate(PvpHelper.GetHeaderAttributePrefab(league));
            attribute.transform.SetParent(_extraAttributeTransform, false);
        }

    }
}
