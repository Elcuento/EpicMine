using System;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Utils;
using CommonDLL.Static;
using DG.Tweening;
using DragonBones;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

namespace BlackTemple.EpicMine
{
    public class VillageSceneController : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed;
        [SerializeField] private float _scrollMax;
        [SerializeField] private float _scrollMin;

        [Space]
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private ScrollRect _worldScrollRect;
        [SerializeField] private RectTransform _worldScrollRectTransform;
        [SerializeField] private RectTransform _worldRectTransform;
        [SerializeField] private ScrollViewEventHandler _scrollViewEventHandler;

        [Space]
        [SerializeField] private TextMeshProUGUI _artefactsAmountText;

        [Space]
        [SerializeField] private RectTransform _autoMinerRectTransform;
        [SerializeField] private RectTransform _shopRectTransform;
        [SerializeField] private RectTransform _burglarRectTransform;
        [SerializeField] private RectTransform _mineRectTransform;
        [SerializeField] private RectTransform _blacksmithRectTransform;
        [SerializeField] private RectTransform _workshopRectTransform;
        [SerializeField] private RectTransform _tasksRectTransform;
        [SerializeField] private RectTransform _enceladRectTransform;
        [SerializeField] private RectTransform _beforeEnceladRectTransform;
        [SerializeField] private RectTransform _torchesMerchantRectTransform;

        [Space]
        [SerializeField] private RectTransform _questPointersTransform;

        [Space]
        [SerializeField] private Transform _pickaxeContainer;

        [Space]
        [SerializeField] private UnityArmatureComponent _workshop;
        
        [Space]
        [SerializeField] private VillageSceneQuestPointer _questPointerPrefab;



        public void SettingsButtonClick()
        {
            WindowManager.Instance.Show<WindowSettings>();
        }

        public void LeaderBoardButtonClick()
        {
            WindowManager
                .Instance
                .Show<WindowLeaderBoard>();
        }


        public void UpgradeButtonClick()
        {
            WindowManager.Instance.Show<WindowUpgrade>(withCurrencies: true);
        }

        public void InventoryButtonClick()
        {
            WindowManager.Instance.Show<WindowInventory>(withCurrencies: true);
        }

        public void ShopButtonClick()
        {
            ScrollTo(_shopRectTransform, ShopBuildingClick);
        }

        public void AutoMinerButtonClick()
        {
            ScrollTo(_autoMinerRectTransform, AutoMinerBuildingClick);
        }

        public void BurglarButtonClick()
        {
            ScrollTo(_burglarRectTransform);
        }

        public void MineButtonClick()
        {
            ScrollTo(_mineRectTransform, MineBuildingClick);
        }

        public void BlacksmithButtonClick()
        {
            ScrollTo(_blacksmithRectTransform, BlacksmithBuildingClick);
        }

        public void WorkshopButtonClick()
        {
            ScrollTo(_workshopRectTransform, WorkshopBuildingClick);
        }

        public void TasksButtonClick()
        {
            ScrollTo(_tasksRectTransform, TasksBuildingClick);
        }

        public void PvpArenaButtonClick()
        {
            ScrollTo(_enceladRectTransform, PvpArenaBuildingClick);
        }
        public void TorchesMerchantButtonClick()
        {
            ScrollTo(_torchesMerchantRectTransform, TorchesMerchantBuildingClick);
        }

        public void ScrollToMerchant()
        {
            ScrollTo(_shopRectTransform);
        }

        public void ScrollToBurglar()
        {
            ScrollTo(_burglarRectTransform);
        }

        public void ScrollToMine()
        {
            ScrollTo(_mineRectTransform);
        }

        public void ScrollToBlacksmith()
        {
            ScrollTo(_blacksmithRectTransform);
        }

        public void ScrollToWorkshop()
        {
            ScrollTo(_workshopRectTransform);
        }

        public void ScrollToTasks()
        {
            ScrollTo(_tasksRectTransform);
        }

