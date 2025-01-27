using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

namespace BlackTemple.EpicMine
{
    public class WindowShardRecipes : WindowBase
    {
        [SerializeField] private WindowRecipesRecipe _recipePrefab;

        [Space]
        [SerializeField] private Transform _recipesContainer;
        [SerializeField] private ScrollRect _recipesScrollView;

        [SerializeField] private Toggle _combine;
        [SerializeField] private Toggle _separate;

        [SerializeField] private TextMeshProUGUI _combineText;
        [SerializeField] private TextMeshProUGUI _separateText;

        [Space]
        [SerializeField] private Color _activeFilterTabColor;
        [SerializeField] private Color _inactiveFilterTabColor;

        private readonly List<WindowRecipesRecipe> _recipes = new List<WindowRecipesRecipe>();
        private Action<Recipe, int> _onChooseRecipe;
        private Recipe _recipe;


        public void Initialize(Action<Recipe, int> onChooseRecipe = null)
        {
            _onChooseRecipe = onChooseRecipe;
        }

        public void Filter()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            _recipesScrollView.verticalNormalizedPosition = 1;
            SetFilterTabsColors();
        }


        private void OnChooseAmount(int amount)
        {
            if (_onChooseRecipe == null)
            {
                var emptySlot = App.Instance.Player.Workshop.SlotsShard.FirstOrDefault(s => s.IsUnlocked && s.StaticRecipe == null);
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

        public void OnClickTab()
        {

            if (_separate.isOn)
            {
                FillRecipies();
            }
            if(_combine.isOn)
            {
                FillRecipies(7);
            }
        }

        private void SetFilterTabsColors()
        {
            _combineText.color = _combine.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
            _separateText.color = _separate.isOn ? _activeFilterTabColor : _inactiveFilterTabColor;
        }

        public void FillRecipies(int filter = 8)
        {
            _recipesContainer.ClearChildObjects();

            var recipes = App.Instance.StaticData.Recipes.Where(x=>x.Id.Contains("shard") && x.FilterCategory == filter).ToList();
            foreach (var staticRecipe in recipes)
            {

                var rec = new Recipe(staticRecipe);
                var recipe = Instantiate(_recipePrefab, _recipesContainer, false);
                recipe.Initialize(rec, OnRecipeClick);
                _recipes.Add(recipe);
            }


            Filter();
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            _combine.isOn = true;
            OnClickTab();
        }

        public override void OnClose()
        {
            base.OnClose();

            _recipesContainer.ClearChildObjects();
            _recipes.Clear();

        }
    }
}
