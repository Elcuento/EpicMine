using System;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class MineSceneWallSection : MineSceneAttackSection
    {
        protected MineSceneWallSectionGhost _ghost;

        protected ExploderPool _pool;


        public virtual void Start()
        {
            SectionType = SectionType.Wall;
        }


        public override void TakeDamage(float value, AttackDamageType type, bool isCritical = false, bool isMissed = false,
            MineSceneAttackPoint attackPoint = null, bool withSectionBuffsAffect = true, bool withHeroBuffsAffect = true)
        {
            base.TakeDamage(value, type, isCritical, isMissed, attackPoint, withSectionBuffsAffect, withHeroBuffsAffect);

            DotWeenHelper.ShakeWall(Camera.main, Item, type, isCritical);
            
        }

        public override void TakeDamage(float value, AttackDamageType type, bool isCritical, bool withSectionBuffsAffect, bool withHeroBuffsAffect)
        {
            base.TakeDamage(value, type);

            DotWeenHelper.ShakeWall(Camera.main, Item, type);
        }

        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            CreateExploderPool();
            InitializeGhost();
        }

        protected void InitializeGhost()
        {
            var ghost = App.Instance.Services.RuntimeStorage.Load<Ghost>(RuntimeStorageKeys.TierGhost);

            if (ghost != null)
            {
                var spawn = Number >= 0 && Random.Range(0, 100) < 40 || Number >= 4;
                if (spawn)
                {
                    var prefab = Resources.Load<MineSceneWallSectionGhost>($"{Paths.ResourcesPrefabsGhostsPath}GhostController");
                    _ghost = Instantiate(prefab, transform, false);
                    _ghost.Initialize(ghost, this);

                    App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.TierGhost);

                    EventManager.Instance.Subscribe<TierGhostDisappearEvent>(OnGhostDisappear);
                }
            }
        }

        public virtual void CreateExploderPool()
        {
            _pool = ExploderPoolController.Instance.GetRandomPool();
        }

        public void SummonGhost()
        {
            if (_ghost != null)
                return;

            if (!IsReady)
                return;

            var ghost = App.Instance.StaticData.Ghosts.Find(x => x.Tier == _selectedTier.Number + 1);

            if (ghost != null)
            {
                var prefab = Resources.Load<MineSceneWallSectionGhost>("Prefabs/Ghosts/GhostController");
                _ghost = Instantiate(prefab, transform, false);
                _ghost.Initialize(ghost, this);

                App.Instance.Services.RuntimeStorage.Remove(RuntimeStorageKeys.TierGhost);

                EventManager.Instance.Subscribe<TierGhostDisappearEvent>(OnGhostDisappear);
            }
        }

        protected override void OnHits(Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType> attackPoints, int combo)
        {
            base.OnHits(attackPoints,combo);
            
            if (attackPoints.Count >= 3)
                DotWeenHelper.ShakeWall(Camera.main, Item, AttackDamageType.Pickaxe, multiply: attackPoints.Count);

        }

        protected override void OnHit()
        {
            base.OnHit();

            DotWeenHelper.ShakeWall(Camera.main, Item, AttackDamageType.Pickaxe, multiply:3);
        }

        public virtual void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<TierGhostDisappearEvent>(OnGhostDisappear);

        }
        public void OnGhostDisappear(TierGhostDisappearEvent data)
        {
            _ghost = null;
            SetContinuePass(data.Action == GhostActionType.Speak
                ? MineLocalConfigs.WallSectionGhostSpeakDelay
                : MineLocalConfigs.WallSectionGhostFlyDelay);
        }

        protected override void OnSetReady()
        {
            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            selectedTier.UnlockDropItem(ItemId);
        }


        protected void SetContinuePass(float time)
        {
            base.SetPassed(time);
        }

        public virtual void ExplodeWall()
        {
            Item.SetActive(false);

            DotWeenHelper.ShakeWallCrash(Camera.main);

            try
            {
                ExploderPoolController.Instance.SetPosition(_pool, Item.transform.position);
                var material = Item.GetComponentInChildren<MeshRenderer>().materials[0];

                ExploderPoolController.Instance.SetFragmentPoolTexture(_pool, material);
                ExploderPoolController.Instance.ExplodeCracked(_pool);
            }
            catch (Exception)
            {
                // ignored
            }

            

        }

        protected override void SetPassed(float delay = MineLocalConfigs.WallSectionMoveDelay)
        {
            ExplodeWall();

            DropGold();
            DropItems();
            DropPickaxeBonusItem();

            if (_ghost != null)
            {
                if(_ghost.Show())
                return;
            }

            base.SetPassed(delay);

        }


        protected override void Clear()
        {
            base.Clear();
            ItemId = string.Empty;
            Health = HealthMax = 0;
            _itemContainer.ClearChildObjects();
            Item = null;
        }
    }
}