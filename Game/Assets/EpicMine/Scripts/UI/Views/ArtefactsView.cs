using System.Linq;
using BlackTemple.Common;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class ArtefactsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _artefactsAmountText;

        private void Start()
        {
            EventManager.Instance.Subscribe<ArtefactsAmountChangeEvent>(OnAmountChange);
            EventManager.Instance.Subscribe<TierOpenEvent>(OnTierOpen);

            _artefactsAmountText.text = "";

            UpdateAmounts();  
        }

        

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<ArtefactsAmountChangeEvent>(OnAmountChange);
                EventManager.Instance.Unsubscribe<TierOpenEvent>(OnTierOpen);
            }
        }


        private void UpdateAmounts()
        {

            _artefactsAmountText.text = $"{App.Instance.Player.Artefacts.Amount}/{LocalConfigs.MaxArtefacts}";

        }

        private void OnTierOpen(TierOpenEvent eventData)
        {
            UpdateAmounts();
        }

        private void OnAmountChange(ArtefactsAmountChangeEvent eventData)
        {
            UpdateAmounts();
        }
    }
}