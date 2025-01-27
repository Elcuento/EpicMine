using System.Collections;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPvpArenaPreviewView : MonoBehaviour
{
    [SerializeField] private Image _picture;
    [SerializeField] private TextMeshProUGUI _ratingNeed;
    [SerializeField] private TextMeshProUGUI _number;
    [SerializeField] private TextMeshProUGUI _name;

    [SerializeField] private GameObject _lock;

    public void Initialize(Sprite picture, string ratingNeed, string number, bool isLocked)
    {
        _picture.sprite = picture;
        _ratingNeed.text = ratingNeed;
        _name.text = LocalizationHelper.GetLocale("leagueArena_" + number);
        _number.text = LocalizationHelper.GetLocale("arena") + $" {number}";
        _lock.SetActive(isLocked);
    }
}
