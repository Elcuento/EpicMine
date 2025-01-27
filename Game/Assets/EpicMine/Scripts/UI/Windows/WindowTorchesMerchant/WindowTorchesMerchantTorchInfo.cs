using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowTorchesMerchantTorchInfo : MonoBehaviour
    {

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
 
        [Space] [SerializeField] private Toggle _craftTab;
        [SerializeField] private Toggle _bonusesTab;
        [SerializeField] private TextMeshProUGUI _craftTabText;
        [SerializeField] private TextMeshProUGUI _bonusesTabText;
        [SerializeField] private Color _activeTabColor;
        [SerializeField] private Color _inactiveTabColor;

        [Space] [SerializeField] private GameObject _craftPanel;

        [Space] [SerializeField] private GameObject _craftLockedPanel;
        [SerializeField] private TextMeshProUGUI _craftLockedPanelText;

        [Space] [SerializeField] private GameObject _craftDonatePanel;
        [SerializeField] private TextMeshProUGUI _craftDonatePanelCost;

        [Space] [SerializeField] private GameObject _craftAdPanel;
        [SerializeField] private TextMeshProUGUI _craftAdPanelDamageLevelText;
        [SerializeField] private TextMeshProUGUI _craftAdPanelDescriptionText;
        [SerializeField] private TextMeshProUGUI _craftAdPanelCost;

        [Space] [SerializeField] private GameObject _craftSelectPanel;
        [SerializeField] private TextMeshProUGUI _craftSelectPanelDescription;
        [SerializeField] private Image _craftSelectPanelButton;
        [SerializeField] private TextMeshProUGUI _craftSelectPanelFortuneLevelText;

        [Space] [SerializeField] private GameObject _craftCreatePanel;
        [SerializeField] private TextMeshProUGUI _craftCreatePanelCostText;
        [SerializeField] private ItemView _craftCreatePanelIngredientPrefab;
        [SerializeField] private Transform _craftCreatePanelIngredientsContainer;
        [SerializeField] private Image _craftCreatePanelButton;
        [SerializeField] private TextMeshProUGUI _craftCreatePanelDamageLevelText;

        [Space]
        [SerializeField] private GameObject _bonusesPanel;
        [SerializeField] private WindowTorchesMerchantTorchInfoBonusItem _bonusesPanelItemPrefab;
        [SerializeField] private Transform _bonusesPanelContainer;

        private Core.Torch _torch;


        public void Initialize(Core.Torch torch)
        {
            Clear();// 

            _torch = torch;

            _icon.sprite = SpriteHelper.GetIcon(_torch.StaticTorch.Id);
            _icon.color = _torch.IsCreated || _torch.IsUnlocked ? Color.white : Color.black;
            _title.text = LocalizationHelper.GetLocale(_torch.StaticTorch.Id);

            UpdateRequiredFortuneLevelTexts();
            UpdateCraftPanel();
            UpdateBonusesPanel();
            OnTabClick();
        }

        public void ReInitialize()
        {
            Initialize(_torch);
        }

        public void Select()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            if (App.Instance.Player.Skills.Fortune.Number + 1 >= _torch.StaticTorch.RequiredFortuneLevel)
            {
                App.Instance.Player.TorchesMerchant.Select(_torch);
                UpdateCraftSelectPanel();
            }
        }

        public void Create()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            var cost = new Dto.Currency(CurrencyType.Gold, _torch.StaticTorch.Cost);

            if (!App.Instance.Player.Wallet.Has(cost))
            {
                WindowManager.Instance.Show<WindowNotEnoughCurrency>()
                    .Initialize("window_not_enough_currency_gold_discription", "window_not_enough_currency_buy",
                        () =>
                        {
                            WindowManager.Instance.Show<WindowShop>()
                                .OpenGold();
                        });

                return;
            }

            _torch.Create();
        }

        public void OnTabClick()
        {
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.Click);

            _craftTabText.color = _inactiveTabColor;
            _bonusesTabText.color = _inactiveTabColor;
            _craftPanel.SetActive(false);
            _bonusesPanel.SetActive(false);

            if (_craftTab.isOn)
            {
                _craftTab.transform.SetAsLastSibling();
                _craftTabText.color = _activeTabColor;
                _craftPanel.SetActive(true);
            }
            else
            {
                _bonusesTab.transform.SetAsLastSibling();
                _bonusesTabText.color = _activeTabColor;
                _bonusesPanel.SetActive(true);
            }
        }


        private void UpdateRequiredFortuneLevelTexts()
        {
            var text = App.Instance.Player.Skills.Fortune.Number + 1 >= _torch.StaticTorch.RequiredFortuneLevel
                ? _torch.StaticTorch.RequiredFortuneLevel.ToString()
                : "<color=" + Colors.ResourceNotEnoughAmountColor + ">" + _torch.StaticTorch.RequiredFortuneLevel +
                  "</color>";

            _craftAdPanelDamageLevelText.text = text;
            _craftSelectPanelFortuneLevelText.text = text;
            _craftCreatePanelDamageLevelText.text = text;
        }

        private void UpdateCraftPanel()
        {
            if (_torch.IsCreated)
            {
                _craftSelectPanel.SetActive(true);
                UpdateCraftSelectPanel();
                return;
            }

            if (!_torch.IsUnlocked)
            {
                var locale = LocalizationHelper.GetLocale("window_torches_merchant_locked");
                var text = string.Format(locale, _torch.StaticTorch.LeagueId);


                _craftLockedPanelText.text = text;
                _craftLockedPanel.SetActive(true);
                return;
            }

            if (_torch.StaticTorch.Type == TorchType.Ad)
            {
                App.Instance.Player.TorchesMerchant.AdTorches.TryGetValue(_torch.StaticTorch.Id, out var currentVal);
                var leftVal = _torch.StaticTorch.Cost - currentVal;

                _craftAdPanel.SetActive(true);
                _craftAdPanelCost.text = leftVal.ToString();
                var descriptionLocale = LocalizationHelper.GetLocale("window_torches_merchant_ad_description");
                _craftAdPanelDescriptionText.text = string.Format(descriptionLocale, leftVal);
                return;
            }

            if (_torch.StaticTorch.Type == TorchType.Reward)
            {
                var descriptionLocale = LocalizationHelper.GetLocale("window_torches_merchant_ad_description");
                _craftAdPanelDescriptionText.text = descriptionLocale;
                return;
            }

            _craftCreatePanel.SetActive(true);
            UpdateCraftCreatePanel();
        }

        private void UpdateCraftSelectPanel()
        {
            if (_torch == App.Instance.Player.TorchesMerchant.SelectedTorch)
            {
                _craftSelectPanelDescription.text = LocalizationHelper.GetLocale("window_torches_merchant_already_in_use");
                _craftSelectPanelButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
            }
            else
            {
                if (App.Instance.Player.Skills.Fortune.Number + 1 >= _torch.StaticTorch.RequiredFortuneLevel)
                {
                    _craftSelectPanelDescription.text =
                        LocalizationHelper.GetLocale("window_torches_merchant_available_for_select");
                    _craftSelectPanelButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGreen;
                }
                else
                {
                    _craftSelectPanelDescription.text =
                        LocalizationHelper.GetLocale("window_torches_merchant_invalid_fortune_level");
                    _craftSelectPanelButton.sprite = App.Instance.ReferencesTables.Sprites.ButtonGrey;
                }
            }
        }

        private void UpdateCraftCreatePanel()
        {
            _craftCreatePanelIngredientsContainer.ClearChildObjects();

            var ingredients = StaticHelper.GetIngredients(_torch.StaticTorch);
            var hasAllIngredients = true;

            foreach (var dtoItem in ingredients)
            {
             
                var ingredient = Instantiate(_craftCreatePanelIngredientPrefab, _craftCreatePanelIngredientsContainer,
                    false);
                var existAmount = App.Instance.Player.Inventory.GetExistAmount(dtoItem.Id);
                string amount;

                if (existAmount >= dtoItem.Amount)
                    amount = dtoItem.Amount.ToString();
                else
                {
                    amount = "<color=" + Colors.ResourceNotEnoughAmountColor + ">" + dtoItem.Amount + "</color>";
                    hasAllIngredients = false;
                }

                ingredient.Initialize(dtoItem.Id, amount);
            }

            var goldExist =
                App.Instance.Player.Wallet.Has(new Dto.Currency(CurrencyType.Gold, _torch.StaticTorch.Cost));
            _craftCreatePanelCostText.text = goldExist
                ? _torch.StaticTorch.Cost.ToString()
                : "<color=" + Colors.ResourceNotEnoughAmountColor + ">" + _torch.StaticTorch.Cost + "</color>";

            if (!goldExist)
                hasAllIngredients = false;

            _craftCreatePanelButton.sprite = hasAllIngredients
                ? App.Instance.ReferencesTables.Sprites.ButtonGrown
                : App.Instance.ReferencesTables.Sprites.ButtonGrey;
        }

        private void UpdateBonusesPanel()
        {
            _bonusesPanelContainer.ClearChildObjects();

               if (_torch.IsCreated || _torch.IsUnlocked)
               {

                   var torchExplosiveDamage = _torch.StaticTorch.ExplosiveStrikeDamage ?? 0;
                   var torchExplosiveCooldown = _torch.StaticTorch.ExplosiveStrikeCooldown ?? 0;

                if (torchExplosiveDamage > 0 || torchExplosiveCooldown > 0)

                {
                    var expLocalate = LocalizationHelper.GetLocale("explosivestrike_ability");
                    var expbon = AddBonusItem(App.Instance.ReferencesTables.Sprites.ExplosiveStrikeAbilityIcon,
                        expLocalate);

                    if (torchExplosiveDamage > 0)
                    {
                        expbon.AddBonus(App.Instance.ReferencesTables.Sprites.AbilityDamageIcon, LocalizationHelper.GetLocale("window_upgrade_ability_damage") ,
                            $"+{torchExplosiveDamage}");
                    }

                    if (torchExplosiveCooldown > 0)
                    {
                        expbon.AddBonus(App.Instance.ReferencesTables.Sprites.AbilityCooldownIcon, LocalizationHelper.GetLocale("window_upgrade_ability_cooldown"),
                            $"-{torchExplosiveCooldown} {LocalizationHelper.GetLocale("seconds")}");
                    }
                }

                   var torchAcidDamage = _torch.StaticTorch.AcidDamage ?? 0;
                   var torchAcidCooldown = _torch.StaticTorch.AcidCooldown ?? 0;

                if (torchAcidDamage > 0 || torchAcidCooldown > 0)
                {
                    var acidLocalate = LocalizationHelper.GetLocale("acid_ability");

                    var acidbon = AddBonusItem(App.Instance.ReferencesTables.Sprites.AcidAbilityIcon, acidLocalate);

                    if (torchAcidDamage > 0)
                    {

                        acidbon.AddBonus(App.Instance.ReferencesTables.Sprites.AbilityDamageIcon, LocalizationHelper.GetLocale("window_upgrade_ability_damage_per_second"),
                            $"+{torchAcidDamage}");
                    }

                    if (torchAcidCooldown > 0)
                    {
                        acidbon.AddBonus(App.Instance.ReferencesTables.Sprites.AbilityCooldownIcon, LocalizationHelper.GetLocale("window_upgrade_ability_cooldown"),
                            $"-{torchAcidCooldown} {LocalizationHelper.GetLocale("seconds")}");
                    }
                }

                   var torchFreezeDamage = _torch.StaticTorch.FreezingDamage ?? 0;
                   var torchFreezeAdditionalParameter = _torch.StaticTorch.FreezingAdditionalParameter ?? 0;
                   var torchFreezeCooldown = _torch.StaticTorch.FreezingCooldown ?? 0;


                if (torchFreezeDamage > 0 ||
                    torchFreezeAdditionalParameter > 0 ||
                    torchFreezeCooldown > 0)
                {

                    var freezeLocalate = LocalizationHelper.GetLocale("freezing_ability");
                    var freezbon = AddBonusItem(App.Instance.ReferencesTables.Sprites.FreezingAbilityIcon, freezeLocalate);

                    if (torchFreezeDamage > 0)
                    {

                        freezbon.AddBonus(App.Instance.ReferencesTables.Sprites.AbilityDamageIcon,
                            LocalizationHelper.GetLocale("window_upgrade_ability_additional_damage"),
                            $"+{torchFreezeDamage}");
                    }

                    if (torchFreezeCooldown > 0)
                    {
                        freezbon.AddBonus(App.Instance.ReferencesTables.Sprites.AbilityCooldownIcon,
                            LocalizationHelper.GetLocale("window_upgrade_ability_cooldown"),
                            $"-{torchFreezeCooldown} {LocalizationHelper.GetLocale("seconds")}");
                    }

                    if (torchFreezeAdditionalParameter > 0)
                    {
                        freezbon.AddBonus(App.Instance.ReferencesTables.Sprites.FreezingAbilityAdditionalIcon,
                            LocalizationHelper.GetLocale("window_upgrade_ability_additional_damage"),
                            $"+{torchFreezeAdditionalParameter}");
                    }
                }
            }
        }

        private WindowTorchesMerchantTorchInfoBonusItem AddBonusItem(Sprite sprite, string text)
        {
            var item = Instantiate(_bonusesPanelItemPrefab, _bonusesPanelContainer, false);
            item.Initialize(sprite, text);
            return item;
        }

        private void Clear()
        {
            _craftLockedPanel.SetActive(false);
            _craftDonatePanel.SetActive(false);
            _craftAdPanel.SetActive(false);
            _craftCreatePanel.SetActive(false);
            _craftSelectPanel.SetActive(false);
        }
    }

}