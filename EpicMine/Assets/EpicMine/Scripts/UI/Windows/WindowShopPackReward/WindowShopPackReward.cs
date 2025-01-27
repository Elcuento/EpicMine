using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using TMPro;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;

public class WindowShopPackReward : WindowBase {

    [SerializeField] private TextMeshProUGUI _header;

    [Space]
    [SerializeField] private ItemView _itemPrefab;
    [SerializeField] private Transform _itemsContainer;


    public void Initialize(List<Item> items, List<Currency> currency)
    {
        Clear();

        foreach (var item in items)
        {
            var itemView = Instantiate(_itemPrefab, _itemsContainer, false);
            itemView.Initialize(item);
        }
        foreach (var item in currency)
        {
            var itemView = Instantiate(_itemPrefab, _itemsContainer, false);
            itemView.Initialize(item);
        }
    }

    public void Clear()
    {
        _itemsContainer.ClearChildObjects();
    }

    public void OnClickOk()
    {
        Close();
    }
}
