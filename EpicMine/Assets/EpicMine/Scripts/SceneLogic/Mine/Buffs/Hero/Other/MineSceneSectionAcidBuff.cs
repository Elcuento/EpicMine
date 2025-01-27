using System.Collections;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneSectionAcidBuff : MineSceneSectionBuff
    {
        public int Stacks { get; private set; }

        public int TimeLeft { get; private set; }

        [SerializeField] private SpriteRenderer[] _fx;


       public override void Initialize(MineSceneSection section, AbilityType type)
       {
           base.Initialize(section, type);

            if (TimeLeft <= 0)
            {
                var buff = section.Hero.Buffs.Find(x => x is MineSceneHeroAcidBuff) as MineSceneHeroAcidBuff;
                if (buff != null && buff.TimeLeft > 0)
                {
                    for (var i = 0; i < buff.Stuck; i++)
                        AddStack();

                    TimeLeft = buff.TimeLeft;
                    StartCoroutine(Timer());
                }
                else
                {
                    AddStack();
                    StartCoroutine(Timer());
                }
                
            }
            else
            {
                AddStack();
                StartCoroutine(Timer());
            }

            var isMonsterSection = section is MineSceneMonsterSection;
            foreach (var spriteRenderer in _fx)
            {
                spriteRenderer.enabled = !isMonsterSection;
            }

        }

        public override void Clear()
        {
            base.Clear();
            Stacks = 0;
            StopAllCoroutines();

            foreach (var fx in _fx)
            {
                fx.transform.DOKill();
                fx.DOKill();
                fx.DOFade(0, 0);
            }
        }

        public void AddStack()
        {
            Stacks++;
            TimeLeft = App.Instance.Player.Abilities.Acid.StaticLevel.Duration;

            if (Stacks > MineLocalConfigs.AcidAbilityMaxStacks)
                Stacks = MineLocalConfigs.AcidAbilityMaxStacks;
            else
            {
                StartCoroutine(MoveFx(_fx[Stacks - 1]));
                StartCoroutine(Damage());
            }

            var randomTickingSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.TickingDamages.Length);
            var randomTickingSound = App.Instance.ReferencesTables.Sounds.TickingDamages[randomTickingSoundIndex];
            AudioManager.Instance.PlaySound(randomTickingSound);
        }


        private IEnumerator Timer()
        {
            while (TimeLeft > 0)
            {
                TimeLeft--;
                yield return new WaitForSeconds(1f);
            }

            _section.RemoveBuff(AbilityType.Acid);
        }

        private IEnumerator Damage()
        {
            var wallSection = (MineSceneAttackSection)_section;

            var torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch;
            var extraDamage = torch.AcidDamage ?? 0;
            while (TimeLeft > 0)
            {
                wallSection.TakeDamage(App.Instance.Player.Abilities.Acid.StaticLevel.Damage + extraDamage,
                    withSectionBuffsAffect: false, type: AttackDamageType.AcidAbility);
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator MoveFx(SpriteRenderer fx)
        {
            while (TimeLeft > 0)
            {
                var randomPositionX = Random.Range(-MineLocalConfigs.HorizontalAttackLineMaxXPosition, MineLocalConfigs.HorizontalAttackLineMaxXPosition);
                var randomPosition = new Vector3(randomPositionX, MineLocalConfigs.VerticalAttackLineMaxYPosition, fx.transform.localPosition.z);
                var randomTime = Random.Range(4.5f, 6.5f);
                var randomDelay = Random.Range(0f, 2.5f);

                fx.transform.localPosition = randomPosition;
                fx.transform
                    .DOLocalMoveY(-MineLocalConfigs.VerticalAttackLineMaxYPosition - 1, randomTime)
                    .OnStart(() => { fx.DOFade(1, randomTime / 4); })
                    .SetEase(Ease.Linear)
                    .SetDelay(randomDelay);

                fx.DOFade(0, randomTime / 4)
                    .SetDelay(randomTime - randomTime / 4 + randomDelay);

                yield return new WaitForSeconds(randomTime + randomDelay);
            }
        }
    }
}