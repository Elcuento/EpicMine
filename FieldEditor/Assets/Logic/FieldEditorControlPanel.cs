using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlackTemple.EpicMine.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


public class FieldEditorControlPanel : MonoBehaviour
{
    [SerializeField] private FieldEditorController _controller;
    [SerializeField] private FieldEditorFigurePanel _figuresPanel;

    [SerializeField] private GameObject _fieldsPanel;
    [SerializeField] private ScrollRect _fieldsList;

    [SerializeField] private EditorFieldControlButtonExtended _editorButtonExtended;
    [SerializeField] private EditorFieldContainer _editorFieldContainerPrefab;
    [SerializeField] private EditorFieldExtendedContainer _editorFieldExtendedContainerPrefab;
    [SerializeField] private Button _buttonPrefab;
    [SerializeField] private InputField _inputFieldPrefab;
    [SerializeField] private Dropdown _dropDownPrefab;

    [SerializeField] private RectTransform _topScroll;
    [SerializeField] private ScrollRect _fieldScroll;
    [SerializeField] private ScrollRect _controlScroll;


    [Header("Controls")]
    private EditorFieldExtendedContainer _idsContainer;
    private Button _id;

    public int PointSize = 2;
    public AttackPointType PointType = AttackPointType.Default;


    public List<Field> Fields { get; private set; }

    public void Start()
    {
        _controller.SetEditorController(this);
        _figuresPanel.SetController(this);


        Fields = new List<Field>();
        PointSize = 2;

        var fields = DataHelper.Load();

        FixList(fields);

        LoadFields(fields);
    }

    public void FixList(List<BlackTemple.EpicMine.Dto.Field> fields)
    {
        var figures = fields.FindAll(x => x is BlackTemple.EpicMine.Dto.FieldFigure);

        foreach (var field in fields)
        {
            var pointArea = field as BlackTemple.EpicMine.Dto.FieldPointArea;

            if (pointArea != null)
            {
                var newFigures = new Dictionary<string, float>();

                foreach (var figure in figures)
                {
                    if (pointArea.Figures.ContainsKey(figure.Id))
                    {
                        newFigures.Add(figure.Id, pointArea.Figures[figure.Id]);
                    }
                    else
                    {
                        newFigures.Add(figure.Id, 0);
                    }
                }

                pointArea.Figures = newFigures;
            }
        }
    }

    public void SelectField(string id)
    {
        foreach (Transform idItem in _idsContainer.Container)
        {
            var label = idItem.GetComponentInChildren<Text>();
            label.color = Color.black;
        }
        _id = _idsContainer.Container.Find(id).GetComponentInChildren<Button>();
        _id.GetComponentInChildren<Text>().color = Color.green;

        PointSize = 2;
    }

    public void InitializeField(string id)
    {
        var field = Fields.Find(x => x.Id == id);

        if (field != null)
        {
            _controller.Initialize(field);
            var tryFind = _idsContainer.Container.Find(field.Id);
            if (tryFind != null)
            {
                _id = tryFind.GetComponent<Button>();
            }

            if (field is FieldFigure)
                FillFiguresControl(field as FieldFigure);
            else FillFieldControl(field as FieldPointArea);


        }
        SelectField(id);
    }

    public void InitializeField(Field field)
    {
        _controller.Initialize(field);
        _id = _idsContainer.Container.Find(field.Id).GetComponent<Button>();

        if (field is FieldFigure)
            FillFiguresControl(field as FieldFigure);
        else FillFieldControl(field as FieldPointArea);

        SelectField(field.Id);
    }

