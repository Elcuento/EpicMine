using System;
using System.Collections;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneBossSection : MineSceneWallSection
    {
        public override void Start()
        {
            base.Start();
            SectionType = CommonDLL.Static.SectionType.Boss;
        }

        private Animator _wallAnimator;
     
        protected override void InitializeHealth()
        {
            var health = App.Instance.StaticData.TierBosses[_selectedTier.Number].Health;

            if (App.Instance.Player.Prestige > 0)
            {
                for (var i = 1; i <= App.Instance.Player.Prestige; i++)
                {
                    var buff = StaticHelper.GetPrestigeBuff(i);
                    health += health * (buff.WallHealthPercent / 100);
                }
            }

            if (_isHardcoreMode)
                health *= App.Instance.StaticData.Configs.Dungeon.Mines.Walls.HardcoreHealthCoefficient;

            Health = HealthMax = health;
        }


        protected override void InitializeItem()
        {
            var pName = $"tier_{_selectedTier.Number + 1}_boss";
            var prefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsBossesPath}{pName}");
            Item = Instantiate(prefab, _itemContainer, false);
            _wallAnimator = Item.GetComponent<Animator>();
            Item.name = pName;
            ItemId = $"tier_{ _selectedTier.Number }_boss";
        }

        public override void SetReady()
        {
            base.SetReady();
            StartCoroutine(HealingCycle());
        }

        protected override void OnSetReady()
        {
            
        }

        public override void CreateExploderPool()
        {
            _pool = ExploderPoolController.Instance.GetRandomSpecificPool(Item.name);
        }

        public override void ExplodeWall()
        {
            Item.SetActive(false);

            DotWeenHelper.ShakeWallCrash(Camera.main);

            try
            {
                var material = Item.GetComponentInChildren<MeshRenderer>().materials[0];
                ExploderPoolController.Instance.SetFragmentPoolTexture(_pool, material);

                ExploderPoolController.Instance.SetPosition(_pool, Item.transform.position + new Vector3(0, 2.5f, 0));
                ExploderPoolController.Instance.ExplodeCracked(_pool);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        protected override void SetPassed(float delay = MineLocalConfigs.WallSectionMoveDelay)
        {
            base.SetPassed(delay);
            StopAllCoroutines();
            AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.MineBossHealing);
        }


        protected override void DropItems()
        {
            base.DropItems();

            var boss = App.Instance.StaticData.TierBosses[_selectedTier.Number];
            DropItem(boss.DropItemId, MineHelper.GetBossDropChance(boss));

        }
        private IEnumerator Healing()
        {
            _wallAnimator?.Play("HealingStart");

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.MineBossHealing, autoPausable: true, loop: true, fadeTime: 0.3f);

            var tierBoss = App.Instance.StaticData.TierBosses[_selectedTier.Number];
            var healPercent = tierBoss.HealPercent;

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FirstTimeDiedOnBoss))
                healPercent *= TutorialLocalConfigs.FirstTimeDiedOnBossIncreaseCoefficient;

            var healValue = HealthMax / 100 * healPercent;
            var healPartDuration = 0.1f;
            var healPartsCount = Mathf.RoundToInt(tierBoss.HealDuration / healPartDuration);
            var healPartValue = healValue / healPartsCount;
            var currentTime = (float)tierBoss.HealDuration;

            while (currentTime > 0)
            {
                currentTime -= healPartDuration;
                Heal(healPartValue);
                yield return new WaitForSeconds(healPartDuration);
            }

            _wallAnimator?.Play("HealingStop");

            AudioManager.Instance.StopSound(App.Instance.ReferencesTables.Sounds.MineBossHealing, fadeTime: 0.3f);
        }

        private IEnumerator HealingCycle()
        {
            var tierBoss = App.Instance.StaticData.TierBosses[_selectedTier.Number];

            var timeout = (float)tierBoss.HealTimeout;
            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.FirstTimeDiedOnBoss))
                timeout /= TutorialLocalConfigs.FirstTimeDiedOnBossIncreaseCoefficient;

            var currentTime = timeout;
            while (true)
            {
                if (currentTime > 0)
                    currentTime -= Time.deltaTime;
                else
                {
                    StartCoroutine(Healing());
                    currentTime = timeout + tierBoss.HealDuration;
                }

                yield return null;
            }
        }
    }
}