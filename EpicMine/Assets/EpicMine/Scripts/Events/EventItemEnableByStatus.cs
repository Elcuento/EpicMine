using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

public class EventItemEnableByStatus : MonoBehaviour
{
    [SerializeField] private GameEventType _eventType;
    [SerializeField] private bool _status;

    [SerializeField] private GameObject _object;
    [SerializeField] private Behaviour _monoBeh;

    private void Start()
    {
        var gameEvent = App.Instance.GameEvents.Events.Find(x => x.StaticGameEvent.Type == _eventType);
        if (gameEvent != null)
        {
            var status = gameEvent.IsActive == _status;
               
            if (_object != null)
            {
                _object.SetActive(status);
            }

            if(_monoBeh != null)
                _monoBeh.enabled =status;
        }
    }
}
