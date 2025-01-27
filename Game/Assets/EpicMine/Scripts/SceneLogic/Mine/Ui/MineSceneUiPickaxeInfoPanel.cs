using BlackTemple.Common;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiPickaxeInfoPanel : MonoBehaviour
    {
        [SerializeField] private Image _pickaxeIcon;

        [SerializeField] private TextMeshProUGUI _pickaxeHealthText;

        [SerializeField] private Image _pickaxeHealthBar;

        [SerializeField] private Image _pickaxeHealthBarFx;


        private void Awake()
        {
            EventManager.Instance.Subscribe<MineScenePickaxeHealthChangeEvent>(OnPickaxeHealthChange);
        }

        private void Start()
        {
            _pickaxeIcon.sprite = SpriteHelper.GetIcon(App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id);
            ChangePickaxeHealth(App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineScenePickaxeHealthChangeEvent>(OnPickaxeHealthChange);
            }
        }

        private void OnPickaxeHealthChange(MineScenePickaxeHealthChangeEvent eventData)
        {
            ChangePickaxeHealth(eventData.Health);
        }

        private void ChangePickaxeHealth(int value)
        {
            _pickaxeHealthText.text = value.ToString();
            _pickaxeHealthBar.DOKill();
            _pickaxeHealthBarFx.DOKill();

            var fillAmount = (float)value / App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth;

            if (_pickaxeHealthBar.fillAmount > fillAmount)
            {
                _pickaxeHealthBar.fillAmount = fillAmount;
                _pickaxeHealthBarFx.DOFillAmount(fillAmount, MineLocalConfigs.PickaxeHealthbarFxTime).SetUpdate(true);
            }
            else
            {
                _pickaxeHealthBar.DOFillAmount(fillAmount, MineLocalConfigs.PickaxeHealthbarFxTime).SetUpdate(true);
                _pickaxeHealthBarFx.fillAmount = fillAmount;
            }
        }

        public void OnClick()
        {
            EventManager.Instance.Publish(new MineSectionAttackLineRotationChangeEvent());
        }
    }
}