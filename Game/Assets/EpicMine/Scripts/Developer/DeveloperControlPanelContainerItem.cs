using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperControlPanelContainerItem : DeveloperControlPanelItem
{

	public Transform Container;

    [SerializeField] private TextMeshProUGUI _label;

    public void Initialize(string txt, GameObject obj)
    {
        _label.text = txt;
        obj.transform.SetParent(Container);
    }

    public void Initialize(string txt)
    {
        _label.text = txt;
    }

}
