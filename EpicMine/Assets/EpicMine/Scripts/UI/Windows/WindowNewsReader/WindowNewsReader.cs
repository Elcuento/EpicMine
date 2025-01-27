using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

public class WindowNewsReader : WindowBase
{
    [SerializeField] private Transform _newsContainer;
   
    [SerializeField] private WindowNewsReaderItem _newsPrefab;

    public void Initialize(List<News> news)
    {
        if (news == null)
            return;

        _newsContainer.ClearChildObjects();

        var countryCode = LocalizationHelper.ToCountryCode(LocalizationHelper.GetSystemLanguage());
        var platform = App.Instance.CurrentPlatform.ToString();

        foreach (var ne in news)
        {
            if(ne.Language != countryCode || (ne.Platform != platform && ne.Platform != PlatformType.All.ToString()))
                continue;

            var pref = Instantiate(_newsPrefab, _newsContainer, false);
            pref.Initialize(ne);
        }
    }
}

