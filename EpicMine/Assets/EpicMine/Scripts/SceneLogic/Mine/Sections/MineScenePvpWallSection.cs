using System;
using System.Collections.Generic;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineScenePvpWallSection : MineSceneAttackSection
    {
        private ExploderPool _pool;
        private PvpArenaMatchInfo _matchData;

        public override void Initialize(int number, MineSceneHero hero)
        {
            _matchData = PvpArenaNetworkController.GetMatchData();

            base.Initialize(number, hero);

            SetDecal(number, _matchData.Arena);
            _pool = ExploderPoolController.Instance.GetRandomPool();
        }

        protected override void InitializeItem()
        {
            ItemId = (_matchData.Arena + 1).ToString();

            var prefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsPvpWallsPath}league_{ItemId}_wall");
            Item = Instantiate(prefab, _itemContainer, false);
        }

        public void SetDecal(int num, int arenaNumber)
        {
            var mesh = Item.GetComponentInChildren<MeshRenderer>();
            mesh.materials[0].SetTexture("_DecalTex", SpriteHelper.GetPvpNumberDecal(num).texture);
            mesh.materials[0].SetColor("_ColorDecal", PvpHelper.GetLeagueWallNumberColor(arenaNumber));
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

        public override bool AddBuff(AbilityType abilityType)
        {

            return base.AddBuff(abilityType);
        }

        public override void RemoveBuff(AbilityType abilityType)
        {

            base.RemoveBuff(abilityType);
        }

        protected override void InitializeHealth()
        {
            var wallsData = _matchData.Walls;

            float health = wallsData[Number];
            Health = HealthMax = health;
        }


        protected override void InitializeAttackAreaPoints(MineSceneAttackAreaCoordinatesPoints area)
        {
            var increaseCoefficient = Mathf.Pow(MineLocalConfigs.AttackLineMoveTimeIncreaseCoefficient, Number);
            var moveTime = MineLocalConfigs.AttackLineMoveTime * increaseCoefficient;

            var isHorizontalAttackLine = !App.Instance.Services.RuntimeStorage.Load<bool>(RuntimeStorageKeys.LastWallAttackLineIsHorizontal);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.LastWallAttackLineIsHorizontal, isHorizontalAttackLine);

            area.OnHits += OnHits;
            area.Initialize(this, isHorizontalAttackLine, moveTime);
        }

        protected override void OnHits(Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType> attackPoints, int combo)
        {

            base.OnHits(attackPoints, combo);

            if (attackPoints.Count >= 3)
               DotWeenHelper.ShakeWall(Camera.main, Item, AttackDamageType.Pickaxe, multiply: attackPoints.Count);
        }


        protected override void SetPassed(float delay = MineLocalConfigs.WallSectionMoveDelay)
        {
            ExplodeWall();

            foreach (var buff in Buffs)
            {
                buff.Clear();
                Destroy(buff.gameObject);
            }

            Buffs.Clear();

            base.SetPassed(delay);
        }

        protected void ExplodeWall()
        {
            Item.SetActive(false);

            DotWeenHelper.ShakeWallCrash(Camera.main);
            try
            {
                ExploderPoolController.Instance.SetPosition(_pool, new Vector3(Item.transform.position.x, Item.transform.position.y - 1.5f, Item.transform.position.z));
                var material = Item.GetComponentInChildren<MeshRenderer>().materials[0];

                ExploderPoolController.Instance.SetFragmentPoolTexture(_pool, material);
                ExploderPoolController.Instance.ExplodeCracked(_pool);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}