using BlackTemple.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine.Utils
{
    public class ScrollViewEventHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public event EventHandler OnDragEnded;
        public event EventHandler OnDragBegin;

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragBegin?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnded?.Invoke();
        }
    }
}