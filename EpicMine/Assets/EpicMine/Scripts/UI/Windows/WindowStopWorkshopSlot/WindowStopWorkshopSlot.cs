using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class WindowStopWorkshopSlot : WindowBase
    {
        [SerializeField] private ItemView _itemPrefab;
        [SerializeField] private RectTransform _itemsContainer;

        private Core.WorkshopSlot _workshopSlot;


        public void Initialize(Core.WorkshopSlot blWorkshopSlot)
        {
            Unsubscribe();
            _workshopSlot = blWorkshopSlot;
            ChangeResources();
            Subscribe();
        }

        public void Stop()
        {
            _workshopSlot.CollectCompleted();
            _workshopSlot.Stop();
            Close();
        }


        public override void OnClose()
        {
            base.OnClose();
            Unsubscribe();
            _itemsContainer.ClearChildObjects();
            _workshopSlot = null;
        }


        private void Subscribe()
        {
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
            EventManager.Instance.Subscribe<WorkshopSlotChangeEvent>(OnSlotChange);
        }

        private void OnSlotComplete(WorkshopSlotCompleteEvent eventData)
        {
            if (eventData.WorkshopSlot == _workshopSlot)
                ChangeResources();
        }

        private void OnSlotChange(WorkshopSlotChangeEvent eventData)
        {
            if (eventData.WorkshopSlot == _workshopSlot)
                ChangeResources();
        }

        private void Unsubscribe()
        {
            EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
            EventManager.Instance.Unsubscribe<WorkshopSlotChangeEvent>(OnSlotChange);
        }


        private void ChangeResources()
        {
            _itemsContainer.ClearChildObjects();

            if (_workshopSlot.CompleteAmount > 0)
            {
                var item = Instantiate(_itemPrefab, _itemsContainer, false);
                item.Initialize(_workshopSlot.StaticRecipe.Id, _workshopSlot.CompleteAmount);
            }

            if (_workshopSlot.CompleteAmount >= _workshopSlot.NecessaryAmount)
                return;

            var ingredients = StaticHelper.GetIngredients(_workshopSlot.StaticRecipe, _workshopSlot.NecessaryAmount - _workshopSlot.CompleteAmount);
            foreach (var ingredient in ingredients)
            {
                var ingredientGo = Instantiate(_itemPrefab, _itemsContainer, false);
                ingredientGo.Initialize(ingredient);
            }
        }
    }
}