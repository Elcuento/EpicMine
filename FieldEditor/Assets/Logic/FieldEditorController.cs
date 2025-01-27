using System;
using System.Collections.Generic;
using BlackTemple.EpicMine.Core;
using UnityEngine;
using UnityEngine.EventSystems;


public class FieldEditorController : MonoBehaviour {

    private FieldEditorControlPanel _controller;


    [SerializeField] private RectTransform _fieldRectTransform;
    [SerializeField] private FieldEditorAttackPoint _pointPrefab;


    public FieldEditorAttackPoint[,] Grid { get; private set; }

    public Field Field { get; private set; }

  
    public void SetEditorController(FieldEditorControlPanel control)
    {
        _controller = control;
    }

    public void ClearField()
    {
        foreach (Transform point in _fieldRectTransform)
        {
            Destroy(point.gameObject);
        }
    }

    public void Initialize(Field field)
    {
        ClearField();

        Field = field;

        var sizeX = Field.Grid.GetLength(0);
        var sizeY = Field.Grid.GetLength(1);

        Grid = new FieldEditorAttackPoint [sizeX, sizeY];

        for (var i = 0; i < sizeX; i++)
        for (var j = 0; j < sizeY; j++)
        {
            Grid[i, j] = CreateAttackPoint(Field.Grid[i,j]);
        }

        for (var i = 0; i < sizeX; i++)
        for (var j = 0; j < sizeY; j++)
        {
            if (Grid[i, j].Point.Size >= 2)
            {
                SetPointSize(Grid[i, j], Grid[i, j].Point.Size);
            }
        }

    }
    
    public void ClearSize(int x, int y, int size)
    {
        if (x + size <= Grid.GetLength(0) && y + size <= Grid.GetLength(1))
        {
            for (var i = x; i < x + size; i++)
            {
                for (var j = y; j < y + size; j++)
                {
                    var nearPoint = Grid[i, j];
                    if (nearPoint.CombinePoint.Count > 0)
                    {
                        ClearCombines(nearPoint);
                        continue;
                    }
                    nearPoint.Point.SetDefault();
                    nearPoint.UnCombine();
                    nearPoint.Refresh();

                }
            }
        }
    }

    public void ClearCombines(FieldEditorAttackPoint point)
    {
        if (point.CombinePoint.Count > 0)
        {
            for (var i = 0; i < point.CombinePoint.Count; i++)
            {
                var nearPoint = point.CombinePoint[i];
                nearPoint.Point.SetDefault();
                nearPoint.UnCombine();
                i--;
            }
        }
    }
    public void SetPointSize(FieldEditorAttackPoint point, int size)
    {
        var type = point.Point.PointType;
      
        ClearSize(point.Point.X, point.Point.Y, size);

        point.Point.SetSize(size);
        point.Point.SetType(type);

        var list = new List<FieldEditorAttackPoint>();

        if (point.Point.X + point.Point.Size <= Grid.GetLength(0) && point.Point.Y + point.Point.Size <= Grid.GetLength(1))
        {
            for (var i = point.Point.X; i < point.Point.X + point.Point.Size; i++)
            {
                for (var j = point.Point.Y; j < point.Point.Y + point.Point.Size; j++)
                {
                    if(Grid[i,j] == point)
                        continue;

                    list.Add(Grid[i,j]);
                }
            }
        }

        list.Add(point);

        foreach (var fieldEditorAttackPoint in list)
        {
            fieldEditorAttackPoint.Combine(list, fieldEditorAttackPoint == point);
            fieldEditorAttackPoint.Refresh();
        }

        point.Refresh();
    }

    public bool IsFree(int x, int y, int size)
    {

        if (x + size <= Grid.GetLength(0) && y + size <= Grid.GetLength(1))
        {
            for (var i = x; i < x + size; i++)
            {
                for (var j = y; j < y + size; j++)
                {
                    if( i == x && j ==y)
                        continue;
                    
                    var nearPoint = Grid[i, j];
                    if (nearPoint.Point.PointType != AttackPointType.Empty
                        || nearPoint.CombinePoint.Count > 0)
                    {
                        return false;
                    }

                }
            }
        }

        return true;
    }

    public void OnClickCell(FieldEditorAttackPoint point, PointerEventData.InputButton button)
    {
        if (point.Point.PointType == AttackPointType.Empty)
        {
            point.Point.SetType(_controller.PointType);
        }
        else
        {
            var typeCount = Enum.GetNames(typeof(AttackPointType)).Length;
            if (typeCount - 1 > (int)point.Point.PointType)
            {
                point.Point.SetType(point.Point.PointType + 1);

            }
            else point.Point.SetType(0);
        }


        if (button == PointerEventData.InputButton.Right)
        {
            ClearSize(point.Point.X, point.Point.Y, point.Point.Size);
            return;
        }

        if (point.Point.X + _controller.PointSize <= Grid.GetLength(0) &&
            point.Point.Y + _controller.PointSize <= Grid.GetLength(1))
        {
            if (point.Point.PointType == AttackPointType.Empty)
            {
                point.Point.SetDefault();
                ClearCombines(point);
                point.Refresh();
                return;
            }

            SetPointSize(point, _controller.PointSize);
        }
        else
        {
            ClearSize(point.Point.X, point.Point.Y, point.Point.Size);
        }
    }

    public float GetFieldScaleSizeRatio()
    {
        var maxX = _fieldRectTransform.sizeDelta.x;
        var maxY = _fieldRectTransform.sizeDelta.y;

        var figX = Grid.GetLength(0);
        var figY = Grid.GetLength(1);

        var pointSizeX = (maxX / (figX * 100));
        var pointSizeY = (maxY / (figY * 100));

        return pointSizeX < pointSizeY ? pointSizeX : pointSizeY;
    }

    private FieldEditorAttackPoint CreateAttackPoint(FieldAttackPoint pointData)
    {
       /* var recSizeX = _fieldRectTransform.rect.width;
        var recSizeY = _fieldRectTransform.rect.height;

        var s = DefaultConstrains.FieldSizeX * 100f;
        var scale = recSizeX / s;

        var xOffset = recSizeX / DefaultConstrains.FieldSizeX;
        var yOffset = recSizeY / DefaultConstrains.FieldSizeY;
      
        var point = Instantiate(_pointPrefab, _fieldRectTransform.transform);

        point.transform.localPosition = new Vector2(-recSizeX / 2 + xOffset * pointData.X, -recSizeY / 2 + yOffset * pointData.Y);
        point.GetComponent<RectTransform>().sizeDelta = new Vector2(scale*100, scale*100);

        point.Initialize(pointData, OnClickCell);
        point.name = $"{pointData.X}_{pointData.Y}";
      //  return point;*/

        var resultSize = GetFieldScaleSizeRatio();
        var spacing = 5;
      //  resultSize = resultSize > 0.4f ? 0.4f : resultSize;

        var offset = 0;

        var point = Instantiate(_pointPrefab, _fieldRectTransform.transform);

        point.Initialize(this, pointData, OnClickCell);
        point.name = $"{pointData.X}_{pointData.Y}";

        point.transform.localPosition = new Vector3(offset + pointData.X * (resultSize * 100), -offset + pointData.Y * (resultSize * 100));
        point.transform.localScale = new Vector3(resultSize, resultSize);

        return point;
    }


    

}
