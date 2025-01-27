using System.Collections.Generic;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;

public class FieldEditorFigure : MonoBehaviour {

    public GameObject PointPrefab;

    public TextMeshProUGUI Id;
    public TMP_InputField Chance;
    public RectTransform Field;

    private FieldFigure _figureField;
    private FieldPointArea _field;


    public void Initialize(FieldPointArea area, FieldFigure figure)
    {

        _field = area;
        _figureField = figure;

        Id.text = _figureField.Id;
        Chance.text = area.Figures[figure.Id].ToString();

        var maxX = Field.sizeDelta.x;
        var maxY = Field.sizeDelta.y;

        var grid = _figureField.GetShortGrid();

        var figX = grid.GetLength(0);
        var figY = grid.GetLength(1);

        var pointSizeX = (maxX / (figX * 100));
        var pointSizeY = (maxY / (figY * 100));

        var resultSize = pointSizeX < pointSizeY ? pointSizeX : pointSizeY;

        resultSize = resultSize > 0.4f ? 0.4f : resultSize;

        var offset = (resultSize * 100) / 2;

        for (var i = 0; i < figX; i++)
        {
            for (var j = 0; j < figY; j++)
            {
                if (grid[i, j].PointType == AttackPointType.Empty)
                    continue;

                var figurePoint = Instantiate(PointPrefab, Field.transform, false);
                figurePoint.transform.localPosition = new Vector3(offset + i * (resultSize * 100), j * (resultSize * 100) + offset);
                figurePoint.transform.localScale = new Vector3(resultSize, resultSize);

            }
        }

    }

    public void OnChangeChance()
    {
        var chance = float.Parse(Chance.text);
        
        var figures = _field.Figures;
        if (figures.ContainsKey(_figureField.Id))
        {
            figures[_figureField.Id] = chance;
        }
        else
        {
            figures.Add(_field.Id, chance);
        }
    }


}
