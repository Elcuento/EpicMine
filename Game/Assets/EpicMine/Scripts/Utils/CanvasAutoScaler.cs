using BlackTemple.EpicMine;
using UnityEngine;
using UnityEngine.UI;


[DisallowMultipleComponent]
public class CanvasAutoScaler : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<CanvasScaler>().matchWidthOrHeight = WindowManager.Instance.CanvasScale;
    }
}
