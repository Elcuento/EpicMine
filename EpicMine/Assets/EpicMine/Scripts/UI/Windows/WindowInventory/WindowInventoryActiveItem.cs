using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowInventoryActiveItem : MonoBehaviour
    {
        public WindowInventoryItem Target { get; private set; }

        [SerializeField] private GameObject _container;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void Initialize(WindowInventoryItem target)
        {
            Target = target;
            _icon.sprite = SpriteHelper.GetIcon(target.StaticItemId);
        }

        private void Update()
        {
            if (Target == null)
            {
                _container.SetActive(false);
                return;
            }

            _container.SetActive(Target.gameObject.activeSelf);
            transform.position = Target.transform.position;
            _amountText.text = Target.Amount.ToString();
        }
    }
}