using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;

using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CommonDLL.Dto;
using CommonDLL.Static;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackSection : MineSceneSection
    {
        public Action<MineSceneAttackPoint, AttackPointHitType, 
            AttackDamageType, int, bool> OnPickaxeHit { get; set; }

        public ObscuredFloat Health { get; set; }
        public ObscuredFloat HealthMax { get; set; }

        public string ItemId { get; protected set; }
        public GameObject Item { get; protected set; }

        public MineSceneAttackArea AttackArea;

        [SerializeField] protected Transform _itemContainer;

        protected Dictionary<string, int> _extraDrop { get; set; } = new Dictionary<string, int>();

        protected Core.Tier _selectedTier;
        protected Core.Mine _selectedMine;
        protected bool _isHardcoreMode;

        public int DamageReceived { get; private set; }

        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            _selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            _selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            _isHardcoreMode = App.Instance
                .Services
                .RuntimeStorage
                .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

            InitializeItem();
            InitializeHealth();
        }

        public virtual void TakeDamage(float value, AttackDamageType type, bool isCritical = false, bool isMissed = false,
            MineSceneAttackPoint attackPoint = null, bool withSectionBuffsAffect = true, bool withHeroBuffsAffect = true)
        {
            if (Health <= 0)
                return;

            var frostDamage = 0f;

            if (withSectionBuffsAffect)
            {
                var existBuff = Buffs?.FirstOrDefault(b => b is MineSceneSectionFreezingBuff);
                if (existBuff != null)
                {
                    var freezingBuff = (MineSceneSectionFreezingBuff)existBuff;
                    var torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch;
                    frostDamage = (App.Instance.Player.Abilities.Freezing.StaticLevel.Damage + (torch.FreezingAdditionalParameter ?? 0)) * (freezingBuff.Stacks);
                }
            }

            if (withHeroBuffsAffect)
            {
                var damagePotionBuff = Hero.Buffs?.FirstOrDefault(b => b is MineSceneHeroDamagePotionBuff);
                if (damagePotionBuff != null)
                {
                    var heroDamagePotionBuff = (MineSceneHeroDamagePotionBuff)damagePotionBuff;
                    value += value / 100f * heroDamagePotionBuff.Potion.Value;
                }
            }

            DamageReceived += (int)(value + frostDamage);

            Health -= value + frostDamage;

            EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, attackPoint, value, isCritical, isMissed, type));

            if (frostDamage > 0)
                EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, null, frostDamage, false, false, AttackDamageType.FrostAbility));

            if (Health <= 0)
                DestroySection();
        }

        public virtual void AddExtraDrop(string key, int val)
        {
            if (!_extraDrop.ContainsKey(key))
                _extraDrop.Add(key, val);
        }

        public virtual void RemoveExtraDrop(string key, int val)
        {
            _extraDrop.Remove(key);
        }

        public override void DestroySection()
        {
            AttackArea.Clear();
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.WallDestroyed);

            Health = 0;
            SetPassed();
        }

        public virtual void TakeDamage(float value, AttackDamageType type, bool isCritical, bool withSectionBuffsAffect, bool withHeroBuffsAffect)
        {
            if (Health <= 0)
                return;

            var frostDamage = 0f;

            if (withSectionBuffsAffect)
            {
                var existBuff = Buffs?.FirstOrDefault(b => b is MineSceneSectionFreezingBuff);
                if (existBuff != null)
                {
                    var freezingBuff = (MineSceneSectionFreezingBuff)existBuff;
                    var torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch;
                    frostDamage = (App.Instance.Player.Abilities.Freezing.StaticLevel.Damage + (torch.FreezingAdditionalParameter ?? 0)) * (freezingBuff.Stacks);
                }
            }

            if (withHeroBuffsAffect)
            {
                var damagePotionBuff = Hero.Buffs?.FirstOrDefault(b => b is MineSceneHeroDamagePotionBuff);
                if (damagePotionBuff != null)
                {
                    var heroDamagePotionBuff = (MineSceneHeroDamagePotionBuff)damagePotionBuff;
                    value += value / 100f * heroDamagePotionBuff.Potion.Value;
                }
            }

            DamageReceived += (int)(value + frostDamage);

            Health -= value + frostDamage;

            EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, null, value, isCritical, false, type));

            if (frostDamage > 0)
                EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, null, frostDamage, isCritical, false, AttackDamageType.FrostAbility));

            if (Health <= 0)
                DestroySection();

            
            EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, null, value, isCritical, false, type));

            if (Health <= 0)
                DestroySection();

        }

        public void Heal(float value)
        {
            Health += value;

            if (Health > HealthMax)
                Health = HealthMax;

            EventManager.Instance.Publish(new MineSceneWallSectionHealEvent(this));
        }

        public override bool AddBuff(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.ExplosiveStrike:
                    // instant damage, no need to add to buffs list
                    _buffFactory.CreateExplosiveStrikeBuff();
                    return true;

                case AbilityType.Freezing:
                    var existFreezingBuff = Buffs.FirstOrDefault(b => b is MineSceneSectionFreezingBuff);
                    if (existFreezingBuff != null)
                    {
                        var freezingBuff = (MineSceneSectionFreezingBuff) existFreezingBuff;
                        freezingBuff.AddStack();
                    }
                    else
                    {
                        Buffs.Add(_buffFactory.CreateFreezingBuff());
                    }

                    FireBuffsChangeEvent();
                    return true;

                case AbilityType.Acid:
                    var existAcidBuff = Buffs.FirstOrDefault(b => b is MineSceneSectionAcidBuff);
                    if (existAcidBuff != null)
                    {
                        var acidBuff = (MineSceneSectionAcidBuff)existAcidBuff;
                        acidBuff.AddStack();
                    }
                    else
                    {
                        Buffs.Add(_buffFactory.CreateAcidBuff());
                    }

                    FireBuffsChangeEvent();
                    return true;

                case AbilityType.Tnt:
                    _buffFactory.CreateTntBuff();
                    FireBuffsChangeEvent();
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public override void RemoveBuff(AbilityType abilityType)
        {
            MineSceneSectionBuff buff = null;

            switch (abilityType)
            {
                case AbilityType.ExplosiveStrike:
                    break;
                case AbilityType.Freezing:
                    buff = Buffs.FirstOrDefault(b => b is MineSceneSectionFreezingBuff);
                    break;
                case AbilityType.Acid:
                    buff = Buffs.FirstOrDefault(b => b is MineSceneSectionAcidBuff);
                    break;
                case AbilityType.Tnt:
                    buff = Buffs.FirstOrDefault(b => b is MineSceneSectionTnt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }

            if (buff != null)
            {
                buff.Clear();
                Buffs.Remove(buff);
                Destroy(buff.gameObject);
                FireBuffsChangeEvent();
            }
        }

        protected virtual void InitializeHealth()
        {
            var staticMine = StaticHelper.GetMine(_selectedTier.Number, _selectedMine.Number);
            var staticWalls = App.Instance.StaticData.MineWalls;

            float health;

            if (Number <= staticWalls.Count - 1)
            {
                var wall = staticWalls[Number];
                health = wall.HealthCoefficient * staticMine.AverageWallHealth;
            }

            // if wall number is larger than the maximum wall number,
            // health is increased from last wall by 10 percent in the degree of difference of numbers
            else
            {
                var additionalCoefficient = Mathf.Pow(1.1f, Number - (staticWalls.Count - 1));
                var lastWall = staticWalls.Last();
                var lastWallHealth = lastWall.HealthCoefficient * staticMine.AverageWallHealth;
                health = lastWallHealth * additionalCoefficient;
            }

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

        protected virtual void InitializeItem()
        {
            ItemId = MineHelper.GetRandomWallDropItem(_selectedTier, _selectedMine);

            var prefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsWallsPath}{ItemId}");
            Item = Instantiate(prefab, _itemContainer, false);
            Item.name = ItemId;
        }

        protected virtual void InitializeAttackAreaNoPoints()
        {
            AttackArea.OnHit += OnHit;
            AttackArea.Initialize(this, false, 0);
        }

        protected virtual void InitializeAttackAreaPoints(MineSceneAttackAreaCoordinatesPoints area)
        {
            
            var increaseCoefficient = Mathf.Pow(MineLocalConfigs.AttackLineMoveTimeIncreaseCoefficient, (Number - 10));
            var moveTime = Number <= 10 ? MineLocalConfigs.AttackLineMoveTime : MineLocalConfigs.AttackLineMoveTime * increaseCoefficient;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.BossMeeting) && !selectedMine.IsComplete)
            {

                switch (selectedMine.Number)
                {
                    case 0:
                        moveTime *= 1.5f;
                        break;
                    case 1:
                        moveTime *= 1.4f;
                        break;
                    case 2:
                        moveTime *= 1.3f;
                        break;
                    case 3:
                        moveTime *= 1.2f;
                        break;
                    case 4:
                        moveTime *= 1.1f;
                        break;
                }
            }

            var isHorizontalAttackLine = !App.Instance.Services.RuntimeStorage.Load<bool>(RuntimeStorageKeys.LastWallAttackLineIsHorizontal);
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.LastWallAttackLineIsHorizontal, isHorizontalAttackLine);

            area.OnHits += OnHits;

            AttackArea.OnHit += OnHit;
            AttackArea.Initialize(this, isHorizontalAttackLine, moveTime);
        }

        protected virtual void OnHit()
        {
            var staticPickaxe = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe;
            var criticalChance = App.Instance.Player.Skills.Crit.Value;


            var damage = MineHelper.GetCurrentPickaxeDamage();
            var isCritical = false;

            if (staticPickaxe.BonusCritPercent > 0)
                criticalChance += staticPickaxe.BonusCritPercent.Value;

            if (App.Instance.Player.Prestige > 0)
                criticalChance += StaticHelper.GetCurrentPrestigeBuff().CriticalPercent;

            var randomValue = Random.Range(0f, 100f);
            if (randomValue <= criticalChance)
            {
                damage *= App.Instance.StaticData.Configs.Dungeon.Mines.CritDamageCoefficient;
                isCritical = true;
            }

            App.Instance.Services.LogService.Log(
                $"Critical chance: {criticalChance}, random value: {randomValue}, pure value: {damage}");

            TakeDamage(damage, isCritical: isCritical, withSectionBuffsAffect: true,
                type: AttackDamageType.Pickaxe);

            OnPickaxeHit?.Invoke(null, AttackPointHitType.Inner, AttackDamageType.Pickaxe, (int) damage,
                isCritical);
        }

        protected virtual void OnHits(Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType> attackPoints, int combo)
        {
            if (attackPoints.Count > 0)
            {
                var staticPickaxe = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe;
                var criticalChance = App.Instance.Player.Skills.Crit.Value;

                foreach (var attackPoint in attackPoints)
                {
                    var damage = MineHelper.GetCurrentPickaxeDamage();
                    var isCritical = false;

                    if (staticPickaxe.BonusCritPercent > 0)
                        criticalChance += staticPickaxe.BonusCritPercent.Value;

                    if (App.Instance.Player.Prestige > 0)
                        criticalChance += StaticHelper.GetCurrentPrestigeBuff().CriticalPercent;

                    var randomValue = Random.Range(0f, 100f);
                    if (randomValue <= criticalChance)
                    {
                        damage *= App.Instance.StaticData.Configs.Dungeon.Mines.CritDamageCoefficient;
                        isCritical = true;
                    }

                    if (attackPoint.Value == AttackPointHitType.Inner)
                        damage *= App.Instance.StaticData.Configs.Dungeon.Mines.AttackHits.InnerAccuracyCoefficient;

                    if (combo > 1)
                    {
                        var comboDamageCoefficientIndex = Mathf.Clamp(combo - 2, 0, App.Instance.StaticData.Configs.Dungeon.Mines.Combo.Count - 1);
                        var comboDamageCoefficient = App.Instance.StaticData.Configs.Dungeon.Mines.Combo[comboDamageCoefficientIndex];

                        damage *= comboDamageCoefficient;
                    }


                    App.Instance.Services.LogService.Log($"Critical chance: {criticalChance}, random value: {randomValue}, pure value: {damage}");
                    TakeDamage(damage, isCritical: isCritical, attackPoint: attackPoint.Key, withSectionBuffsAffect: true, type: AttackDamageType.Pickaxe);

                    OnPickaxeHit?.Invoke(attackPoint.Key, attackPoint.Value, AttackDamageType.Pickaxe, (int)damage, isCritical);
                }
                
                return;
            }

            var missedDamage = MineHelper.GetCurrentPickaxeDamage() * App.Instance.StaticData.Configs.Dungeon.Mines.AttackHits.MissAccuracyCoefficient;
            TakeDamage(missedDamage, isMissed: true, withSectionBuffsAffect:true, type: AttackDamageType.Pickaxe);
        }


        public override void SetReady()
        {

            base.SetReady();

            var attackArea = AttackArea as MineSceneAttackAreaCoordinatesPoints;

            if (attackArea != null)
            {
                InitializeAttackAreaPoints(attackArea);
                return;
            }
            else
            {
                InitializeAttackAreaNoPoints();
                return;
            }
            

        }

        protected override void SetPassed(float delay = MineLocalConfigs.WallSectionMoveDelay)
        {
            foreach (var buff in Buffs)
            {
                buff.Clear();
                Destroy(buff.gameObject);
            }

            Buffs.Clear();

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

        protected virtual void DropGold()
        {
            var randomGold = 0;
            var wallsCount = _selectedMine.IsLast ? 10 : 1;

            for (var i = 0; i < wallsCount; i++)
            {
                var min = _selectedTier.StaticTier.WallMoneyMin;
                var max = _selectedTier.StaticTier.WallMoneyMax + 1;
                randomGold += Random.Range(min, max);
            }

            if (randomGold <= 0)
                return;

            if (_isHardcoreMode)
                randomGold *= App.Instance.StaticData.Configs.Dungeon.Mines.Walls.HardcoreDropItemAmountCoefficient;

            var prestigeGold = 0;
            if (App.Instance.Player.Prestige > 0)
            {
                var prestigeBuff = StaticHelper.GetCurrentPrestigeBuff();
                prestigeGold = Mathf.CeilToInt(randomGold * (prestigeBuff.GoldPercent / 100));
            }

            var pickaxeGold = 0;
            var pickaxeBonusGoldPercent = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.BonusGoldPercent;
            if (pickaxeBonusGoldPercent > 0)
                pickaxeGold = Mathf.CeilToInt(randomGold * (pickaxeBonusGoldPercent.Value / 100));

            randomGold += prestigeGold + pickaxeGold;

            var dtoCurrency = new Dto.Currency(CurrencyType.Gold, randomGold);
            App.Instance.Player.Wallet.Add(dtoCurrency, IncomeSourceType.FromMining);

            MineHelper.AddDroppedCurrency(dtoCurrency);
            var viewportPosition = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, 4f));
            WindowManager.Instance.Show<WindowFlyingIcons>(withSound: false).Create(dtoCurrency, viewportPosition, Tags.InventoryButton, 0.1f);
        }

        protected virtual void DropItems()
        {
            var droppedItemAmount = App.Instance.StaticData.Configs.Dungeon.Mines.Walls.DefaultDropItemAmount;

            if (_isHardcoreMode)
                droppedItemAmount *= App.Instance.StaticData.Configs.Dungeon.Mines.Walls.HardcoreDropItemAmountCoefficient;

            var fortuneChance = App.Instance.Player.Skills.Fortune.Value;

            var pickaxeBonusFortunePercent = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.BonusFortunePercent;
            if (pickaxeBonusFortunePercent > 0)
                fortuneChance += pickaxeBonusFortunePercent.Value;

            if (App.Instance.Player.Prestige > 0)
            {
                var prestigeBuff = StaticHelper.GetCurrentPrestigeBuff();
                fortuneChance += prestigeBuff.FortunePercent;
            }

            var randomValue = Random.Range(0f, 100f);
            if (randomValue <= fortuneChance)
                droppedItemAmount *= App.Instance.StaticData.Configs.Dungeon.Mines.Walls.FortuneDropItemAmountCoefficient;

            if (_selectedMine.IsLast)
            {
                for (var i = 0; i < 10; i++)
                {
                    var randomItem = MineHelper.GetRandomWallDropItem(_selectedTier, _selectedMine);
                    DropItem(randomItem, droppedItemAmount, i * 0.1f);
                }
            }
            else
                DropItem(ItemId, droppedItemAmount);

        //    App.Instance.Services.LogService.Log($"Drop items, fortune chance: {fortuneChance}, random value: {randomValue}");
        }

        protected virtual void DropItem(string id, int amount, float delay = 0f)
        {
            var droppedItem = new Item(id, amount);
            App.Instance.Player.Inventory.Add(droppedItem, IncomeSourceType.FromMining);

            MineHelper.AddDroppedItem(droppedItem);
            var viewportPosition = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, 4f));
            WindowManager.Instance.Show<WindowFlyingIcons>(withSound: false).Create(droppedItem, viewportPosition, Tags.InventoryButton, delay);
        }

        protected virtual void DropExtraItems()
        {
            foreach (var i in _extraDrop)
            {
                if (Random.Range(0, 100) < i.Value)
                    DropItem(i.Key, 1);
            }
        }

        protected void DropPickaxeBonusItem()
        {
            var staticPickaxe = App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe;
            if (string.IsNullOrEmpty(staticPickaxe.BonusDropItemId) || staticPickaxe.BonusDropItemPercent <= 0)
                return;

            var droppedItemAmount = App.Instance.StaticData.Configs.Dungeon.Mines.Walls.DefaultDropItemAmount;

            var randomBonusItemValue = Random.Range(0f, 100f);

            var logMessage = $"Drop pickaxe bonus item, bonus chance: {staticPickaxe.BonusDropItemPercent}, random value: {randomBonusItemValue}";
            App.Instance.Services.LogService.Log(logMessage);

            if (randomBonusItemValue > staticPickaxe.BonusDropItemPercent)
                return;

            var pickaxeBonusDroppedItem = new Item(staticPickaxe.BonusDropItemId, droppedItemAmount);
            App.Instance.Player.Inventory.Add(pickaxeBonusDroppedItem, IncomeSourceType.FromMining);

            MineHelper.AddDroppedItem(pickaxeBonusDroppedItem);
            var viewportPosition = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, 4f));
            WindowManager.Instance.Show<WindowFlyingIcons>(withSound: false).Create(pickaxeBonusDroppedItem, viewportPosition, Tags.InventoryButton, 0.2f);
        }

  
    }
}