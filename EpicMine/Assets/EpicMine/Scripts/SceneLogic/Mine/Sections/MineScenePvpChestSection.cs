using System;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineScenePvpChestSection : MineSceneSection
    {
        [SerializeField] private GameObject _chestRoyal;
        [SerializeField] private GameObject _chestSimple;

        private PvpChestType _chestType;

        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            _chestType = UnityEngine.Random.Range(0, 100) < 80 ? PvpChestType.Simple : PvpChestType.Royal; // TODO CHANCE

            EventManager.Instance.Publish(new PvpArenaOnFindChest(_chestType));

            _chestSimple.SetActive(_chestType == PvpChestType.Simple);
            _chestRoyal.SetActive(_chestType == PvpChestType.Royal);
        }

        public override void SetReady()
        {
            base.SetReady();

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.ChestFound);
            SetCompleted();
        }

        private void SetCompleted()
        {
            SetPassed();
        }
    }
}