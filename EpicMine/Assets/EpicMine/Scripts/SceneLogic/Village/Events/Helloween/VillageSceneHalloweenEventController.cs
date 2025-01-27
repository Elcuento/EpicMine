using System.Collections.Generic;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;
public class VillageSceneHalloweenEventController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _halloweenContent;

    private void Start()
    {
        if(App.Instance.GameEvents.IsActive(GameEventType.Halloween))
        {
            foreach (var o in _halloweenContent)
            {
                o.SetActive(true);
            }
        }
    }
}
