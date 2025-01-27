using System.Collections.Generic;
using System.Linq;
using BlackTemple.EpicMine.Core;
using UnityEngine;

public class FieldEditorFigurePanel : MonoBehaviour
{

    [SerializeField] private GameObject _root;

    [SerializeField] private Transform _figuresContainer;
    [SerializeField] private FieldEditorFigure _figurePrefab;


    private FieldEditorControlPanel _controller;



    public void Start()
    {
        _root.SetActive(false);
    }

    public void SetController(FieldEditorControlPanel controller)
    {
        _controller = controller;
    }

    public void OnClickClose()
    {
        _root.SetActive(false);
    }

    public void ShowFigures(FieldPointArea field)
    {
        _root.SetActive(true);

        foreach (Transform fig in _figuresContainer)
        {
            Destroy(fig.gameObject);
        }

        var fieldFigures = field.Figures;

        var figuresIds = new List<string>(fieldFigures.Keys);

        var figureList = _controller.Fields.Where(x => x is FieldFigure).Cast<FieldFigure>().ToList();

        if (figureList.Count == 0)
            return;

        for (var i = 0; i < figuresIds.Count; i++)
        {
            var key = figuresIds[i];

            var sourceFigure = figureList.Find(x => x.Id == key);
            if (sourceFigure == null)
            {
                figuresIds.Remove(key);
                figuresIds.Remove(figuresIds[i]);
                i--;
            }
        }

        foreach (var fig in figureList)
        {
            if (!fieldFigures.ContainsKey(fig.Id))
            {
                fieldFigures.Add(fig.Id, 0);
            }

            var figureObject = Instantiate(_figurePrefab, _figuresContainer);
            figureObject.Initialize(field,fig);
          
        }

    }
}
