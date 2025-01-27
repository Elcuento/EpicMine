using System;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneEnchantedChestSection : MineSceneSection, IPointerDownHandler
    {
        public EnchantedChestType ChestType { get; private set; }

        [SerializeField] private GameObject _amberChest;

        [SerializeField] private GameObject _rubyChest;

        [SerializeField] private GameObject _lazuriteChest;

        [SerializeField] private GameObject _malachiteChest;


        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);

            ChestType = MineHelper.GetRandomEnchantedChestType();
            switch (ChestType)
            {
                case EnchantedChestType.Amber:
                    _amberChest.SetActive(true);
                    break;
                case EnchantedChestType.Ruby:
                    _rubyChest.SetActive(true);
                    break;
                case EnchantedChestType.Lazurite:
                    _lazuriteChest.SetActive(true);
                    break;
                case EnchantedChestType.Malachite:
                    _malachiteChest.SetActive(true);
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

            _amberChest.SetActive(false);
            _rubyChest.SetActive(false);
            _lazuriteChest.SetActive(false);
            _malachiteChest.SetActive(false);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            var window = WindowManager.Instance.Show<WindowOpenEnchantedChest>(withPause: true, withCurrencies: true);

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            window.Initialize(ChestType, selectedTier.Number, OnCloseWindowChest);
        }

        private void OnCloseWindowChest()
        {
            SetPassed();
        }
    }
}