using System.Collections;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowTorchesMerchantItem : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _pvpSmallHeader;
    [SerializeField] private PvpHeaderPanel _pvpHeaderPanel;

    [SerializeField] private Image _backGround;
    [SerializeField] private Image _backGroundShadow;
    [SerializeField] private RectTransform _backGroundRec;
    [SerializeField] private RectTransform _backGroundShadowRec;

    public void Initialize(int torLeague, string text, int amount)
    {
        _pvpHeaderPanel.Initialize(torLeague  - 1, text);
        _pvpSmallHeader.SetText(App.Instance.StaticData.Leagues[torLeague - 1].Rating + "+");
        _backGround.color = App.Instance.ReferencesTables.Colors.LeagueBackGroundColors[torLeague-1];
        _backGroundShadow.color = App.Instance.ReferencesTables.Colors.LeagueBackGroundColors[torLeague-1];
         LocalizationHelper.GetLocale("league_" + torLeague);

         if (amount > 5)
         {
             _backGroundRec.sizeDelta = new Vector2(0,677);
             _backGroundShadowRec.sizeDelta = new Vector2(0, 677);
         }

    }
    
}
