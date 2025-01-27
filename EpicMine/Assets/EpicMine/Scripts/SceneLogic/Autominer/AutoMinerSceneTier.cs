using System;
using BlackTemple.EpicMine;
using UnityEngine;
using UnityEngine.UI;

public class AutoMinerSceneTier : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _selectedBorder;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _locked;

    [SerializeField] private GameObject _line;

    private Action<int, bool> _onClick;

    public BlackTemple.EpicMine.Core.Tier Tier { get; private set; }


    public void Initialize(BlackTemple.EpicMine.Core.Tier tier, Action<int, bool> onClick)
    {
        Tier = tier;

        if (Tier.IsOpen)
            SetOpen();
        else
        {
            SetClose();
        }

        if(tier.IsLast)
            _line.gameObject.SetActive(false);

        _onClick = onClick;
    }

    private void SetClose()
    {
        _locked.SetActive(true);
        _icon.gameObject.SetActive(false);
        _icon.sprite = App.Instance.ReferencesTables.Sprites.MineIncompleteIcon;
    }
    private void SetOpen()
    {
        _locked.SetActive(false);
        _icon.gameObject.SetActive(true);
        _icon.sprite = App.Instance.ReferencesTables.Sprites.MineCompleteIcon;
    }

    public void FadeIn()
    {
        _canvasGroup.alpha = 1f;
    }

    public void FadeOut()
    {
        _canvasGroup.alpha = 0.5f;
    }

    public void RemoveSelection()
    {
        _selectedBorder.SetActive(false);
    }

    public void SetSelection()
    {
        _selectedBorder.SetActive(true);
    }

    public void OnClick()
    {
        _onClick?.Invoke(Tier.Number, false);
    }

}
