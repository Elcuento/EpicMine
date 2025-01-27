using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class DeveloperManager : MonoBehaviour
    {
        public static bool IsDebug;
        
        public void Start()
        {
            // DONT TOUCH IT
            if (Debug.isDebugBuild || Application.version == "0.0.0" || IsDebug)
            {
                var dev = Resources.Load<DeveloperController>("Prefabs/Developer/DeveloperController");
                  Instantiate(dev, transform);
            }
            else
            {
                enabled = false;
            }
        }
    }
}
