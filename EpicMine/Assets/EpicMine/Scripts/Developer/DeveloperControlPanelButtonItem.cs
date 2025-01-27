using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperControlPanelButtonItem : DeveloperControlPanelItem {

    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _label;


    public string GetText()
    {
        return _label.text;
    }

    public void Initialize(Action<DeveloperControlPanelButtonItem> onClick, string label)
    {
        _label.text = label;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            onClick?.Invoke(this);
        });
    }

    public void Initialize(Action onClick, string label)
    {
        _label.text = label;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
           
            onClick?.Invoke();
        });
    }
}
