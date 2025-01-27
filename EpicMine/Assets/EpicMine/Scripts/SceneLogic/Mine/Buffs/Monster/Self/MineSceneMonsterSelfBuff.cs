using System;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneMonsterSelfBuff : MineSceneMonsterBuff
    {
        private MonsterAbility _staticBuff;

        private float _duration;

        private float _timer;

        public override void Initialize(MonsterAbility ability, MineSceneMonsterSection monster)
        {
            base.Initialize(ability, monster);


            _staticBuff = ability;
            _duration = ability.Duration;

            Execute();
        }

        private void Update()
        {
            if (_timer >= 1)
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
                            _monsterSection.Heal(chars.Value * _monsterSection.HealthMax);
                            else _monsterSection.Heal(chars.Value);
                        }
                        {
                            _monsterSection.TakeDamage(Math.Abs(chars.Value), AttackDamageType.Ability);
                        }

                        break;
                    case MonsterAbilityCharacteristicType.Energy:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _duration -= 1;
            _timer = 0;

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