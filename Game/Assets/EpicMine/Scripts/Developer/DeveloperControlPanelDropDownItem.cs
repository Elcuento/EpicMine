using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperControlPanelDropDownItem : DeveloperControlPanelItem
{
    
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private TextMeshProUGUI _label;

    public int GetValue()
    {
        return _dropdown.value;
    }

    public void Initialize(Action<int> onChange, string label, List<string> options, int value = 0)
    {
        _label.text = label;

        _dropdown.options = new List<TMP_Dropdown.OptionData>();

        foreach (var t in options)
        {
            _dropdown.options.Add(new TMP_Dropdown.OptionData(name = t));
        }

        _dropdown.onValueChanged.AddListener((a) =>
        {
            onChange?.Invoke(a);
        });

        _dropdown.value = value;
        _dropdown.RefreshShownValue();

        DontDestroyObject = true;
    }

}
