using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiHeroBuffItem : MonoBehaviour
    {
        public MineSceneHeroBuff Buff { get; private set; }

        [SerializeField] private Image _icon;

        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(MineSceneHeroBuff buff)
        {
            StopAllCoroutines();
            Buff = buff;

            _icon.sprite = SpriteHelper.GetHeroBuffIcon(buff);

            var damagePotionBuff = Buff as MineSceneHeroDamagePotionBuff;
            if (damagePotionBuff != null)
            {
                StartCoroutine(Timer(damagePotionBuff.TimeLeft));
            }

            var acidBuff = Buff as MineSceneHeroAcidBuff;
            if (acidBuff != null)
            {
                StartCoroutine(Timer(acidBuff.TimeLeft));
            }

            // ReSharper disable once IdentifierTypo
            var prestigebuff = Buff as MineSceneHeroPrestigeBuff;
            if (prestigebuff != null)
            {
                _text.text = "";
            }
        }


        private IEnumerator Timer(int timeLeft)
        {
            while (timeLeft > 0)
            {
                _text.text = TimeHelper.Format(timeLeft, detailed: true);
                yield return new WaitForSeconds(1f);
                timeLeft--;
            }
        }
    }
}