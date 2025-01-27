using System.Collections;
using System.Linq;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneHeroDamagePotionBuff : MineSceneHeroBuff
    {
        public int TimeLeft { get; private set; }

        public Potion Potion { get; private set; }


        public override void Initialize(MineSceneHero hero)
        {
            base.Initialize(hero);

            Potion = App.Instance.StaticData.Potions.FirstOrDefault(p => p.Type == PotionType.Damage);
            if (Potion == null)
                return;

            if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.DamagePotionTimeLeft))
                TimeLeft = App.Instance.Services.RuntimeStorage.Load<int>(RuntimeStorageKeys.DamagePotionTimeLeft);

            if (TimeLeft <= 0)
                TimeLeft = Potion.Time * 60;

            StartCoroutine(Timer());
        }

        public override void Clear()
        {
            base.Clear();
            TimeLeft = 0;
            App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.DamagePotionTimeLeft);
        }

        public void UpdateTime()
        {
            TimeLeft = Potion.Time * 60;
        }


        private IEnumerator Timer()
        {
            while (TimeLeft > 0)
            {
                TimeLeft--;
                yield return new WaitForSeconds(1);
            }

            _hero.RemoveBuff(this);
        }

        private void OnDestroy()
        {
            if (TimeLeft > 0 && App.Instance != null)
                App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.DamagePotionTimeLeft, TimeLeft);
        }
    }
}