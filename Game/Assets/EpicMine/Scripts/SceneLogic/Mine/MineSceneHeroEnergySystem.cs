using BlackTemple.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneHeroEnergySystem
    {
        public ObscuredInt Value { get; private set; }


        public void Add(int value)
        {
            Value += value;
            if (Value > App.Instance.StaticData.Configs.Dungeon.Mines.MaxEnergy)
                Value = App.Instance.StaticData.Configs.Dungeon.Mines.MaxEnergy;

            OnEnergyChange();
        }

        public bool Subtract(int value)
        {
            if (Value < value)
                return false;

            Value -= value;
            OnEnergyChange();
            return true;
        }


        private void OnEnergyChange()
        {
            EventManager.Instance.Publish(new MineSceneEnergyChangeEvent(Value));
        }
    }
}