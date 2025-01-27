using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Action PointClickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        PointClickEvent?.Invoke();
    }
}
