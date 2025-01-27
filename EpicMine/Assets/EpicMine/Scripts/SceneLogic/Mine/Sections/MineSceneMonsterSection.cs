using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

public class MineSceneMonsterSection : MineSceneAttackSection
{
    private ISectionMonsterFactory _monsterFactory;
    public Monster StaticMonster { get; private set; }

    public MineSceneMonster Monster { get; private set; }


    public void Start()
    {
        SectionType = SectionType.Monster;
    }

    public void SetMonsterId(string id)
    {
        StaticMonster = App.Instance.StaticData.Monsters
            .Find(x => x.Id == id); 
    }

    public override void Initialize(int number, MineSceneHero hero)
    {
        _monsterFactory = new DefaultSectionMonsterFactory(_itemContainer);
      
        base.Initialize(number, hero);
    }

    protected override void InitializeItem()
    {
        ItemId = StaticMonster.Id;

        Monster = _monsterFactory.CreateMonster(StaticMonster.Type, StaticMonster.Id);
        Monster.Initialize(StaticMonster, Hero, this, OnAttack);
        Item = Monster.gameObject;
    }

    protected override void InitializeHealth()
    {
        var staticWalls = App.Instance.StaticData.MineWalls;

        var health = Random.Range(StaticMonster.HealthMin * (1 - StaticMonster.HealthMaxPercentOffset),
            StaticMonster.HealthMin * (1 + StaticMonster.HealthMaxPercentOffset ));

        if (_isHardcoreMode)
        {

            if (Number <= staticWalls.Count - 1)
            {
                var wall = staticWalls[Number];
                health = wall.HealthCoefficient * health;
            }
            else
            {
                var additionalCoefficient = Mathf.Pow(1.1f, Number - (staticWalls.Count - 1));
                health = health * additionalCoefficient;
            }

            if (App.Instance.Player.Prestige > 0)
            {
                for (var i = 1; i <= App.Instance.Player.Prestige; i++)
                {
                    var buff = StaticHelper.GetPrestigeBuff(i);
                    health += health * (buff.WallHealthPercent / 100);
                }
            }

            health *= App.Instance.StaticData.Configs.Dungeon.Mines.Monsters.HardcoreHealthCoefficient;
        }

        Health = HealthMax = health;
    }

