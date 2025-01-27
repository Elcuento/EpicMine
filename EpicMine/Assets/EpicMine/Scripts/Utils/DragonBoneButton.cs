using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragonBoneButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Material _material;

    [SerializeField] private Color _pressColor = Color.grey;
    [SerializeField] private Color _normalColor = Color.white;

    [SerializeField] private UnityEvent _clickAction;


    public void SetMaterial(Material material)
    {
        _material = material;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _material.DOKill();

        _material.DOBlendableColor(_pressColor, 0.2f).OnComplete(() =>{});
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _material.DOKill();

        _material.DOBlendableColor(_normalColor, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _material.DOKill();

        _clickAction?.Invoke();
        _material.DOBlendableColor(_normalColor, 0.2f);
    }
}
