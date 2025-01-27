using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlideHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public Action OnSliderDragRight;
    public Action OnSliderDragLeft;

    private Vector2 _startDrag;
    private bool _isDragg;

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragg)
        {
            if (_startDrag.x > eventData.scrollDelta.x)
            {
                OnSliderDragLeft?.Invoke();

                _isDragg = false;

            }
            else
            {
                OnSliderDragRight?.Invoke();

                _isDragg = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragg = true;
        _startDrag = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragg = false;
    }
}
