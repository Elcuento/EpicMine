using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class MineSceneHero : MonoBehaviour
    {
        public MineSceneHeroEnergySystem EnergySystem { get; private set; }

        public MineSceneSection CurrentSection;

        public MineScenePickaxe Pickaxe => _pickaxe;

        public MineSceneTorch Torch => _torch;

        public List<MineSceneHeroBuff> Buffs { get; private set; }

        [SerializeField] private MineSceneBaseController _sceneController;

        [SerializeField] private MineScenePickaxe _pickaxe;

        [SerializeField] private MineSceneTorch _torch;

        [SerializeField] private MineSceneHeroMoveController _moveController;

        [SerializeField] private Color _damagePotionVignetteColor;

        [SerializeField] private Color _healthPotionVignetteColor;

        [SerializeField] private Color _energyPotionVignetteColor;

        private IHeroBuffFactory _buffFactory;

        public void MoveForward(int sectionsCount)
        {
            var newPosition = MineLocalConfigs.SectionSize * ((CurrentSection != null ? CurrentSection.Number : 0) + sectionsCount);
            _moveController.Move(newPosition, MineLocalConfigs.SectionMoveTime * sectionsCount);
        }

        public void MoveToNextNotEmptySection()
        {
            var nextSection = _sceneController
                .SectionProvider
                .Sections
                .FirstOrDefault(s => s.Number > CurrentSection.Number);

            if (nextSection == null)
                return;

            nextSection.SetAppear();
            EventManager.Instance.Publish(new MineSceneSectionAppearEvent(nextSection));

            MoveNext(nextSection);
        }

        private void MoveNext(MineSceneSection destination)
        {
            var newPosition = MineLocalConfigs.SectionSize * ((CurrentSection != null ? CurrentSection.Number : 0) + 1);
            var duration = MineLocalConfigs.SectionMoveTime;

            CurrentSection?.SetExit();

            _moveController.Move(
                newPosition,
                duration,
                () => {
                    CurrentSection = destination;
                    destination.SetReady();

                    if (!destination.IsPassed)
                    EventManager.Instance.Publish(new MineSceneHeroMoveEvent(false));
                    
                });
        }

        public bool ApplyAbility(AbilityType abilityType)
        {
            var currentSection = _sceneController
                .SectionProvider.Sections
                .FirstOrDefault(s => s.IsReady && !s.IsPassed && s.Number == CurrentSection.Number);

            if (currentSection == null)
                return false;

            if (!currentSection.AddBuff(abilityType))
                return false;

            if (abilityType == AbilityType.Acid)
            {
                var existBuff = Buffs.FirstOrDefault(b => b is MineSceneHeroAcidBuff);
                if (existBuff != null)
                {
                    var existAcidBuff = (MineSceneHeroAcidBuff) existBuff;
                    existAcidBuff.UpdateTime();
                }
                else
                {
                    var buff = _buffFactory.CreateAcidBuff();
                    Buffs.Add(buff);
                }

                FireBuffsChangeEvent();
            }

            return true;
        }

        public bool UseAbility(AbilityType abilityType)
        {
            int energyCost;

            switch (abilityType)
            {
                case AbilityType.ExplosiveStrike:
                    energyCost = App.Instance.Player.Abilities.ExplosiveStrike.StaticLevel.EnergyCost;
                    break;
                case AbilityType.Freezing:
                    energyCost = App.Instance.Player.Abilities.Freezing.StaticLevel.EnergyCost;
                    break;
                case AbilityType.Acid:
                    energyCost = App.Instance.Player.Abilities.Acid.StaticLevel.EnergyCost;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }

            if (EnergySystem.Value < energyCost)
                return false;


            if (!ApplyAbility(abilityType))
                return false;

            EnergySystem.Subtract(energyCost);
            return true;
        }

        public void UseItem(string itemId)
        {
            var dtoItem = new Item(itemId, 1);
            if (!App.Instance.Player.Inventory.Has(dtoItem))
                return;

            if (StaticHelper.IsTypeOf(itemId, typeof(Tnt)))
            {
                if (CurrentSection != null)
                {
                    var wallSection = CurrentSection as MineSceneAttackSection;
                    if (wallSection != null)
                    {
                        var tnt = App.Instance.StaticData.Tnt.FirstOrDefault(t => t.Id == itemId);

                        if (tnt != null)
                        {
                            wallSection.AddBuff(AbilityType.Tnt);
                            App.Instance.Player.Inventory.Remove(dtoItem, SpendType.Using);

                            var randomTntSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.Tnts.Length);
                            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Tnts[randomTntSoundIndex]);
                        }
                    }
                }
                return;
            }

            if (StaticHelper.IsTypeOf(itemId, typeof(Potion)))
            {
                var potion = App.Instance.StaticData.Potions.FirstOrDefault(p => p.Id == itemId);

                if (potion != null)
                {
                    App.Instance.Player.Inventory.Remove(dtoItem, SpendType.Using);
                    Color vignetteColor;

                    switch (potion.Type)
                    {
                        case PotionType.Damage:
                            AddBuff(typeof(MineSceneHeroDamagePotionBuff));
                            vignetteColor = _damagePotionVignetteColor;
                            break;
                        case PotionType.Health:
                            Pickaxe.AddHealth((int)potion.Value);
                            vignetteColor = _healthPotionVignetteColor;
                            break;
                        case PotionType.Energy:
                            EnergySystem.Add((int)potion.Value);
                            vignetteColor = _energyPotionVignetteColor;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    WindowManager.Instance.Show<WindowVignette>()
                        .Initialize(vignetteColor);

                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Potion);
                }
            }
        }

        public void AddBuff(Type type)
        {
            if (typeof(MineSceneHeroDamagePotionBuff) == type)
            {
                var existBuff = Buffs.FirstOrDefault(b => b is MineSceneHeroDamagePotionBuff);

                if (existBuff != null)
                {
                    var existDamagePotionBuff = (MineSceneHeroDamagePotionBuff) existBuff;
                    existDamagePotionBuff.UpdateTime();
                }
                else
                {
                    var buff = _buffFactory.CreateDamagePotionBuff();
                    Buffs.Add(buff);
                }

                FireBuffsChangeEvent();
            }
        }


        public void RemoveBuff(MineSceneHeroBuff buff)
        {
            var existBuff = Buffs.FirstOrDefault(b => b == buff);
            if (existBuff != null)
            {
                existBuff.Clear();
                Buffs.Remove(existBuff);
                Destroy(existBuff.gameObject);
                FireBuffsChangeEvent();
            }
        }


        private void Awake()
        {
            EnergySystem = new MineSceneHeroEnergySystem();
            Buffs = new List<MineSceneHeroBuff>();
            _buffFactory = new DefaultHeroBuffFactory(this);

            EventManager.Instance.Subscribe<MineSceneWallSectionHitEvent>(OnSectionHit);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPass);
        }

        private void Start()
        {
            if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.DamagePotionTimeLeft))
            {
                Buffs.Add(_buffFactory.CreateDamagePotionBuff());
                FireBuffsChangeEvent();
            }

            if (App.Instance.Services.RuntimeStorage.IsDataExists(RuntimeStorageKeys.Energy))
            {
                var val = App.Instance.Services.RuntimeStorage.Load<int>(RuntimeStorageKeys.Energy);
                EnergySystem.Add(val);
            }

            if (App.Instance.Player.Prestige > 0)
            {
                var prestigeBuff = _buffFactory.CreatePrestigeBuff();
                Buffs.Add(prestigeBuff);
                FireBuffsChangeEvent();
            }
        }

        public void Initialize(MineSceneSection startSection)
        {
            CurrentSection = startSection;
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<MineSceneWallSectionHitEvent>(OnSectionHit);

            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPass);
        }


        private void OnSectionPass(MineSceneSectionPassedEvent eventData)
        {
            if (_pickaxe.Health >= App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth)
                return;

            if (Random.Range(0, 100) > 60)
                return;

            var isMonsterSection = eventData.Section is MineSceneMonsterSection;
            var isWallSection = eventData.Section is MineSceneWallSection;

            if (isMonsterSection || isWallSection)
            {
                var randomX = Random.Range(-1, 1);
                var randomY = Random.Range(-1, 2);

                var amount = 1;

                var viewportPosition = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x + randomX, transform.position.y + randomY + 4,
                    transform.position.z));
                var windowFlyingIcons = WindowManager.Instance.Show<WindowFlyingIcons>();

                windowFlyingIcons.Create(
                    App.Instance.ReferencesTables.Sprites.HeartIcon,
                    $"+{amount}",
                    viewportPosition,
                    Tags.PlayerHealthBar);

                _pickaxe.AddHealth(amount);
            }
        }

        private void OnSectionHit(MineSceneWallSectionHitEvent eventData)
        {

            var isMonsterSection = eventData.Section is MineSceneMonsterSection;
            var isWallSection = eventData.Section is MineSceneWallSection;
            var isPvpWallSection = eventData.Section is MineScenePvpWallSection;


            if (!eventData.IsMiss)
            {
                if (isMonsterSection)
                {
                    var randomX = Random.Range(-1, 1);
                    var randomY = Random.Range(-1, 2);

                    SpawnEnergyWordIcon(
                        new Vector3(transform.position.x + randomX, transform.position.y + randomY + 4,
                            transform.position.z), 1);

                    _pickaxe.Hit(eventData.Section.transform.position);
                }

                if (isPvpWallSection || isWallSection)
                {
                    if (Random.Range(0, 100) > 50)
                    {
                        var randomX = Random.Range(-1, 1);
                        var randomY = Random.Range(-1, 2);

                        SpawnEnergyWordIcon(
                            new Vector3(transform.position.x + randomX, transform.position.y + randomY + 4,
                                transform.position.z), 1);

                        _pickaxe.Hit(eventData.Section.transform.position);

                        //   _pickaxe.Hit(attackPoint.Key.transform.position);
                    }
                }
                else
                {
                    foreach (var attackPoint in eventData.AttackPoints)
                    {
                        if (attackPoint.Key.PointType == AttackPointType.Health)
                        {
                            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.HealthAttackPointHit);

                            var amount = 1;
                            var mainViewportPosition =
                                Camera.main.WorldToViewportPoint(attackPoint.Key.transform.position);
                            var windowFlyingIcons = WindowManager.Instance.Show<WindowFlyingIcons>();

                            windowFlyingIcons.Create(
                                App.Instance.ReferencesTables.Sprites.HeartIcon,
                                $"+{amount}",
                                mainViewportPosition,
                                Tags.PlayerHealthBar);

                            Pickaxe.AddHealth(amount);
                        }
                        else if (attackPoint.Key.PointType == AttackPointType.Energy)
                        {
                            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.EnergyAttackPointHit);
                            SpawnEnergyWordIcon(attackPoint.Key.transform.position, 5);
                        }

                        _pickaxe.Hit(attackPoint.Key.transform.position);

                    }

                    if (eventData.AttackPoints.Count > 0 && eventData.Combo > 1)
                        SpawnEnergyWordIcon(eventData.AttackPoints.Last().Key.transform.position,
                            eventData.AttackPoints.Count);
                }

            }


            Pickaxe.Dig(eventData.IsMiss, !isMonsterSection);
        }


        public void SpawnEnergyWordIcon(Vector3 position, int amount)
        {
            var viewportPosition = Camera.main.WorldToViewportPoint(position);
            var windowFlyingIcons = WindowManager.Instance.Show<WindowFlyingIcons>();
            windowFlyingIcons.Create(
                App.Instance.ReferencesTables.Sprites.EnergyIcon,
                $"+{amount}",
                viewportPosition,
                Tags.PlayerEnergyBar);

            EnergySystem.Add(amount);
        }

        private void FireBuffsChangeEvent()
        {
            EventManager.Instance.Publish(new MineSceneHeroBuffsChangeEvent(this));
        }
    }
}