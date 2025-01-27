using System;
using System.Linq;
using BlackTemple.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowRecipesRecipe : MonoBehaviour
    {
        public Core.Recipe Recipe { get; private set; }
        public bool CanCreate { get; private set; }

        [SerializeField] private GameObject _lockedPanel;
        [SerializeField] private GameObject _unlockedPanel;

        [SerializeField] private ItemView _ingredientPrefab;
        [SerializeField] private Sprite _buttonCreateEnabledSprite;
        [SerializeField] private Sprite _buttonCreateDisabledSprite;

        [Space]
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _iconCountTitle;
        [SerializeField] private Image _priceIcon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _craftTime;
        [SerializeField] private TextMeshProUGUI _price;
        [SerializeField] private RectTransform _ingredientsContainer;

        [Space]
        [SerializeField] private Color _shardColor;
        [SerializeField] private Color _ingotColor;

        [SerializeField] private Image _lockedBackground;
        [SerializeField] private Image _lockedIconBackground;

        [SerializeField] private Image _lockedLine1;
        [SerializeField] private Image _lockedLine2;
        [SerializeField] private Image _lockedLine3;

        [Space]
        [SerializeField] private Image _unlockBackground;
        [SerializeField] private Image _unlockIconBackground;

        [SerializeField] private Image _unlockLine1;
        [SerializeField] private Image _unlockLine2;
        [SerializeField] private Image _unlockLine3;


        [Space]
        [SerializeField] private Image _createButtonImage;
        [SerializeField] private TextMeshProUGUI _createButtonLabel;
        [SerializeField] private GameObject _createButton;

        [Space]
        [SerializeField] private GameObject _redDot;
        [SerializeField] private RectTransform _rect;

        private Action<Core.Recipe> _onClick;

        private Camera _camera;


        public void Initialize(Core.Recipe recipe, Action<Core.Recipe> onClick)
        {
            _lockedPanel.SetActive(!recipe.IsUnlocked);
            _unlockedPanel.SetActive(recipe.IsUnlocked);

            Recipe = recipe;
            _onClick = onClick;

            _icon.sprite = SpriteHelper.GetIcon(recipe.StaticRecipe.Id);
            _title.text = LocalizationHelper.GetLocale(recipe.StaticRecipe.Id);
            _craftTime.text = TimeHelper.Format(recipe.StaticRecipe.CraftTime, detailed: true);

            var resource = App.Instance.StaticData.Resources.FirstOrDefault(r => r.Id == recipe.StaticRecipe.Id);
            if (resource != null)
            {
                _price.text = resource.Price > 0 ? resource.Price.ToString() : "";
                _priceIcon.enabled = resource.Price > 0;
            }
            else
            {
                var hilt = App.Instance.StaticData.Hilts.FirstOrDefault(h => h.Id == recipe.StaticRecipe.Id);
                if (hilt != null)
                    _price.text = hilt.Price.ToString();
            }

            CheckCanCreate();
            _createButtonImage.sprite = CanCreate ? _buttonCreateEnabledSprite : _buttonCreateDisabledSprite;

            UpdateIngredients();

            _createButtonLabel.text = recipe.StaticRecipe.FilterCategory == 8
                ? LocalizationHelper.GetLocale("window_shard_recipes_create_button")
                : LocalizationHelper.GetLocale("window_recipes_create_button");

            _iconCountTitle.text = recipe.StaticRecipe.Amount > 1 ? recipe.StaticRecipe.Amount.ToString() : "";

            SetColors();

        }

        public void SetColors()
        {
            var backColor = Recipe.StaticRecipe.FilterCategory == 8 || Recipe.StaticRecipe.FilterCategory == 7 ? _shardColor : _ingotColor;

            _unlockBackground.color = backColor;
            _unlockIconBackground.color = backColor;
            _unlockLine1.color = backColor;
            _unlockLine2.color = backColor;
            _unlockLine3.color = backColor;

            _lockedBackground.color = backColor;
            _lockedIconBackground.color = backColor;
            _lockedLine1.color = backColor;
            _lockedLine2.color = backColor;
            _lockedLine3.color = backColor;
        }

        public void Create()
        {
            if (!CanCreate)
                return;

            _onClick(Recipe);
        }

        public void ShowRedDot()
        {
            _redDot.SetActive(true);
        }

        public void HideRedDot()
        {
            _redDot.SetActive(false);
        }


        private void Start()
        {
            Camera worldCamera = null;

            var uiCameraGo = GameObject.FindWithTag(Tags.UICamera);
            if (uiCameraGo != null)
                worldCamera = uiCameraGo.GetComponent<Camera>();

            if (worldCamera == null)
                worldCamera = Camera.main;

            _camera = worldCamera;
        }

        private void Update()
        {
            if (!_redDot.activeSelf)
                return;

            var isVisible = _rect.IsFullyVisibleFrom(_camera);
            if (!isVisible)
                return;

            App.Instance.Controllers.RedDotsController.ViewRecipe(Recipe.StaticRecipe.Id);
        }

        private void CheckCanCreate()
        {
            CanCreate = true;
            var ingredients = StaticHelper.GetIngredients(Recipe.StaticRecipe);

            foreach (var ingredient in ingredients)
            {
                var exist = App.Instance.Player.Inventory.GetExistAmount(ingredient.Id);
                var maxAmount = exist / ingredient.Amount;
                if (maxAmount < 1)
                {
                    CanCreate = false;
                    return;
                }
            }

            if (CanCreate)
            {
                CanCreate = Recipe.StaticRecipe.Id.Contains("shard") ? App.Instance.Player.Workshop.IsEmptyShardSlotExists : 
                    App.Instance.Player.Workshop.IsEmptySlotExists;

            }
        }

        private void UpdateIngredients()
        {
            _ingredientsContainer.ClearChildObjects();

            foreach (var ingredient in StaticHelper.GetIngredients(Recipe.StaticRecipe))
                CreateIngredient(ingredient.Id, ingredient.Amount);
        }

        private void CreateIngredient(string itemStaticId, int amount)
        {
            var ingredient = Instantiate(_ingredientPrefab, _ingredientsContainer, false);
            var existAmount = App.Instance.Player.Inventory.GetExistAmount(itemStaticId);
            var template = existAmount >= amount
                ? "{0}/{1}"
                : "{0}/<color=#BF6262>{1}</color>";
            ingredient.Initialize(itemStaticId, string.Format(template, existAmount, amount));
        }
    }
}