using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopButton : MonoBehaviour {

    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _particles;
    [SerializeField] private GameObject _arrow;

    private void Start () {

        EventManager.Instance.Subscribe<WorkshopSlotChangeEvent>(OnChangeWorkShopSlot);
        EventManager.Instance.Subscribe<WorkshopSlotStartMeltingEvent>(OnWorkshopStartMelting);

        CheckRestsSlots();

        if (_arrow == null)
            return;

        EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnWorkShopSlotCompleteEvent);
        EventManager.Instance.Subscribe<WorkshopSlotClearEvent>(OnWorkShopSlotClearEvent);

        _arrow.SetActive(App.Instance.Player.Workshop.Slots.Exists(x => x.IsComplete) ||
                                 App.Instance.Player.Workshop.SlotsShard.Exists(x => x.IsComplete));
    }

    private void OnWorkShopSlotClearEvent(WorkshopSlotClearEvent data)
    {
        _arrow.SetActive(App.Instance.Player.Workshop.Slots.Exists(x => x.IsComplete) ||
                         App.Instance.Player.Workshop.SlotsShard.Exists(x => x.IsComplete));

        CheckRestsSlots();
    }

    private void OnWorkShopSlotCompleteEvent(WorkshopSlotCompleteEvent data)
    {
        _arrow.SetActive(true);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<WorkshopSlotChangeEvent>(OnChangeWorkShopSlot);
        EventManager.Instance.Unsubscribe<WorkshopSlotStartMeltingEvent>(OnWorkshopStartMelting);

        if (_arrow == null)
            return;

        EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnWorkShopSlotCompleteEvent);
        EventManager.Instance.Unsubscribe<WorkshopSlotClearEvent>(OnWorkShopSlotClearEvent);
    }

    private void OnWorkshopStartMelting(WorkshopSlotStartMeltingEvent eventData)
    {
         CheckRestsSlots();
    }

    private void OnChangeWorkShopSlot(WorkshopSlotChangeEvent eventData)
    {
        if (eventData.WorkshopSlot.NecessaryAmount == eventData.WorkshopSlot.CompleteAmount)
        {
           CheckRestsSlots();
        }
    }

    private void CheckRestsSlots()
    {
        var isInactive = App.Instance.Player.Workshop.Slots.All(s => s.NecessaryAmount == s.CompleteAmount)
                         && App.Instance.Player.Workshop.SlotsShard.All(s => s.NecessaryAmount == s.CompleteAmount);

        _particles.SetActive(!isInactive);

        _icon.sprite = isInactive
            ? App.Instance.ReferencesTables.Sprites.WorkShopInactiveButton
            : App.Instance.ReferencesTables.Sprites.WorkShopActiveButton;
    }
    
}