        public void ScrollToPvp()
        {
            ScrollTo(_enceladRectTransform);
        }

        public void ScrollToTorchesMerchant()
        {
            ScrollTo(_torchesMerchantRectTransform);
        }

        public void AutoMinerBuildingClick()
        {
            if (!App.Instance.Player.AutoMiner.IsOpen)
                return;

            SceneManager.Instance.LoadScene(ScenesNames.AutoMiner);
        }

        public void ShopBuildingClick()
        {
            var windowShop = WindowManager.Instance.Show<WindowShop>(withCurrencies: true);
            windowShop.OpenDefault();
        }

        public void MineBuildingClick()
        {
            SceneManager.Instance.LoadScene(ScenesNames.Tiers);
        }

        public void BlacksmithBuildingClick()
        {
            WindowManager.Instance.Show<WindowBlacksmith>(withCurrencies: true);
        }

        public void WorkshopBuildingClick()
        {
            WindowManager.Instance.Show<WindowWorkshop>(withCurrencies: true);
        }

        public void TasksBuildingClick()
        {
            WindowManager.Instance.Show<WindowDailyTasksQuest>(withCurrencies: true)
                .OpenLastTab();
        }

       
        public void PvpArenaBuildingClick()
        {
            SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
        }


        public void TorchesMerchantBuildingClick()
        {
            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialUnRowStepIds
                .TorchesShopClick))
            {
                var dialogue =
                    App.Instance.StaticData.Dialogues.FirstOrDefault(d => d.Id == "tutorial_step_torches_shop_click");
                var windowDialogue = WindowManager.Instance.Show<WindowDialogue>();
                windowDialogue.Initialize(dialogue, ()=>
                {
                    App.Instance.Controllers.TutorialController.SetStepComplete(TutorialUnRowStepIds
                        .TorchesShopClick);
                    WindowManager.Instance.Show<WindowTorchesMerchant>();
                });

                return;
            }
            WindowManager.Instance.Show<WindowTorchesMerchant>(withCurrencies: true, withRating: true);
        }
  
        private void Start()
        {
            _worldScrollRect.horizontalNormalizedPosition = 0.4f;
            _scrollViewEventHandler.OnDragBegin += OnDragBegin;

            CheckWorkshopStove();

            EventManager.Instance.Subscribe<WorkshopSlotStartMeltingEvent>(OnSlotStartMelting);
            EventManager.Instance.Subscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
            EventManager.Instance.Subscribe<WorkshopSlotClearEvent>(OnSlotClear);
            EventManager.Instance.Subscribe<PickaxeSelectEvent>(OnPickaxeSelect);

            ShowCurrentPickaxe();

           ShopHelper.ShowStartGameShop();
            
      //     WindowManager.Instance.Show<WindowDialogue>()
      //         .Initialize("asd","valeri","waiting","","",null);

        }

        public void RegisterVillageQuestCharacter(VillageSceneCharacter character, GameObject arrow)
        {
            Instantiate(_questPointerPrefab, _questPointersTransform, false)
                .Initialize(character, arrow, _questPointersTransform);
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
                CheckWorkshopStove();
        }

        private void OnDestroy()
        {
            if (_scrollViewEventHandler != null)
                _scrollViewEventHandler.OnDragBegin -= OnDragBegin;

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<WorkshopSlotStartMeltingEvent>(OnSlotStartMelting);
                EventManager.Instance.Unsubscribe<WorkshopSlotCompleteEvent>(OnSlotComplete);
                EventManager.Instance.Unsubscribe<WorkshopSlotClearEvent>(OnSlotClear);
                EventManager.Instance.Unsubscribe<PickaxeSelectEvent>(OnPickaxeSelect);
            }
        }

        public void ScrollToCharacter(CharacterType type, Action onEnd)
        {
            switch (type)
            {
                case CharacterType.Valeri:
                    ScrollTo(_torchesMerchantRectTransform);
                    break;
                case CharacterType.Encelad:
                    ScrollTo(_enceladRectTransform);
                    break;
                case CharacterType.Burglar:
                    ScrollTo(_burglarRectTransform);
                    break;
                case CharacterType.Merchant:
                    ScrollTo(_shopRectTransform);
                    break;
                case CharacterType.Blacksmith:
                    ScrollTo(_blacksmithRectTransform);
                    break;
                case CharacterType.Pumpkin1:
                    ScrollTo(_enceladRectTransform);
                    break;
                case CharacterType.Pumpkin2:
                    ScrollTo(_blacksmithRectTransform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        public void ClickCharacter(CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Valeri:
                    TorchesMerchantBuildingClick();
                    break;
                case CharacterType.Encelad:
                    PvpArenaBuildingClick();
                    break;
                case CharacterType.Burglar:
                    break;
                case CharacterType.Merchant:
                    ShopBuildingClick();
                    break;
                case CharacterType.Blacksmith:
                    BlacksmithBuildingClick();
                    break;
                case CharacterType.Pumpkin1:
                    break;
                case CharacterType.Pumpkin2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public void OnDrag()
        {

            if (_worldScrollRect.content.localPosition.x > _scrollMin)
            {
                _worldScrollRect.content.localPosition = new Vector3(_scrollMin, _worldScrollRect.content.localPosition.y);
            }
            _scrollMax = _worldScrollRectTransform.rect.width - _worldRectTransform.rect.width - 130;

            if (_worldRectTransform.anchoredPosition.x < _scrollMax)
            {
                _worldScrollRect.content.localPosition = new Vector3(_scrollMax, _worldScrollRect.content.localPosition.y);
            }

        }

        private void OnDragBegin()
        {
            _worldScrollRect.content.DOKill();
        }

        private void OnSlotStartMelting(WorkshopSlotStartMeltingEvent eventData)
        {
            CheckWorkshopStove();
        }

        private void OnSlotComplete(WorkshopSlotCompleteEvent eventData)
        {
            CheckWorkshopStove();
        }

        private void OnSlotClear(WorkshopSlotClearEvent eventData)
        {
            CheckWorkshopStove();
        }

        private void OnPickaxeSelect(PickaxeSelectEvent eventData)
        {
            ShowCurrentPickaxe();
        }
        private void ScrollTo(RectTransform target, TweenCallback onComplete = null, bool immediately = false)
        {
            _worldScrollRect.content.DOKill();

            var contentWidthHalf = _worldScrollRect.content.sizeDelta.x / 2f;
            var targetPosition = target.anchoredPosition.x;
            var position = -(contentWidthHalf + targetPosition - _rootRectTransform.sizeDelta.x / 2f);
            var maxPosition = -(_worldScrollRect.content.sizeDelta.x - _rootRectTransform.sizeDelta.x);
            var positionElasticOffset = _rootRectTransform.sizeDelta.x / 10f;
            var clampedPosition = Mathf.Clamp(position, maxPosition - positionElasticOffset, positionElasticOffset);

            var distance = Mathf.Abs(_worldScrollRect.content.anchoredPosition.x - clampedPosition);
            var duration = distance / _scrollSpeed;

            _worldScrollRect.content
                .DOAnchorPosX(clampedPosition, immediately ? 0f : duration)
                .OnComplete(onComplete);
        }

        private void CheckWorkshopStove()
        {
            var isInactive = App.Instance.Player.Workshop.Slots.All(s => s.NecessaryAmount == s.CompleteAmount)
                 && App.Instance.Player.Workshop.SlotsShard.All(s => s.NecessaryAmount == s.CompleteAmount);
            _workshop.animation.Play(isInactive ? "Not_working" : "Working");
        }

        private void ShowCurrentPickaxe()
        {
            /*_pickaxeContainer.ClearChildElements();

            var path = PathsConstants.PrefabsResourcesPath + App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Number;
            var prefab = Resources.Load<GameObject>(path);

            Instantiate(prefab, _pickaxeContainer, false);*/
        }
    }
}