    public void FillFields(List<Field> fields, bool figures = false)
    {
        Sort();
        Fields = fields;


        foreach (Transform trans in _fieldScroll.content)
        {
            Destroy(trans.gameObject);
        }

       var category = AddControlItem("", _fieldScroll.content);
        category.Initialize(false);

        AddExtendedButton(ShowFields, () => { ShowFieldsList(); }, "Поля", category.Container);
        AddExtendedButton(ShowFigures, () => { ShowFieldsList(true); }, "Фигуры", category.Container);


        _idsContainer = AddExtendedControlItem($"{(figures ? "Фигуры" : "Поля")}", (a) =>
        {
            foreach (Transform trans in _idsContainer.Container)
            {
                if(trans.gameObject!=_idsContainer.gameObject)
                trans.gameObject.SetActive(trans.name.StartsWith(a));
                LayoutRebuilder.ForceRebuildLayoutImmediate(_idsContainer.Container.GetComponent<RectTransform>());
            }
        },_fieldScroll.content);

        foreach (var field in fields)
        {
            if (!figures &&  field is FieldFigure || figures && field is FieldPointArea)
                continue;

            var id = field.Id;
            var button = AddButton(() => { InitializeField(id); }, id, _idsContainer.Container);
            button.name = id;
        }


        var first = Fields.FirstOrDefault(x => figures ? (x is FieldFigure) : (x is FieldPointArea));
        if (first == null)
        {
            var id = Guid.NewGuid().ToString();

            if(figures)
            first = new FieldFigure(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY, id);
            else first = new FieldPointArea(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY, id);

            var button = AddButton(() => { InitializeField(id); }, id, _idsContainer.Container);
            button.name = id;

            Fields.Add(first);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_fieldScroll.content);
        
        InitializeField(first);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_fieldScroll.content);
    }


    public void LoadFields(List<BlackTemple.EpicMine.Dto.Field> fields)
    {
        if (fields == null || fields.Count <= 0)
        {
            Fields = new List<Field>();

            var fieldId = "mine_0";
            var figureId = "figure_1";

            Fields.Add(new FieldPointArea(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY, fieldId));
            Fields.Add(new FieldFigure(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY, figureId));
          
        }
        else
        {
            foreach (var field in fields)
            {
                if(field is BlackTemple.EpicMine.Dto.FieldPointArea)
                Fields.Add(new FieldPointArea(field));
                else Fields.Add(new FieldFigure(field));
            }
        }


        FillFields(Fields);
    }

    public void Sort()
    {
        Fields.Sort((x, y) => string.Compare(x.Id, y.Id));
    }

    public void FillFiguresControl(FieldFigure field)
    {
        foreach (Transform item in _controlScroll.content)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in _topScroll)
        {
            Destroy(item.gameObject);
        }

        AddControlItem("Id",
            AddInputField((a) =>
            {
                if (string.IsNullOrEmpty(a))
                    return;

                field.SetId(a);
                if (_id != null)
                {
                    _id.GetComponentInChildren<Text>().text = a;
                    _id.name = a;
                }
            }, field.Id),
            _controlScroll.content
        );

        AddControlItem("Point size",
            AddDropDown((a) => { PointSize = a + 2; }, new List<string>() { "1", "2", "3" }),
            _controlScroll.content
        );

        var config = AddControlItem("Default Point Type", _controlScroll.content);

        AddDropDown((a) => { PointType = (AttackPointType)a; }, Enum.GetNames(typeof(AttackPointType)).ToList(), (int)PointType, config.Container);

        AddButton(() =>
        {
            var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            foreach (var f in Fields)
            {
                var data = JsonConvert.SerializeObject(f.ExportData(), jsonSettings);
                File.WriteAllText(Application.persistentDataPath + "/fields/" + f.Id + ".txt", data);
            }
            Debug.Log("Export ok " + Application.persistentDataPath + "/fields/");
        }, "Export separated", _topScroll.transform);

        AddButton(Save, "Save", _topScroll.transform);

        AddButton(()=>
        {
            New(true);
        }, "New", _topScroll.transform);

        AddButton(Remove, "Remove", _topScroll.transform);

        AddButton(Clear, "Clear", _topScroll.transform);

        AddButton(() =>
        {
            Save();
            Exit();
        }, "Exit", _topScroll.transform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_controlScroll.content);
    }
    public void FillFieldControl(FieldPointArea field)
    {
        foreach (Transform item in _controlScroll.content)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in _topScroll)
        {
            Destroy(item.gameObject);
        }

        AddControlItem("Id",
            AddInputField((a) =>
            {
                if (string.IsNullOrEmpty(a))
                    return;

                field.SetId(a);
                if (_id != null)
                {
                    _id.GetComponentInChildren<Text>().text = a;
                    _id.name = a;
                }
            }, field.Id),
            _controlScroll.content
        );

        AddControlItem("Type",
            AddDropDown((a) => { field.SetRulesType((FieldRulesType) a); }, Enum.GetNames(typeof(FieldRulesType)).ToList(), (int)field.RulesType),
            _controlScroll.content
        );

        AddControlItem("Max Points",
            AddInputField((a) =>
            {
                field.SetMaxPoints(int.Parse(a));
            }, field.MaxPoints.ToString()),
            _controlScroll.content
        );


        AddControlItem("Point size",
            AddDropDown((a) => { PointSize = a + 2; }, new List<string>() {"1", "2", "3"}),
            _controlScroll.content
        );

        AddControlItem("Figures Chances",
            AddButton(() =>
            {
                _figuresPanel.ShowFigures(_controller.Field as FieldPointArea);
            }, "Figures Chances", null),
            _controlScroll.content
        );

        var config = AddControlItem("Default Point Type", _controlScroll.content);

        AddDropDown((a) => { PointType = (AttackPointType)a; }, Enum.GetNames(typeof(AttackPointType)).ToList(), (int)PointType, config.Container);


        AddButton(() =>
        {
            var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            foreach (var f in Fields)
            {
                var data = JsonConvert.SerializeObject(f.ExportData(), jsonSettings);
                File.WriteAllText(Application.persistentDataPath + "/fields/" + f.Id + ".txt", data);
            }
            Debug.Log("Export ok " + Application.persistentDataPath + "/fields/");
        }, "Export separated", _topScroll.transform);


        AddButton(Save, "Save", _topScroll.transform);

        AddButton(() =>
        {
            New();
        }, "New", _topScroll.transform);

        AddButton(Remove, "Remove", _topScroll.transform);

        AddButton(Clear, "Clear", _topScroll.transform);

        AddButton(() =>
        {
            Save();
            Exit();
        }, "Exit", _topScroll.transform);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_controlScroll.content);
    }

    public EditorFieldContainer AddControlItem(string label, GameObject obj, Transform trans)
    {
        var controlItemContainer = Instantiate(_editorFieldContainerPrefab, trans, false);
        obj.transform.SetParent(controlItemContainer.Container, false);
        controlItemContainer.Label.text = label;
        return controlItemContainer;
    }

    public EditorFieldContainer AddControlItem(string label, Transform trans)
    {
        var controlItemContainer = Instantiate(_editorFieldContainerPrefab, trans, false);
        controlItemContainer.Label.text = label;
        return controlItemContainer;
    }
    public EditorFieldExtendedContainer AddExtendedControlItem(string label, Action<string> onChange, Transform trans)
    {
        var controlItemContainer = Instantiate(_editorFieldExtendedContainerPrefab, trans, false);
        controlItemContainer.Initialize(label,onChange);
        return controlItemContainer;
    }

    public GameObject AddButton(Action action, string text, Transform trans)
    {
        var fieldButton = Instantiate(_buttonPrefab, trans, false);
        fieldButton.GetComponentInChildren<Text>().text = text;
        fieldButton.onClick.AddListener(() =>
        {
            action?.Invoke();
        });
        return fieldButton.gameObject;
    }

    public GameObject AddExtendedButton(Action action, Action action2, string text, Transform trans)
    {
        var fieldButton = Instantiate(_editorButtonExtended, trans, false);
        fieldButton.Initialize(action, text,action2);
        return fieldButton.gameObject;
    }

    public GameObject AddInputField(Action<string> onValueChange, string text)
    {
        var fieldButton = Instantiate(_inputFieldPrefab, _controlScroll.content, false);
        fieldButton.text = text;
        fieldButton.onValueChanged.AddListener((a) =>
        {
            onValueChange?.Invoke(a);
        });
        return fieldButton.gameObject;
    }

    public GameObject AddDropDown(Action<int> onValueChange, List<string> list, int value = 0, Transform trans = null)
    {
        var dropDown = Instantiate(_dropDownPrefab, _controlScroll.content, false);

        dropDown.options = new List<Dropdown.OptionData> ();

        for (var i = 0; i < list.Count; i++)
        {
            dropDown.options.Add(new Dropdown.OptionData(name = list[i]));
        }

        dropDown.onValueChanged.AddListener((a) =>
        {
            onValueChange?.Invoke(a);
        });

        dropDown.value = value;
        dropDown.RefreshShownValue();

        dropDown.transform.SetParent(trans,false);

        return dropDown.gameObject;
    }

    public void Save()
    {
        DataHelper.Save(Fields);
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void Remove()
    {
        var isFigure = _controller.Field is FieldFigure;

        Fields.Remove(_controller.Field);


        var leftFigures = Fields.FindAll(x => x is FieldFigure);

        if (leftFigures.Count <= 0)
        {
            var field = new Field(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY,
                Guid.NewGuid().ToString());
            Fields.Add(field);

            var id = field.Id;

            var button = AddButton(() => { InitializeField(id); }, field.Id, _idsContainer.transform);
            button.name = field.Id;
        }

        FillFields(Fields, isFigure);
       
    }

    public void New(bool figureField = false)
    {
        var id = "";
        if (figureField)
        {
            var field = new FieldFigure(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY, Guid.NewGuid().ToString());
            Fields.Add(field);
            id = field.Id;
        }
        else
        {
            var field = new FieldPointArea(DefaultConstrains.FieldSizeX, DefaultConstrains.FieldSizeY, Guid.NewGuid().ToString());
            Fields.Add(field);
            id = field.Id;
        }
     
       FillFields(Fields, figureField);
       SelectField(id);
        InitializeField(id);
    }

    public void ShowFigures()
    {
        FillFields(Fields, true);
    }

    public void ShowFieldsList(bool figure = false)
    {

         FillFields(Fields, figure);

        _fieldsPanel.SetActive(true);

        foreach (Transform tra in _fieldsList.content)
        {
            Destroy(tra.gameObject);
        }

        foreach (var field in Fields)
        {
            if(field is FieldFigure && !figure || field is FieldPointArea && figure)
                continue;

            var fieldId = field.Id;
            var button = AddButton(() => { InitializeField(fieldId); }, fieldId, _fieldsList.content);
            button.name = fieldId;
        }
    }

    public void HideFieldList()
    {
        _fieldsPanel.SetActive(false);
    }


    public void ShowFields()
    {
        FillFields(Fields);
    }

    public void Clear()
    {
        foreach (var point in _controller.Grid)
        {
            point.Point.SetDefault();
            point.UnCombine();
            point.Refresh();
        }
    }

    public void OnApplicationQuit()
    {
        DataHelper.Save(Fields);
    }
}
