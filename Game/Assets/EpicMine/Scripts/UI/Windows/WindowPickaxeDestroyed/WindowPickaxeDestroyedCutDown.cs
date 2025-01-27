using System;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowPickaxeDestroyedCutDown : WindowBase
    {
        [SerializeField] private ItemView _itemPrefab;
        [SerializeField] private Transform _windowStatsItemsContainer;

        private Action _onRestart;


        public void Initialize(int score, Core.Mine mine, Action onRestart = null)
        {
            _onRestart = onRestart;
        }

        public void Restart()
        {
            _onRestart?.Invoke();

            MineHelper.ClearTempStorage(withClearSelectedMine: false);
            Close();
        }
        

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeDestroyedWindow);


            var lastMineDroppedItems = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<string, int>>(RuntimeStorageKeys.LastMineDroppedItems);

            var lastMineDroppedCurrencies = App.Instance
                .Services
                .RuntimeStorage
                .Load<Dictionary<CurrencyType, int>>(RuntimeStorageKeys.LastMineDroppedCurrencies);

         //   var healthRefillCount = App.Instance.Services.RuntimeStorage.Load<int>(RuntimeStorageKeys.HealthRefillCount);

            if (lastMineDroppedItems != null)
            {
                foreach (var droppedItem in lastMineDroppedItems)
                {
                    var item = Instantiate(_itemPrefab, _windowStatsItemsContainer, false);
                    var dtoItem = new Item(droppedItem.Key, droppedItem.Value);
                    item.Initialize(dtoItem);
                }
            }

            if (lastMineDroppedCurrencies != null)
            {
                foreach (var droppedCurrency in lastMineDroppedCurrencies)
                {
                    var item = Instantiate(_itemPrefab, _windowStatsItemsContainer, false);
                    var dtoCurrency = new Dto.Currency(droppedCurrency.Key, droppedCurrency.Value);
                    item.Initialize(dtoCurrency);
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            _windowStatsItemsContainer.ClearChildObjects();
        }
    }
}