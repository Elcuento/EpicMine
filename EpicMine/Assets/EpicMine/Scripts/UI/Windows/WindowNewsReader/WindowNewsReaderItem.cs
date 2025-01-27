
using CommonDLL.Dto;
using TMPro;
using UnityEngine;

public class WindowNewsReaderItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _date;
    [SerializeField] private TextMeshProUGUI _description;

    public void Initialize(News data)
    {
        _title.text = data.Title;
        _description.text = data.Description;
        _date.text = data.Date;
    }
}
