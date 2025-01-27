using System;
using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowNewDiscover : WindowBase
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _picture;

    private Action _onClose;

    public void Initialize(string id, string description, Action onClose = null)
    {
        Clear();
        _onClose = onClose;

        _description.text = LocalizationHelper.GetLocale(description);
        _picture.sprite = SpriteHelper.GetDiscoverPicture($"{id}_discover");

    }

    public override void OnClose()
    {
        base.OnClose();
        _onClose?.Invoke();
    }

    private void Clear()
    {
        _onClose = null;
        _picture.sprite = null;
    }
}
