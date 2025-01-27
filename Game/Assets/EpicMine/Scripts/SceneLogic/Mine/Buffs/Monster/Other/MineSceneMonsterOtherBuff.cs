using System;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneMonsterOtherBuff : MineSceneMonsterBuff
    {
        [SerializeField] private GameObject _executeParticle;

        private MonsterAbility _staticBuff;
        private MineSceneHero _target;

        private float _duration;

        private float _timer;

        public void Initialize(MonsterAbility ability, MineSceneMonsterSection monster, MineSceneHero hero)
        {
            base.Initialize(ability, monster);

            
            _staticBuff = ability;
            _duration = ability.Duration;
            _target = hero;

            Execute();
        }

        private void Update()
        {
            if (_duration <= 1)
                return;

            if(_timer >= 1)
                Execute();

            _timer += Time.deltaTime;
        }

        private void Execute()
        {
            foreach (var chars in _staticBuff.Characteristics)
            {
                switch (chars.Key)
                {
                    case MonsterAbilityCharacteristicType.Health:

                        if (chars.Value > 0)
                        {
                            if(_staticBuff.Percent == 1)
                            _target.Pickaxe.AddHealth((int) (chars.Value * _target.Pickaxe.Health) );
                            else _target.Pickaxe.AddHealth((int)(chars.Value ));
                        }
                        else
                        {
                            if (_staticBuff.Percent == 1)
                                _target.Pickaxe.RemoveHealth((int)(-chars.Value * _target.Pickaxe.Health));
                            else _target.Pickaxe.RemoveHealth((int)(-chars.Value));
                        }
                        break;
                    case MonsterAbilityCharacteristicType.Energy:


                        if (chars.Value > 0)
                        {
                            if (_staticBuff.Percent == 1)
                                _target.EnergySystem.Add((int)(chars.Value * _target.Pickaxe.Health));
                            else _target.EnergySystem.Add((int)(chars.Value));
                        }
                        else
                        {
                            if (_staticBuff.Percent == 1)
                                _target.EnergySystem.Subtract((int)(-chars.Value * _target.Pickaxe.Health));
                            else _target.EnergySystem.Subtract((int)(-chars.Value));
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_executeParticle != null)
            {
                _executeParticle.transform.SetParent(_target.transform);
                _executeParticle.transform.localPosition = new Vector3(0.176f, 4.09f, -2.57f);
                _executeParticle.SetActive(true);
            }

            _duration -= 1;

            if (_duration <= 0)
            {
                Remove();
            }
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}