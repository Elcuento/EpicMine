using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class VillageSceneBurglar : VillageSceneCharacter
    {
        [SerializeField]
        private List<VillageScenePlaceForChest> _chests = new List<VillageScenePlaceForChest>();



        protected override void Start()
        {
            Initialize();
            _controller.RegisterVillageQuestCharacter(this, _questArrow);
            CheckQuest();
        }

        private void Initialize()
        {
            for (var i = 0; i < App.Instance.Player.Burglar.Chests.Count; i++)
            {
                var chest = App.Instance.Player.Burglar.Chests[i];
                var sceneChest = _chests[i];

                sceneChest.Initialize(chest);
            }
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            EventManager.Instance.Subscribe<ChestStartBreakingEvent>(OnChestStartBreaking);
            EventManager.Instance.Subscribe<ChestBreakingTimeLeftEvent>(OnChestBreakingTimeLeft);
            EventManager.Instance.Subscribe<ChestBreakedEvent>(OnChestBreaked);
            EventManager.Instance.Subscribe<BurglarChestOpenedEvent>(OnChestOpened);
            EventManager.Instance.Subscribe<ChestAddedEvent>(OnChestAdded);
        }

        protected override void UnSubscribe()
        {
            base.UnSubscribe();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<ChestStartBreakingEvent>(OnChestStartBreaking);
                EventManager.Instance.Unsubscribe<ChestBreakingTimeLeftEvent>(OnChestBreakingTimeLeft);
                EventManager.Instance.Unsubscribe<ChestBreakedEvent>(OnChestBreaked);
                EventManager.Instance.Unsubscribe<BurglarChestOpenedEvent>(OnChestOpened);
                EventManager.Instance.Unsubscribe<ChestAddedEvent>(OnChestAdded);
            }
        }

        private void OnChestAdded(ChestAddedEvent eventData)
        {
            Debug.LogError("Add");
            Initialize();
        }

        private void OnChestStartBreaking(ChestStartBreakingEvent eventData)
        {
            foreach (var chest in _chests)
            {
                if (chest.Chest == eventData.Chest)
                {
                    chest.SetBreaking();
                    return;
                }
            }
        }

        private void OnChestBreakingTimeLeft(ChestBreakingTimeLeftEvent eventData)
        {
            foreach (var chest in _chests)
            {
                if (chest.Chest == eventData.Chest)
                {
                    chest.SetBreakingTimeLeft(eventData.TimeLeft);
                    return;
                }
            }
        }

        private void OnChestBreaked(ChestBreakedEvent eventData)
        {
            foreach (var chest in _chests)
            {
                if (chest.Chest == eventData.Chest)
                {
                    chest.SetBroken();
                    return;
                }
            }
        }

        private void OnChestOpened(BurglarChestOpenedEvent eventData)
        {
            var chest = _chests.FirstOrDefault(ch => ch.Chest == eventData.Chest);
            if (chest == null)
                return;

            chest.Clear();
        }
    }
}