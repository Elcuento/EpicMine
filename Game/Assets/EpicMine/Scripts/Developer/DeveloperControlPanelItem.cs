using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[SuppressMessage("ReSharper", "IdentifierTypo")]
public class DeveloperControlPanelItem : MonoBehaviour
{
    public DeveloperControlPanelVerticalScrollItem VerticalScroll;
    public bool DontDestroyObject;

    public void SetVerticalScroll(DeveloperControlPanelVerticalScrollItem item)
    {
        VerticalScroll = item;
    }
}
