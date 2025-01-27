using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;

public class VillageSceneWorkshop : MonoBehaviour
{
    [SerializeField] private GameObject _completeArrow;

    private void Start()
    {
        var isTutorCompleted = App.Instance.Controllers.TutorialController.IsComplete;

        if (!isTutorCompleted)
            return;

        _completeArrow.SetActive(App.Instance.Player.Workshop.Slots.Exists(x => x.IsComplete) ||
                                     App.Instance.Player.Workshop.SlotsShard.Exists(x => x.IsComplete));

        EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnWorkShopSlotCompleteEvent);
        EventManager.Instance.Subscribe<WorkshopSlotClearEvent>(OnWorkShopSlotClearEvent);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnWorkShopSlotCompleteEvent);
        EventManager.Instance.Unsubscribe<WorkshopSlotClearEvent>(OnWorkShopSlotClearEvent);
    }

    private void OnWorkShopSlotCompleteEvent(WorkshopSlotCompleteEvent eventData)
    {
        _completeArrow.SetActive(true);
    }

    private void OnWorkShopSlotClearEvent(WorkshopSlotClearEvent eventData)
    {
        _completeArrow.SetActive(App.Instance.Player.Workshop.Slots.Exists(x => x.IsComplete) ||
                                 App.Instance.Player.Workshop.SlotsShard.Exists(x => x.IsComplete));
    }
}
