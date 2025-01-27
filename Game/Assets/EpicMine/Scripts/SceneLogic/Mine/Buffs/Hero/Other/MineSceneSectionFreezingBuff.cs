using System.Collections;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneSectionFreezingBuff : MineSceneSectionBuff
    {
        public int Stacks { get; private set; }

        public int TimeLeft { get; private set; }

        [SerializeField] private SpriteRenderer[] _fx;

        [SerializeField] private ParticleSystem _particles;


        public override void Initialize(MineSceneSection section, AbilityType type)
        {
            base.Initialize(section, type);

            AddStack();
            StartCoroutine(Timer());

            if(section is MineSceneMonsterSection)
                foreach (var spriteRenderer in _fx)
                {
                    spriteRenderer.enabled = false;
                }
        }

        public override void Clear()
        {
            base.Clear();
            Stacks = 0;

            foreach (var fx in _fx)
            {
                fx.DOKill();
                fx.DOFade(0, 0).SetUpdate(true);
            }
        }


        public void AddStack()
        {
            Stacks++;
            TimeLeft = App.Instance.Player.Abilities.Freezing.StaticLevel.Duration;

            if (Stacks > MineLocalConfigs.FreezingAbilityMaxStacks)
                Stacks = MineLocalConfigs.FreezingAbilityMaxStacks;

            foreach (var fx in _fx)
                fx.DOKill();

            _fx[Stacks - 1].DOFade(1, 0.25f).SetUpdate(true);
            _particles.Play();

            var randomIncreaseSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.IncreaseDamages.Length);
            var randomIncreaseSound = App.Instance.ReferencesTables.Sounds.IncreaseDamages[randomIncreaseSoundIndex];
            AudioManager.Instance.PlaySound(randomIncreaseSound);
        }

        public void SubtractStack()
        {
            if (Stacks > 0)
            {
                _fx[Stacks - 1].DOKill();
                _fx[Stacks - 1].DOFade(0, 0.25f).SetUpdate(true);
            }

            Stacks--;
            _section.FireBuffsChangeEvent();

            if (Stacks > 0)
                return;

            _section.RemoveBuff(AbilityType.Freezing);
        }


        private IEnumerator Timer()
        {
            while (TimeLeft > 0)
            {
                yield return new WaitForSeconds(1f);

                TimeLeft--;

                if (TimeLeft == 2)
                {
                    foreach (var fx in _fx)
                    {
                        var alpha = fx.color.a;
                        if (alpha > 0)
                        {
                            fx.DOKill();
                            fx.DOFade(0.2f, 0.25f);
                            fx.DOFade(alpha, 0.25f).SetDelay(0.25f);
                            fx.DOFade(0.2f, 0.25f).SetDelay(0.5f);
                            fx.DOFade(alpha, 0.25f).SetDelay(0.75f);
                            fx.DOFade(0.2f, 0.25f).SetDelay(1f);
                            fx.DOFade(alpha, 0.25f).SetDelay(1.25f);
                            fx.DOFade(0.2f, 0.25f).SetDelay(1.5f);
                        }
                    }
                }
            }

            Clear();
            _section.RemoveBuff(AbilityType.Freezing);
        }
    }
}