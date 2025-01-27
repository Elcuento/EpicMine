using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorFieldControlButtonExtended : MonoBehaviour
{

    [SerializeField] private Button _button1;
    [SerializeField] private Text _button1Text;

    [SerializeField] private Button _button2;

    public void Initialize(Action button1, string text, Action button2)
    {
        _button1.onClick.AddListener(()=> {button1?.Invoke();});
        _button1Text.text = text;

        _button2.onClick.AddListener(() => { button2?.Invoke(); });
    }
}
