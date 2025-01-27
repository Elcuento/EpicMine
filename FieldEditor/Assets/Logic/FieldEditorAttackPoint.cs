using System;
using System.Collections.Generic;
using BlackTemple.EpicMine.Core;
using UnityEngine;
using UnityEngine.EventSystems;

using Image = UnityEngine.UI.Image;

public class FieldEditorAttackPoint : MonoBehaviour, IPointerClickHandler
{
    private Action<FieldEditorAttackPoint, PointerEventData.InputButton> _onClick;

    [SerializeField] private Image _picture;

    [SerializeField] public List<Image> _links;

    public FieldAttackPoint Point { get; private set;  }

    public bool IsMain;

    public List<FieldEditorAttackPoint> CombinePoint = new List<FieldEditorAttackPoint>();

    private FieldEditorController _controller;


    public void Initialize(FieldEditorController controller, FieldAttackPoint data, Action<FieldEditorAttackPoint, PointerEventData.InputButton> onClick)
    {
        _controller = controller;
        Point = data;
        Refresh();
        _onClick = onClick;
    }

    public void Combine(List<FieldEditorAttackPoint> points, bool isMain = false)
    {
        IsMain = isMain;

        CombinePoint = points;
        if(!IsMain)
            Point.SetType(AttackPointType.Empty);
        Refresh();
    }

    public void UnCombine()
    {
        IsMain = false;
        if (CombinePoint.Count <= 0)
            return;

        CombinePoint.Remove(this);
        Refresh();    
    }

    public void SetLink(AttackPointLinkType linkType, Color color)
    {
        switch (linkType)
        {
            case AttackPointLinkType.Left:
                _links[0].gameObject.SetActive(true);
                _links[0].color = color;
                break;
            case AttackPointLinkType.Right:
                _links[1].gameObject.SetActive(true);
                _links[1].color = color;
                break;
            case AttackPointLinkType.Up:
                _links[2].gameObject.SetActive(true);
                _links[2].color = color;
                break;
            case AttackPointLinkType.Down:
                _links[3].gameObject.SetActive(true);
                _links[3].color = color;
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClick?.Invoke(this, eventData.button);

        Refresh();
    }

    public void Refresh()
    {
        if (CombinePoint.Contains(this) && !IsMain)
        {
            _picture.enabled = false;
            return;
        }
        else _picture.enabled = true;

        var sizeRatio = _controller.GetFieldScaleSizeRatio();
        _picture.GetComponent<RectTransform>().localScale = new Vector3(Point.Size * sizeRatio, sizeRatio * Point.Size);

        switch (Point.PointType)
        {
            case AttackPointType.Empty:
                _picture.color = Color.white;
                break;
            case AttackPointType.Default:
                _picture.color = Color.green;
                break;
            case AttackPointType.Health:
                _picture.color = Color.red;
                break;
            case AttackPointType.Energy:
                _picture.color = Color.yellow;
                break;
            case AttackPointType.Random:
                _picture.color = Color.grey;
                break;

        }
    }
}
