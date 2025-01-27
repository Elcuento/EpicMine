using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorFieldExtendedContainer : MonoBehaviour {

    public Text Label;
    public Transform Container;
    public InputField Input;

    public void Initialize(string text, Action<string> action)
    {
        Label.text = text;
        Input.onValueChanged.RemoveAllListeners();
        Input.onValueChanged.AddListener((a) =>
        {
            action?.Invoke(a);
        });
    }
}
