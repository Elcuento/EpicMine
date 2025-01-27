using UnityEngine;

public class DeveloperControlPanelVerticalScrollItem : MonoBehaviour
{
    public Transform Content;

    public int Index;

    public void Update()
    {
        Index = transform.GetSiblingIndex();
    }
}
