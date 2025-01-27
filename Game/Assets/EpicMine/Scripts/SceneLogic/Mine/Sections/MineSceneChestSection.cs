using System;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneChestSection : MineSceneSection, IPointerDownHandler
    {
        public ChestType ChestType { get; private set; }

        [SerializeField] private GameObject _simpleChest;

        [SerializeField] private GameObject _royalChest;


        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            ChestType = App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ForceOpenChest)
                ? MineHelper.GetRandomChestType()
                : ChestType.Simple;

            switch (ChestType)
            {
                case ChestType.Simple:
                    _simpleChest.SetActive(true);
                    break;
                case ChestType.Royal:
                    _royalChest.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetChest(ChestType type)
        {
            switch (ChestType)
            {
                case ChestType.Simple:
                    _simpleChest.SetActive(true);
                    break;
                case ChestType.Royal:
                    _royalChest.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void SetReady()
        {
            base.SetReady();

            var foundChestCount = App.Instance.Services.RuntimeStorage.Load<int>(RuntimeStorageKeys.FoundChestCount);
            foundChestCount++;
            App.Instance.Services.RuntimeStorage.Save(RuntimeStorageKeys.FoundChestCount, foundChestCount);

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.ChestFound);
        }

        protected override void SetPassed(float delay = MineLocalConfigs.OtherSectionMoveDelay)
        {
            base.SetPassed(delay);
            _simpleChest.SetActive(false);
            _royalChest.SetActive(false);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            var window = WindowManager.Instance.Show<WindowMineChest>(withPause: true, withCurrencies: true);

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            window.Initialize(selectedTier.Number, ChestType, OnCloseWindowChest);
        }

        private void OnCloseWindowChest()
        {
            SetPassed();
        }
    }
}