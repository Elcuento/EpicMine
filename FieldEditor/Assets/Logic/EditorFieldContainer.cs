using UnityEngine;
using UnityEngine.UI;

public class EditorFieldContainer : MonoBehaviour
{
    public Text Label;
    public Transform Container;

    public void Initialize(string text)
    {
        Label.text = text;
    }

    public void Initialize(bool enableLabel)
    {
        Label.gameObject.SetActive(enableLabel);
    }
}
