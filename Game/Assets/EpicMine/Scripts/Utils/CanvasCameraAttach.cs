using UnityEngine;

namespace BlackTemple.EpicMine.Utils
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraAttach : MonoBehaviour
    {
        private void Start()
        {
            Camera worldCamera = null;

            var uiCameraGo = GameObject.FindWithTag(Tags.UICamera);
            if (uiCameraGo != null)
                worldCamera = uiCameraGo.GetComponent<Camera>();

            if (worldCamera == null)
                worldCamera = Camera.main;

            gameObject
                .GetComponent<Canvas>()
                .worldCamera = worldCamera;
        }
    }
}