using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

namespace BlackTemple.EpicMine
{
    public class WindowRecipes : WindowBase
    {
        [SerializeField] private WindowRecipesRecipe _recipePrefab;

        [Space]
        [SerializeField] private Transform _recipesContainer;
        [SerializeField] private ScrollRect _recipesScrollView;

        [Space]
        [SerializeField] private Toggle _firstDozenTiers;
        [SerializeField] private Toggle _secondDozenTiers;
        [SerializeField] private Toggle _thirdDozenTiers;
        [SerializeField] private Toggle _fourthDozenTiers;
        [SerializeField] private Toggle _fifthDozenTiers;
        [SerializeField] private Toggle _hilts;

        [Space]
        [SerializeField] private RedDotBaseView _firstDozenTiersRedDot;
        [SerializeField] private RedDotBaseView _secondDozenTiersRedDot;
        [SerializeField] private RedDotBaseView _thirdDozenTiersRedDot;
        [SerializeField] private RedDotBaseView _fourthDozenTiersRedDot;
        [SerializeField] private RedDotBaseView _fifthDozenTiersRedDot;
        [SerializeField] private RedDotBaseView _hiltsRedDot;

        [Space]
        [SerializeField] private TextMeshProUGUI _firstDozenTiersText;
        [SerializeField] private TextMeshProUGUI _secondDozenTiersText;
        [SerializeField] private TextMeshProUGUI _thirdDozenTiersText;
        [SerializeField] private TextMeshProUGUI _fourthDozenTiersText;
        [SerializeField] private TextMeshProUGUI _fifthDozenTiersText;

        [Space]
        [SerializeField] private GameObject _hiltHint;

        [Space]
        [SerializeField] private Color _activeFilterTabColor;
        [SerializeField] private Color _inactiveFilterTabColor;

        private readonly List<WindowRecipesRecipe> _recipes = new List<WindowRecipesRecipe>();
        private Action<Core.Recipe, int> _onChooseRecipe;
        private Core.Recipe _recipe;


        public void Initialize(Action<Core.Recipe, int> onChooseRecipe = null)
        {
            _onChooseRecipe = onChooseRecipe;
        }

        public void Filter()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            foreach (var recipe in _recipes)
            {
                switch (recipe.Recipe.StaticRecipe.FilterCategory)
                {
                    case 1:
                        recipe.gameObject.SetActive(_firstDozenTiers.isOn);
                        break;
                    case 2:
                        recipe.gameObject.SetActive(_secondDozenTiers.isOn);
                        break;
                    case 3:
                        recipe.gameObject.SetActive(_thirdDozenTiers.isOn);
                        break;
                    case 4:
                        recipe.gameObject.SetActive(_fourthDozenTiers.isOn);
                        break;
                    case 5:
                        recipe.gameObject.SetActive(_fifthDozenTiers.isOn);
                        break;
                    case 6:
                        recipe.gameObject.SetActive(_hilts.isOn);
                        break;
                }
            }

            _recipesScrollView.verticalNormalizedPosition = 1;
            SetFilterTabsColors();
        }


        private void OnChooseAmount(int amount)
        {
            if (_onChooseRecipe == null)
            {
                var emptySlot = App.Instance.Player.Workshop.Slots.FirstOrDefault(s => s.IsUnlocked && s.StaticRecipe == null);
                emptySlot?.Start(_recipe, amount);
            }
            else 
                _onChooseRecipe.Invoke(_recipe, amount);

            _recipe = null;
            Close();
        }

        private void OnRecipeClick(Core.Recipe recipe)
        {
            _recipe = recipe;
            var windowAmount = WindowManager.Instance.Show<WindowChooseRecipeAmount>();
            windowAmount.Initialize(_recipe, OnChooseAmount);
        }

        private void SetFilterTabsColors()
        {
            _firstDozenTiersText.color = _firstDozenTiers.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _secondDozenTiersText.color = _secondDozenTiers.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _thirdDozenTiersText.color = _thirdDozenTiers.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _fourthDozenTiersText.color = _fourthDozenTiers.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _fifthDozenTiersText.color = _fifthDozenTiers.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            var recipes = App.Instance.Player.Workshop.Recipes.OrderByDescending(r => r.IsUnlocked).Where(x=> !x.StaticRecipe.Id.Contains("shard")).ToList();

            foreach (var staticRecipe in recipes)
            {
                var recipe = Instantiate(_recipePrefab, _recipesContainer, false);
                recipe.Initialize(staticRecipe, OnRecipeClick);
                _recipes.Add(recipe);
            }

            var tiersLocale = LocalizationHelper.GetLocale("tiers");
            _firstDozenTiersText.text = $"1-10 {tiersLocale}";
            _secondDozenTiersText.text = $"11-20 {tiersLocale}";
            _thirdDozenTiersText.text = $"21-30 {tiersLocale}";
            _fourthDozenTiersText.text = $"31-40 {tiersLocale}";
            _fifthDozenTiersText.text = $"41-50 {tiersLocale}";

            Filter();

            if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.UnlockTier))
            {
                OnRedDotRecipesChange(App.Instance.Controllers.RedDotsController.NewRecipes);
                App.Instance.Controllers.RedDotsController.OnRecipesChange += OnRedDotRecipesChange;

               _hiltHint.SetActive(!PlayerPrefsHelper.IsExist(PlayerPrefsType.WindowRecipesHiltHint));
            }
        }

        public void OnClickHiltHint()
        {
            PlayerPrefsHelper.Save(PlayerPrefsType.WindowRecipesHiltHint, true);
            _hiltHint.SetActive(false);
        }

        public override void OnClose()
        {
            base.OnClose();

            foreach (Transform recipe in _recipesContainer)
                Destroy(recipe.gameObject);

            _recipes.Clear();

            if (App.Instance != null)
            {
                App.Instance.Controllers.RedDotsController.OnRecipesChange -= OnRedDotRecipesChange;
            }
        }

        private void OnRedDotRecipesChange(List<string> unlockedRecipes)
        {
            var firstDozen = 0;
            var secondDozen = 0;
            var thirdDozen = 0;
            var fourthDozen = 0;
            var fifthDozen = 0;
            var hilts = 0;

            foreach (var recipe in _recipes)
            {
                recipe.HideRedDot();

                if (unlockedRecipes.Contains(recipe.Recipe.StaticRecipe.Id))
                {
                    recipe.ShowRedDot();

                    switch (recipe.Recipe.StaticRecipe.FilterCategory)
                    {
                        case 1:
                            firstDozen++;
                            break;
                        case 2:
                            secondDozen++;
                            break;
                        case 3:
                            thirdDozen++;
                            break;
                        case 4:
                            fourthDozen++;
                            break;
                        case 5:
                            fifthDozen++;
                            break;
                        case 6:
                            hilts++;
                            break;
                    }
                }

                _firstDozenTiersRedDot.Show(firstDozen);
                _secondDozenTiersRedDot.Show(secondDozen);
                _thirdDozenTiersRedDot.Show(thirdDozen);
                _fourthDozenTiersRedDot.Show(fourthDozen);
                _fifthDozenTiersRedDot.Show(fifthDozen);
                _hiltsRedDot.Show(hilts);
            }
        }
    }
}