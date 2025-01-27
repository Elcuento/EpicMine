using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperControlPanelToggleItem : DeveloperControlPanelItem
{

    [SerializeField] private Toggle _toggle;
    [SerializeField] private TextMeshProUGUI _label;

    public bool Status()
    {
        return _toggle.isOn;
    }

    public void Initialize(Action<bool> onChange, string label, bool defaultVal = true)
    {
        _label.text = label;
        _toggle.isOn = defaultVal;
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener((a) =>
        {
            onChange?.Invoke(a);
        });

        DontDestroyObject = true;
    }

}
