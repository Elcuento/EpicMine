using System;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;

public class WindowShopEtcHeaderItem : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI _headerLabel;

    public void Initialize(string label)
    {
        _headerLabel.text = label;
    }

}