    public override void TakeDamage(float value, AttackDamageType type, bool isCritical, bool withSectionBuffsAffect, bool withHeroBuffsAffect)
    {
        var health = Health;

        var damage = MonstersHelper.CalculateDamageOnMonster(StaticMonster, type, value);

        if (damage == 0)
        {
            EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, null, damage, isCritical, false, type, true));
            return;
        }

        base.TakeDamage(damage, type, isCritical: isCritical, withSectionBuffsAffect : withSectionBuffsAffect, withHeroBuffsAffect:withHeroBuffsAffect);


        if (type == AttackDamageType.Item || type == AttackDamageType.Ability || type == AttackDamageType.FireAbility)
            DotWeenHelper.ShakeWall(Camera.main, Item, type);

        if (Health > 0)
        {
            var healsLeft = health - Health;
            Monster.Damage(healsLeft, type);
        }
    }

    public override void TakeDamage(float value, AttackDamageType type, bool isCritical = false, bool isMissed = false,
        MineSceneAttackPoint attackPoint = null, bool withSectionBuffsAffect = true, bool withHeroBuffsAffect = true)
    {
        var health = Health;

        if (Health <= 0)
            return;

        var damage = MonstersHelper.CalculateDamageOnMonster(StaticMonster, type, value);

        if (damage == 0 || isMissed)
        {
            EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, attackPoint, damage, isCritical, false, type, true));
            return;
        }

        var frostDamage = 0f;

        if (withSectionBuffsAffect)
        {
            var existBuff = Buffs?.FirstOrDefault(b => b is MineSceneSectionFreezingBuff);
            if (existBuff != null)
            {
                var freezingBuff = (MineSceneSectionFreezingBuff)existBuff;
                var torch = App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch;
                frostDamage = (App.Instance.Player.Abilities.Freezing.StaticLevel.Damage + (torch.FreezingAdditionalParameter ?? 0)) * (freezingBuff.Stacks);

                frostDamage = MonstersHelper.CalculateDamageOnMonster(StaticMonster, AttackDamageType.FrostAbility,
                    frostDamage);
            }
        }

        if (withHeroBuffsAffect)
        {
            var damagePotionBuff = Hero.Buffs?.FirstOrDefault(b => b is MineSceneHeroDamagePotionBuff);
            if (damagePotionBuff != null)
            {
                var heroDamagePotionBuff = (MineSceneHeroDamagePotionBuff)damagePotionBuff;
                damage += damage / 100f * heroDamagePotionBuff.Potion.Value;
            }
        }

        Health -= damage + frostDamage;

        EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, attackPoint, damage, isCritical, false, type));

        if (frostDamage > 0)
            EventManager.Instance.Publish(new MineSceneWallSectionDamageEvent(this, null, frostDamage, false, false, AttackDamageType.FrostAbility));

        if (type == AttackDamageType.Item || type == AttackDamageType.Ability || type == AttackDamageType.FireAbility)
            DotWeenHelper.ShakeWall(Camera.main, Item, type);

        if (Health <= 0)
            DestroySection();
        else
        {
            var healthLeft = health - Health;
            Monster.Damage(healthLeft, type);
        }
    }

    protected override void OnHit()
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

        damage = (int) (damage * (1 + MineLocalConfigs.PickaxeMonsterDamageCoefficient));
        damage = damage <= 0 ? 1 : damage;
        App.Instance.Services.LogService.Log($"Critical chance: {criticalChance}, random value: {randomValue}, pure value: {damage}");
        TakeDamage(damage, isCritical: isCritical, withSectionBuffsAffect: true, type: AttackDamageType.Pickaxe, isMissed:Monster.IsAway);

    }

    public override void RemoveBuff(AbilityType abilityType)
    {
        Monster.RemoveBuff(abilityType);
        
        base.RemoveBuff(abilityType);
    }

    public override bool AddBuff(AbilityType abilityType)
    {
        if (Health > 0)
        {
            Monster.AddBuff(abilityType);
        }
        return base.AddBuff(abilityType);
    }

    public void OnAttack(float damage)
    {
        Hero.Pickaxe.RemoveHealth((int)damage);
    }

    protected override void SetPassed(float delay = MineLocalConfigs.WallSectionMonsterDelay)
    {
        foreach (var buff in Buffs)
        {
            buff.Clear();
            Destroy(buff.gameObject);
        }

        Buffs.Clear();

        DropGold();
        DropItems();
        DropPickaxeBonusItem();
        DropExtraItems();

        base.SetPassed(delay);

        Monster.Death();
    }



    public override void SetReady()
    {
        base.SetReady();

        if (App.Instance.Player.Features.Exist(FeaturesType.TorchAbility))
        {
            if (!App.Instance.Player.AdditionalInfo.IsFourEnergyAbilityWindowShowed)
            {
                App.Instance.Player.AdditionalInfo.IsFourEnergyAbilityWindowShowed = true;

                WindowManager
                    .Instance
                    .Show<WindowNewEnergyAbility>()
                    .Initialize(AbilityType.Torch);
            }
        }
    }

    protected override void DropItems()
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

        var randomItems = MineHelper.GetRandomMonsterDropItems(StaticMonster);

        if (randomItems.Count == 0)
            return;

        foreach (var item in randomItems)
        {
            var count = Random.Range(item.Count, item.CountMax) * droppedItemAmount;

            DropItem(item.Id, count);
        }
    

        App.Instance.Services.LogService.Log($"Drop items, fortune chance: {fortuneChance}, random value: {randomValue}");
    }

    protected override void DropGold()
    {
        var dropGold = MineHelper.GetRandomMonsterDropCurrency(StaticMonster)
            .FirstOrDefault(x=>x.Type == EntityType.CurrencyGold);

        var randomGold =  dropGold == null ? 0 : Random.Range(dropGold.Count, dropGold.CountMax);
       

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

        var dtoCurrency = new BlackTemple.EpicMine.Dto.Currency(CurrencyType.Gold, randomGold);
        App.Instance.Player.Wallet.Add(dtoCurrency, IncomeSourceType.FromMining);

        MineHelper.AddDroppedCurrency(dtoCurrency);
        var viewportPosition = Camera.main.WorldToViewportPoint(transform.position + new Vector3(0, 4f));
        WindowManager.Instance.Show<WindowFlyingIcons>(withSound: false).Create(dtoCurrency, viewportPosition, Tags.InventoryButton, 0.1f);
    }


    public override void SetAppear()
    {
        base.SetAppear();
        Monster.Appear();
    }


    public void EditorSetAttributes(MineSceneDeveloperPanel.MonsterExtraAttributes attributes)
    {
        return;
        // health
        var health =
            Random.Range(StaticMonster.HealthMin * (1 - StaticMonster.HealthMaxPercentOffset),
                StaticMonster.HealthMin * (1 + StaticMonster.HealthMaxPercentOffset)) *
            (1 + attributes.HealthPercent);

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

        EventManager.Instance.Publish(new MineSceneWallSectionHealEvent(this));
    }
}
