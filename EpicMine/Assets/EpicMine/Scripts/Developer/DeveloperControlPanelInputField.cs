using System;
using TMPro;
using UnityEngine;

public class DeveloperControlPanelInputField : DeveloperControlPanelItem
{

   [SerializeField] private TMP_InputField _inputField;

    public string GetText()
    {
        return _inputField.text;
    }

    public void SetText(string text)
    {
        _inputField.text = text;
    }

    public void SetTextColor(Color col)
    {
        _inputField.textComponent.color = col;
    }

    public void Initialize(Action<string> onChange, string label)
    {
        _inputField.text = label;
        _inputField.onValueChanged.RemoveAllListeners();
        _inputField.onValueChanged.AddListener((a) =>
        {
            onChange?.Invoke(a);
        });

        DontDestroyObject = true;
    }

}
