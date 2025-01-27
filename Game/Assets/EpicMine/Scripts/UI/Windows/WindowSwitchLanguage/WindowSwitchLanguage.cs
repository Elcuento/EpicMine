using System;
using System.IO;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;
using UnityEngine.UI;


public class WindowSwitchLanguage : WindowBase
{
    private Action _onClose;

    [SerializeField] private Toggle _ru;
    [SerializeField] private Toggle _english;

    private SystemLanguage _language;


    public void Initialize(Action onClose)
    {
        Clear();

        _language = LocalizationHelper.GetCurrentLanguage();

        _onClose = onClose;
        _ru.isOn = _language == SystemLanguage.Russian;
        _english.isOn = _language == SystemLanguage.English;

        Filter();
    }

    public void OnToggle(bool a)
    {
        if (!a) return;

        if (_ru.isOn)
        {
            SwitchLanguage(SystemLanguage.Russian);
        }else if (_english.isOn)
        {
            SwitchLanguage(SystemLanguage.English);
        }
    }


    public void SwitchLanguage(SystemLanguage language)
    {
        if (LocalizationHelper.GetCurrentLanguage() == language)
            return;

        App.Instance.SetLanguage(language);

    }


    public void Filter()
    {
        var ruRes = _language == SystemLanguage.Russian;
        var engRes = _language == SystemLanguage.English;

        if (_ru.isOn != ruRes)
        {
            _ru.isOn = ruRes;
        }
        if (_english.isOn != engRes)
        {
            _english.isOn = ruRes;
        }

    }

    public override void OnClose()
    {
        _onClose?.Invoke();
        base.OnClose();
    }

    public void Clear()
    {
        _onClose = null;
    }
}
