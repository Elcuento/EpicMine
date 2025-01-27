using BlackTemple.EpicMine;

using TMPro;
using UnityEngine;

public class WindowRating : WindowBase
{
    [SerializeField] private TextMeshProUGUI _ratingText;


    public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
    {
        base.OnShow(withPause, withCurrencies, withRating);
        _ratingText.text = App.Instance.Player.Pvp.Rating.ToString();
    }

}
