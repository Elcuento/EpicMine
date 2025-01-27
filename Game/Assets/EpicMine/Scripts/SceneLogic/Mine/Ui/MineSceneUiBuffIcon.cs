using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiBuffIcon : MonoBehaviour
    {
        public MineSceneSectionBuff Buff { get; private set; }

        [SerializeField] private Image _icon;

        [SerializeField] private TextMeshProUGUI _text;


        public void Initialize(MineSceneSectionBuff buff)
        {
            Buff = buff;
            _icon.sprite = SpriteHelper.GetSectionBuffIcon(buff);

            var freezingBuff = Buff as MineSceneSectionFreezingBuff;
            if (freezingBuff != null)
            {
                _text.gameObject.SetActive(true);
                _text.text = freezingBuff.Stacks.ToString();
                return;
            }

            var acidBuff = Buff as MineSceneSectionAcidBuff;
            if (acidBuff != null)
            {
                _text.gameObject.SetActive(true);
                _text.text = acidBuff.Stacks.ToString();
                return;
            }

            _text.gameObject.SetActive(false);
        }
    }
}